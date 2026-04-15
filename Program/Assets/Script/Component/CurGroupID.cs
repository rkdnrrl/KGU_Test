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
            // 분기형 시나리오 디버깅 시 현재 그룹을 바로 확인할 수 있어 흐름 추적이 쉬워집니다.
            text.text = item.GetColumnName("GroupID");
        }
        
    }
}


