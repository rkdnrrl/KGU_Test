using UnityEngine;
using UnityCoder.Samples;

public class AudioHandler : HandlerHelper
{
    public static AudioHandler instance;
    public TTS tts;

    private void Awake()
    {
        instance = this;
    }
    public override void Execute(TableDataItem data)
    {
        if(data.GetColumnName("Target") == "Narration")
        {
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        }

    }

    public void PlayTTS(string str)
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        tts.Speak(str, 1, 1, "ko-KR");
    }
}


