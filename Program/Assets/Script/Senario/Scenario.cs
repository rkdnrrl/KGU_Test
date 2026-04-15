using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// 데이터 기반 실행기를 두어 콘텐츠 변경 시 코드 수정량을 줄입니다.
public class Scenario : MonoBehaviour
{
    // 타입별 핸들러를 캐시해 매 스텝마다 탐색 비용을 줄이고 실행 경로를 단순화합니다.
    private HandlerHelper[] Helpers;

    // 현재 그룹을 저장해야 NextGroupID로 다음 흐름을 안정적으로 이어갈 수 있습니다.
    TableDataItem curGroup;

    Coroutine codelay;

    public string startGroup = "OLDMAN_10";
    private void Start()
    {
        Helpers = GetComponentsInChildren<HandlerHelper>();
    }

    public TableDataItem GetCurGroup()
    {
        return curGroup;
    }

    // 같은 그룹 액션을 묶어 실행해야 연출 타이밍을 데이터로 제어할 수 있습니다.
    public void GroupAction()
    {

        List<TableDataItem> actionList = null;

        if(curGroup == null)
            actionList = TableManager.GetValueList("Action", "GroupID", startGroup);
        else
            actionList = TableManager.GetValueList("Action", "GroupID", curGroup.GetColumnName("NextGroupID"));

        bool needInteract = false;
        float delay = 0f;

        foreach (var item in actionList)
        {
            curGroup = TableManager.GetValue("Group", "GroupID", item.GetColumnName("GroupID"));
            TableDataItem tdi = TableManager.GetValue("Action", "RowID", item.GetColumnName("RowID"));
            Action(tdi);

            if (tdi.GetColumnName("Type") == "Interaction" && tdi.GetColumnName("Type") == "Quiz")
                needInteract = true;

            float delayTime = float.Parse(tdi.GetColumnName("Duration"));

            if (delayTime > 0)
                delay = delayTime;

        }

        if (codelay != null)
            StopCoroutine(codelay);

        if (!needInteract && delay > 0)
            codelay = StartCoroutine(Delay(delay));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GroupAction();

    }


    public void Action(TableDataItem tdi)
    {
        foreach (var item in Helpers)
        {
            if (item.HandlerType == tdi.GetColumnName("Type"))
            {
                item.Execute(tdi);
                return;
            }
        }
        
    }

}



