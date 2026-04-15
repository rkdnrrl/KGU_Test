using UnityEngine;

public class DialogueHandler : HandlerHelper
{
    public override void Execute(TableDataItem data)
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        TableDataItem tdi = TableManager.GetValue("Text", "TextID", data.GetColumnName("Command"));

        if (tdi == null)
            return;

        string str = tdi.GetColumnName("Content");

        DialogueComponent[] dialogues = FindObjectsByType<DialogueComponent>();

        foreach (var item in dialogues)
        {
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
            if (data.GetColumnName("Target") == item.targetID)
            {
                item.SetText(str);
            }
        }

        if(data.GetColumnName("Type") == "Dialogue")
        {
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
            if (data.GetColumnName("Target") == "Narration" || data.GetColumnName("Target") == "OldMan")
            {
                AudioHandler.instance.PlayTTS(str);
            }
        }
    }
}


