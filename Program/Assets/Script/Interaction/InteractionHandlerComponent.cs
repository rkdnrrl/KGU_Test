using System;
using System.Collections;
using UnityEngine;

public class InteractionHandlerComponent : MonoBehaviour
{
    public string interactionID;

    private TableDataItem interactionData;
    private Coroutine subscribeCoroutine;
    private bool isSubscribed;

    protected virtual void OnEnable()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        subscribeCoroutine = StartCoroutine(DelaySubscribe());
    }

    protected virtual void OnDisable()
    {
        if (subscribeCoroutine != null)
        {
            StopCoroutine(subscribeCoroutine);
            subscribeCoroutine = null;
        }

        Unsubscribe();
    }

    private IEnumerator DelaySubscribe()
    {
        while (InteractionHandler.instance == null)
            yield return null;

        Subscribe();
    }

    private void Subscribe()
    {
        if (isSubscribed || InteractionHandler.instance == null)
            return;

        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        InteractionHandler.instance.OnInteraction += HandleInteractionEvent;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || InteractionHandler.instance == null)
            return;

        InteractionHandler.instance.OnInteraction -= HandleInteractionEvent;
        isSubscribed = false;
    }

    private void HandleInteractionEvent(TableDataItem data)
    {
        if (data == null)
            return;

        string targetID = data.GetColumnName("TargetID");

        if (!string.IsNullOrEmpty(interactionID) &&
            !string.Equals(interactionID, targetID, StringComparison.OrdinalIgnoreCase))
        {
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
            return;
        }

        InteractionStart(data);
    }

    public virtual void InteractionStart(TableDataItem data)
    {
        interactionData = data;
    }

    public virtual void InteractionFinish()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        if (InteractionHandler.instance != null)
            InteractionHandler.instance.InteractionFinish();
    }

    protected TableDataItem GetInteractionData()
    {
        return interactionData;
    }
}


