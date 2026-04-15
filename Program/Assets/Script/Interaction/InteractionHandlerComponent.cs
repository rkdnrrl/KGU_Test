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
        // ?몃뱾???깃????앹꽦 ?쒖꽌 ?곹뼢??諛쏆? ?딅룄濡?肄붾（?댁뿉??援щ룆?⑸땲??
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

        // 以묐났 援щ룆??留됱븘 媛숈? ?명꽣?숈뀡????踰??ㅽ뻾?섎뒗 臾몄젣瑜??덈갑?⑸땲??
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
            // 媛쒕퀎 而댄룷?뚰듃媛 ?먯떊??TargetID留?泥섎━?섍쾶 ?댁꽌 ?????ㅼ쨷 ?명꽣?숈뀡 異⑸룎??留됱뒿?덈떎.
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
        // 醫낅즺 ?좏샇瑜?InteractionHandler濡?紐⑥븘??洹몃９ 吏꾪뻾 洹쒖튃????怨녹뿉???좎??⑸땲??
        if (InteractionHandler.instance != null)
            InteractionHandler.instance.InteractionFinish();
    }

    protected TableDataItem GetInteractionData()
    {
        return interactionData;
    }
}


