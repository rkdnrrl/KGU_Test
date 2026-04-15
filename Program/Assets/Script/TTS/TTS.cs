using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

namespace UnityCoder.Samples
{
    public class TTS : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void readTextAloud(string text, float rate, float pitch, string lang, string objName, string methodName);

        [DllImport("__Internal")]
        private static extern string GetVoices();

        [DllImport("__Internal")]
        private static extern void stopTextAloud();
#endif

        private float pitch = 1f;
        private float rate = 1f;

        public bool isSpeaking;

        public void Speak(string text, float rate = 1.0f, float pitch = 1.0f, string lang = "en-US")
        {
            // 이전 발화를 끊고 새 발화를 시작해 대사 겹침으로 인한 전달력 저하를 막습니다.
            StopSpeak();
            isSpeaking = true;

#if UNITY_WEBGL && !UNITY_EDITOR
            readTextAloud(text, rate, pitch, lang, gameObject.name, nameof(OnTTSFinished));
#else
            Debug.Log($"[TTS Editor/Non-WebGL] {text} / rate:{rate} / pitch:{pitch} / lang:{lang}");
#endif
        }

        public void SetPitch(float pitch)
        {
            this.pitch = pitch;
        }

        public void SetRate(float rate)
        {
            this.rate = rate;
        }

        public void Tempo()
        {
            Speak("안녕하세요. 테스트입니다.", rate, pitch, "ko-KR");
        }

        public void ReadInputField(TMP_InputField source)
        {
            var msg = source.text;
            Debug.Log(msg);
            Speak(msg, rate, pitch, "ko-KR");
        }

        public void OnTTSFinished(string result)
        {
            // 완료 플래그를 명시적으로 내리지 않으면 상위 로직이 다음 발화를 막아버릴 수 있습니다.
            isSpeaking = false;
            Debug.Log("TTS 상태: " + result);
        }

        public void StopSpeak()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            stopTextAloud();
#endif
            isSpeaking = false;
        }

        public void LogVoices()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var voices = GetVoices();
            Debug.Log(voices);
#else
            Debug.Log("GetVoices는 WebGL 빌드에서만 동작");
#endif
        }
    }
}

