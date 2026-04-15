using UnityEngine;

public class DialogueHandler : HandlerHelper
{
    public override void Execute(TableDataItem data)
    {
        // ???蹂몃Ц??蹂꾨룄 ?뚯씠釉붿뿉 ?먯뼱, ?≪뀡 ?쒗듃瑜?嫄대뱶由ъ? ?딄퀬??臾멸뎄瑜?援먯껜?????덇쾶 ?⑸땲??
        TableDataItem tdi = TableManager.GetValue("Text", "TextID", data.GetColumnName("Command"));

        if (tdi == null)
            return;

        string str = tdi.GetColumnName("Content");

        DialogueComponent[] dialogues = FindObjectsByType<DialogueComponent>();

        foreach (var item in dialogues)
        {
            // 媛숈? ?몃뱾?щ? ?대젅?댁뀡/罹먮┃??UI媛 ?④퍡 ?곌린 ?꾪빐 ?寃?湲곗??쇰줈 遺꾧린?⑸땲??
            if (data.GetColumnName("Target") == item.targetID)
            {
                item.SetText(str);
            }
        }

        if(data.GetColumnName("Type") == "Dialogue")
        {
            // 紐⑤뱺 ?띿뒪?몃? ?쎌뼱踰꾨━??臾몄젣瑜?留됯린 ?꾪빐 ?꾩옱???꾩슂???寃잙쭔 TTS瑜??ㅽ뻾?⑸땲??
            if (data.GetColumnName("Target") == "Narration" || data.GetColumnName("Target") == "OldMan")
            {
                AudioHandler.instance.PlayTTS(str);
            }
        }
    }
}


