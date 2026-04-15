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
            // ?ㅼ젣 ?ъ깮 ??대컢? ????먮쫫?먯꽌 寃곗젙?섎?濡? ?ш린?쒕뒗 ?낅쭔 媛蹂띻쾶 ?좎??⑸땲??
        }

    }

    public void PlayTTS(string str)
    {
        // ?꾩옱 湲곕낯 ?쒕굹由ъ삤 諛쒖쓬 ?덉쭏??留욎텛湲??꾪빐 ?쒓뎅?대? 湲곕낯 ?몄뼱濡??〓땲??
        tts.Speak(str, 1, 1, "ko-KR");
    }
}


