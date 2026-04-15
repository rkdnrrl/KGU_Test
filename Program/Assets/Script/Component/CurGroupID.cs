using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurGroupID : MonoBehaviour
{
    Scenario scenario;
    public Text text;

    private void Start()
    {
        scenario = FindAnyObjectByType<Scenario>();
    }
    public void GetCurGroupID()
    {
        TableDataItem item = scenario.GetCurGroup();

        if(item != null)
        {
            // 遺꾧린???쒕굹由ъ삤 ?붾쾭源????꾩옱 洹몃９??諛붾줈 ?뺤씤?????덉뼱 ?먮쫫 異붿쟻???ъ썙吏묐땲??
            text.text = item.GetColumnName("GroupID");
        }
        
    }
}


