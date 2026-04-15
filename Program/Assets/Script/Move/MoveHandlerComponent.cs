using System;
using System.Collections;
using UnityEngine;

public class MoveHandlerComponent : MonoBehaviour
{
    public string characterID;

    public void OnEnable()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
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
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
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


