using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InteractionHandler_ListenVoice : InteractionHandlerComponent
{
    [Header("OpenAI STT")]
    [SerializeField] private string apiKey = "";
    [SerializeField] private string model = "gpt-4o-mini-transcribe";
    [SerializeField] private string language = "";
    [SerializeField] private bool normalizeText = true;
    [SerializeField] private string normalizeModel = "gpt-4o-mini";

    [Header("WebGL Proxy")]
    [SerializeField] private string webglTranscriptionUrl = "";
    [SerializeField] private string webglNormalizeUrl = "";
    [SerializeField] private string webglProxyBearerToken = "";

    [Header("Recording")]
    [SerializeField] private int sampleRate = 16000;
    [SerializeField] private int maxRecordingSeconds = 20;
    [SerializeField] private float minRecordingSeconds = 0.25f;
    [SerializeField] private string microphoneDevice = "";
    [SerializeField] private bool autoStartOnInteraction = true;
    [SerializeField] private bool requireUserGestureOnWebGL = true;

    [Header("Debug")]
    [SerializeField] private string recognizedText;
    [SerializeField] private bool isRecording;
    [SerializeField] private bool isTranscribing;

    public Text recognizedText_Text;

    public GameObject buttonShow;

    private AudioClip recordedClip;
    private bool waitingForFinish;
    private Coroutine startListeningCoroutine;
    private Coroutine finishWhenReadyCoroutine;
    private float recordingStartedAt;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void JS_Microphone_Start(string gameObjectName, string onReadyMethod, string onErrorMethod);

    [DllImport("__Internal")]
    private static extern void JS_Microphone_StopAndTranscribe(
        string gameObjectName,
        string onResultMethod,
        string onErrorMethod,
        string endpoint,
        string bearerToken,
        string model,
        string language);

    [DllImport("__Internal")]
    private static extern void JS_Microphone_Stop();
#endif

    public string RecognizedText => recognizedText;

    public override void InteractionStart(TableDataItem data)
    {
        base.InteractionStart(data);

        if (buttonShow != null)
            buttonShow.SetActive(true);

        if (!autoStartOnInteraction)
            return;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (requireUserGestureOnWebGL)
        {
            // 브라우저 정책상 사용자 제스처 없이 마이크를 여는 시도가 차단될 수 있어 수동 시작을 허용합니다.
            Debug.Log("WebGL microphone start is waiting for user click. Bind StartListeningFromUserClick() to a UI button.");
            return;
        }
#endif

        StartListening();
    }

    [ContextMenu("Test Start")]
    public void TestStart()
    {
        StartListening();
    }

    public void StartListeningFromUserClick()
    {
        StartListening();
    }

    [ContextMenu("Test Finish")]
    public override void InteractionFinish()
    {
        if (isTranscribing)
        {
            Debug.LogWarning("Speech-to-text is already running.");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        if (!isRecording)
        {
            Debug.LogWarning("No active voice recording.");
            return;
        }

        string endpoint = GetTranscriptionUrl();
        string token = GetWebGLBearerToken();

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            Debug.LogError("Transcription URL is empty.");
            return;
        }

        if (!HasAuthorization(endpoint))
        {
            Debug.LogError("Authorization is empty for this endpoint. Set apiKey or webglProxyBearerToken.");
            return;
        }

        waitingForFinish = true;
        isRecording = false;
        isTranscribing = true;

        try
        {
            // WebGL에서는 JS 브리지에서 녹음 중지와 업로드를 함께 처리해야 브라우저 제약 대응이 쉽습니다.
            JS_Microphone_StopAndTranscribe(
                gameObject.name,
                nameof(OnWebGLTranscriptionResult),
                nameof(OnWebGLMicError),
                endpoint,
                token,
                model,
                language);
        }
        catch (Exception ex)
        {
            Debug.LogError("WebGL stop/transcribe call failed: " + ex.Message);
            isTranscribing = false;
        }

        return;
#else
        if (!isRecording && startListeningCoroutine != null)
        {
            // 시작 코루틴이 아직 마이크를 열지 못한 상태에서도 종료 요청을 잃지 않도록 대기 코루틴으로 연결합니다.
            if (finishWhenReadyCoroutine != null)
                StopCoroutine(finishWhenReadyCoroutine);

            finishWhenReadyCoroutine = StartCoroutine(FinishWhenRecordingReady());
            return;
        }

        if (!isRecording || recordedClip == null)
        {
            Debug.LogWarning("No active voice recording.");
            return;
        }

        float elapsed = Time.realtimeSinceStartup - recordingStartedAt;
        if (elapsed < minRecordingSeconds)
        {
            // 너무 짧은 녹음은 빈/불안정 결과가 많아 최소 녹음 시간을 보장합니다.
            if (finishWhenReadyCoroutine != null)
                StopCoroutine(finishWhenReadyCoroutine);

            finishWhenReadyCoroutine = StartCoroutine(WaitThenFinish(minRecordingSeconds - elapsed));
            return;
        }

        waitingForFinish = true;
        StopAndTranscribe();
#endif
    }

    public void StartListening()
    {
        if (isRecording)
        {
            Debug.LogWarning("Voice recording is already running.");
            return;
        }

        if (sampleRate <= 0)
            sampleRate = 16000;

        if (maxRecordingSeconds <= 0)
            maxRecordingSeconds = 20;

        recognizedText = string.Empty;
        waitingForFinish = false;

        if (finishWhenReadyCoroutine != null)
        {
            StopCoroutine(finishWhenReadyCoroutine);
            finishWhenReadyCoroutine = null;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        string endpoint = GetTranscriptionUrl();
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            Debug.LogError("Transcription URL is empty.");
            return;
        }

        if (!HasAuthorization(endpoint))
        {
            Debug.LogError("Authorization is empty for this endpoint. Set apiKey or webglProxyBearerToken.");
            return;
        }

        try
        {
            // 권한 팝업 처리와 실제 녹음 시작은 브라우저 API가 담당하므로 JS 브리지 진입점을 호출합니다.
            JS_Microphone_Start(gameObject.name, nameof(OnWebGLMicReady), nameof(OnWebGLMicError));
        }
        catch (Exception ex)
        {
            Debug.LogError("WebGL microphone start call failed: " + ex.Message);
        }
#else
        if (startListeningCoroutine != null)
            StopCoroutine(startListeningCoroutine);

        startListeningCoroutine = StartCoroutine(StartListeningRoutine());
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    public void OnWebGLMicReady(string result)
    {
        isRecording = true;
        recordingStartedAt = Time.realtimeSinceStartup;
        Debug.Log("Voice recording started.");
    }

    public void OnWebGLTranscriptionResult(string text)
    {
        recognizedText = text ?? string.Empty;
        StartCoroutine(FinalizeTranscriptionRoutine());
    }

    public void OnWebGLMicError(string error)
    {
        Debug.LogError("WebGL microphone/STT error: " + error);
        isRecording = false;
        isTranscribing = false;
    }
#endif

    private IEnumerator FinalizeTranscriptionRoutine()
    {
        if (normalizeText && !string.IsNullOrWhiteSpace(recognizedText))
            yield return StartCoroutine(NormalizeTranscriptCoroutine());

        Debug.Log("Recognized Voice Text: " + recognizedText);

        isTranscribing = false;

        // 후처리/화면 갱신이 끝나기 전에 다음 그룹이 시작되지 않도록 짧게 완충 시간을 둡니다.
        yield return new WaitForSeconds(5f);


        if (waitingForFinish)
            base.InteractionFinish();
    }

#if !UNITY_WEBGL || UNITY_EDITOR
    private IEnumerator StartListeningRoutine()
    {
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.LogError("Microphone permission denied.");
            startListeningCoroutine = null;
            yield break;
        }

        recordedClip = Microphone.Start(microphoneDevice, false, maxRecordingSeconds, sampleRate);

        float waitStart = Time.realtimeSinceStartup;
        while (Microphone.GetPosition(microphoneDevice) <= 0)
        {
            if (Time.realtimeSinceStartup - waitStart > 2f)
                break;

            yield return null;
        }

        if (recordedClip == null)
        {
            Debug.LogError("Failed to start microphone recording.");
            if (Microphone.IsRecording(microphoneDevice))
                Microphone.End(microphoneDevice);
            startListeningCoroutine = null;
            yield break;
        }

        isRecording = true;
        recordingStartedAt = Time.realtimeSinceStartup;
        Debug.Log("Voice recording started.");
        startListeningCoroutine = null;
    }

    private IEnumerator FinishWhenRecordingReady()
    {
        float timeout = Time.realtimeSinceStartup + 2f;

        while (!isRecording && startListeningCoroutine != null && Time.realtimeSinceStartup < timeout)
            yield return null;

        finishWhenReadyCoroutine = null;

        if (!isRecording || recordedClip == null)
        {
            Debug.LogWarning("No active voice recording.");
            yield break;
        }

        waitingForFinish = true;
        StopAndTranscribe();
    }

    private IEnumerator WaitThenFinish(float waitSeconds)
    {
        if (waitSeconds > 0f)
            yield return new WaitForSeconds(waitSeconds);

        finishWhenReadyCoroutine = null;

        if (!isRecording || recordedClip == null)
        {
            Debug.LogWarning("No active voice recording.");
            yield break;
        }

        waitingForFinish = true;
        StopAndTranscribe();
    }

    private void StopAndTranscribe()
    {
        int samples = Microphone.GetPosition(microphoneDevice);

        if (recordedClip != null)
        {
            float elapsed = Mathf.Max(0f, Time.realtimeSinceStartup - recordingStartedAt);
            int estimated = Mathf.RoundToInt(elapsed * recordedClip.frequency);
            int minSamples = Mathf.RoundToInt(Mathf.Max(0.1f, minRecordingSeconds) * recordedClip.frequency);

            if (samples <= 0)
                samples = estimated;

            if (samples <= 0)
                samples = minSamples;

            int clipSamples = Mathf.Max(0, recordedClip.samples);
            if (clipSamples > 0)
                samples = Mathf.Clamp(samples, 1, clipSamples);
        }

        Microphone.End(microphoneDevice);
        isRecording = false;

        if (samples <= 0 || recordedClip == null)
        {
            Debug.LogWarning("No recorded audio samples found.");
            return;
        }

        // STT 업로드 호환성을 위해 PCM 데이터를 wav 포맷으로 감싸 전송합니다.
        byte[] wavBytes = WavUtility.FromAudioClip(recordedClip, samples);
        StartCoroutine(TranscribeCoroutine(wavBytes));

        //if (buttonShow != null)
        //    buttonShow.SetActive(false);
    }

    private IEnumerator TranscribeCoroutine(byte[] wavBytes)
    {
        isTranscribing = true;

        string transcriptionUrl = GetTranscriptionUrl();
        if (string.IsNullOrWhiteSpace(transcriptionUrl))
        {
            Debug.LogError("Transcription URL is empty.");
            isTranscribing = false;
            yield break;
        }

        if (!HasAuthorization(transcriptionUrl))
        {
            Debug.LogError("Authorization is empty for this endpoint. Set apiKey or webglProxyBearerToken.");
            isTranscribing = false;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("model", model);

        if (!string.IsNullOrWhiteSpace(language))
            form.AddField("language", language);

        form.AddBinaryData("file", wavBytes, "speech.wav", "audio/wav");

        using (UnityWebRequest req = UnityWebRequest.Post(transcriptionUrl, form))
        {
            ApplyAuthorization(req);
            req.timeout = 60;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("STT request failed: " + req.error + " / " + req.downloadHandler.text);
                isTranscribing = false;
                yield break;
            }

            string json = req.downloadHandler.text;
            TranscriptionResponse response = JsonUtility.FromJson<TranscriptionResponse>(json);
            recognizedText = response != null ? response.text : string.Empty;
        }

        yield return StartCoroutine(FinalizeTranscriptionRoutine());
    }
#endif

    private IEnumerator NormalizeTranscriptCoroutine()
    {
        string normalizeUrl = GetNormalizeUrl();
        if (string.IsNullOrWhiteSpace(normalizeUrl))
            yield break;

        if (!HasAuthorization(normalizeUrl))
            yield break;

        string dominantLanguageCode = DetectDominantLanguageCode(recognizedText);

        ChatCompletionsRequest payload = new ChatCompletionsRequest
        {
            model = normalizeModel,
            temperature = 0f,
            messages = new ChatMessage[]
            {
                new ChatMessage
                {
                    role = "system",
                    // 후처리를 별도 호출로 분리해 STT의 원문 인식과 표기 정규화를 독립적으로 조정할 수 있게 합니다.
                    content = "You post-process multilingual speech transcripts. Translate the full transcript into the required target language code provided by the user message. Return only one final sentence in that target language's standard script. Preserve meaning and keep names/brands naturally. If confidence is low, keep original wording but still prefer the target language."
                },
                new ChatMessage
                {
                    role = "user",
                    content = "target_language=" + dominantLanguageCode + "\\ntranscript=" + recognizedText
                }
            }
        };

        string body = JsonUtility.ToJson(payload);
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        using (UnityWebRequest req = new UnityWebRequest(normalizeUrl, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            ApplyAuthorization(req);
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 45;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Normalize request failed. Keeping raw transcript. " + req.error);
                yield break;
            }

            ChatCompletionsResponse parsed = JsonUtility.FromJson<ChatCompletionsResponse>(req.downloadHandler.text);
            if (parsed != null &&
                parsed.choices != null &&
                parsed.choices.Length > 0 &&
                parsed.choices[0].message != null &&
                !string.IsNullOrWhiteSpace(parsed.choices[0].message.content))
            {
                recognizedText = parsed.choices[0].message.content.Trim();

                if (recognizedText_Text != null)
                    recognizedText_Text.text = recognizedText;
            }
        }
    }

    protected override void OnDisable()
    {
        if (startListeningCoroutine != null)
        {
            StopCoroutine(startListeningCoroutine);
            startListeningCoroutine = null;
        }

        if (finishWhenReadyCoroutine != null)
        {
            StopCoroutine(finishWhenReadyCoroutine);
            finishWhenReadyCoroutine = null;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            JS_Microphone_Stop();
        }
        catch
        {
        }
#else
        if (isRecording)
            Microphone.End(microphoneDevice);
#endif

        isRecording = false;
        isTranscribing = false;
        base.OnDisable();
    }

    private string GetTranscriptionUrl()
    {
        if (!string.IsNullOrWhiteSpace(webglTranscriptionUrl))
            return webglTranscriptionUrl;

        return "https://api.openai.com/v1/audio/transcriptions";
    }

    private string GetNormalizeUrl()
    {
        if (!string.IsNullOrWhiteSpace(webglNormalizeUrl))
            return webglNormalizeUrl;

        return "https://api.openai.com/v1/chat/completions";
    }

    private string GetWebGLBearerToken()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!string.IsNullOrWhiteSpace(webglProxyBearerToken))
            return NormalizeAuthorizationToken(webglProxyBearerToken);

        return NormalizeAuthorizationToken(apiKey);
#else
        return NormalizeAuthorizationToken(apiKey);
#endif
    }

    private void ApplyAuthorization(UnityWebRequest req)
    {
        string token = GetWebGLBearerToken();
        if (!string.IsNullOrWhiteSpace(token))
            req.SetRequestHeader("Authorization", "Bearer " + token);
    }

    private bool HasAuthorization(string requestUrl)
    {
        string token = GetWebGLBearerToken();
        if (!string.IsNullOrWhiteSpace(token))
            return true;

        return IsCustomProxyUrl(requestUrl);
    }

    private static bool IsCustomProxyUrl(string requestUrl)
    {
        if (string.IsNullOrWhiteSpace(requestUrl))
            return false;

        return requestUrl.IndexOf("api.openai.com", StringComparison.OrdinalIgnoreCase) < 0;
    }

    private static string NormalizeAuthorizationToken(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return string.Empty;

        string token = rawToken.Trim();

        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = token.Substring("Bearer ".Length).Trim();

        return token;
    }

    [Serializable]
    private class TranscriptionResponse
    {
        public string text;
    }

    [Serializable]
    private class ChatCompletionsRequest
    {
        public string model;
        public float temperature;
        public ChatMessage[] messages;
    }

    [Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class ChatCompletionsResponse
    {
        public ChatChoice[] choices;
    }

    [Serializable]
    private class ChatChoice
    {
        public ChatMessage message;
    }

    private static class WavUtility
    {
        public static byte[] FromAudioClip(AudioClip clip, int samplesToUse)
        {
            int channels = clip.channels;
            float[] samples = new float[samplesToUse * channels];
            clip.GetData(samples, 0);

            byte[] pcmBytes = ConvertSamplesTo16BitPcm(samples);
            return BuildWav(pcmBytes, channels, clip.frequency);
        }

        private static byte[] ConvertSamplesTo16BitPcm(float[] samples)
        {
            byte[] bytes = new byte[samples.Length * 2];
            int offset = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                short value = (short)Mathf.Clamp(samples[i] * short.MaxValue, short.MinValue, short.MaxValue);
                byte[] shortBytes = BitConverter.GetBytes(value);
                bytes[offset++] = shortBytes[0];
                bytes[offset++] = shortBytes[1];
            }

            return bytes;
        }

        private static byte[] BuildWav(byte[] pcmData, int channels, int frequency)
        {
            int headerSize = 44;
            byte[] wav = new byte[headerSize + pcmData.Length];

            Encoding.ASCII.GetBytes("RIFF").CopyTo(wav, 0);
            BitConverter.GetBytes(wav.Length - 8).CopyTo(wav, 4);
            Encoding.ASCII.GetBytes("WAVE").CopyTo(wav, 8);

            Encoding.ASCII.GetBytes("fmt ").CopyTo(wav, 12);
            BitConverter.GetBytes(16).CopyTo(wav, 16);
            BitConverter.GetBytes((short)1).CopyTo(wav, 20);
            BitConverter.GetBytes((short)channels).CopyTo(wav, 22);
            BitConverter.GetBytes(frequency).CopyTo(wav, 24);

            int byteRate = frequency * channels * 2;
            BitConverter.GetBytes(byteRate).CopyTo(wav, 28);
            BitConverter.GetBytes((short)(channels * 2)).CopyTo(wav, 32);
            BitConverter.GetBytes((short)16).CopyTo(wav, 34);

            Encoding.ASCII.GetBytes("data").CopyTo(wav, 36);
            BitConverter.GetBytes(pcmData.Length).CopyTo(wav, 40);

            Buffer.BlockCopy(pcmData, 0, wav, 44, pcmData.Length);
            return wav;
        }
    }

    private static string DetectDominantLanguageCode(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "en";

        int ko = 0;
        int ja = 0;
        int zh = 0;
        int ar = 0;
        int ru = 0;
        int latin = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c >= '\uAC00' && c <= '\uD7A3')
            {
                ko++;
                continue;
            }

            if ((c >= '\u3040' && c <= '\u30FF') || (c >= '\u31F0' && c <= '\u31FF'))
            {
                ja++;
                continue;
            }

            if (c >= '\u4E00' && c <= '\u9FFF')
            {
                zh++;
                continue;
            }

            if (c >= '\u0600' && c <= '\u06FF')
            {
                ar++;
                continue;
            }

            if (c >= '\u0400' && c <= '\u04FF')
            {
                ru++;
                continue;
            }

            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                latin++;
            }
        }

        int max = latin;
        string lang = "en";

        if (ko > max) { max = ko; lang = "ko"; }
        if (ja > max) { max = ja; lang = "ja"; }
        if (zh > max) { max = zh; lang = "zh"; }
        if (ar > max) { max = ar; lang = "ar"; }
        if (ru > max) { max = ru; lang = "ru"; }

        // 스크립트 비율 기준으로 목표 언어를 선택해 혼합 발화에서도 일관된 후처리 방향을 잡습니다.
        return lang;
    }
}


