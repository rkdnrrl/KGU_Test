using System;
using System.Collections;
using UnityEngine;

public class MoveHandlerComponent : MonoBehaviour
{
    public string characterID;

    public void OnEnable()
    {
        // ?대룞 ?몃뱾??珥덇린???쒖꽌媛 ?щ씪??援щ룆 ?꾨씫???녿룄濡?吏???곌껐?⑸땲??
        StartCoroutine(DelayAction(() =>
        {
            MoveHandler.instance.OnMovePlay += Play;
        }));
    }

    public void OnDisable()
    {
        StartCoroutine(DelayAction(() =>
        {
            MoveHandler.instance.OnMovePlay -= Play;
        }));



    }
    public void Play(TableDataItem data)
    {

        if(characterID == data.GetColumnName("Target"))
        {
            // ?ъ씤?몃? ???ㅻ툕?앺듃濡?愿由ы븯硫??묒??먯꽌???대쫫留?諛붽퓭???꾩튂瑜??ы솢?⑺븷 ???덉뒿?덈떎.
            MovePosTransform[] mptList = GameObject.FindObjectsByType<MovePosTransform>();

            foreach (var item in mptList)
            {
                if(item.name == data.GetColumnName("Command"))
                {
                    transform.position = item.transform.position;
                    transform.rotation = item.transform.rotation;
                }
            }
        }
    }

    IEnumerator DelayAction(Action action)
    {
        while (MoveHandler.instance == null)
        {
            yield return null;
        }


        if (action != null)
            action();
    }
}


