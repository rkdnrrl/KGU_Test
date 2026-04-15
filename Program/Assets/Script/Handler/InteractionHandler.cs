using System;
using UnityEngine;

public class InteractionHandler : HandlerHelper
{
    public static InteractionHandler instance;
    public event Action<TableDataItem> OnInteraction;
    Scenario scenario;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        scenario = FindAnyObjectByType<Scenario>();
    }

    public override void Execute(TableDataItem data)
    {
        // ?≪뀡 ?쒗듃? ?명꽣?숈뀡 ?곸꽭 ?뚯씠釉붿쓣 遺꾨━??媛숈? ?명꽣?숈뀡???щ윭 ?④퀎?먯꽌 ?ъ궗?⑺븷 ???덇쾶 ?⑸땲??
        TableDataItem intData = TableManager.GetValue("Interaction", "InteractionID", data.GetColumnName("Command"));

        OnInteraction?.Invoke(intData);
        Debug.Log("Interaction");
    }

    public void InteractionFinish()
    {
        // ?명꽣?숈뀡 醫낅즺 ???ㅼ쓬 洹몃９ 吏꾪뻾????怨녹뿉???듭씪???먮쫫 ?꾨씫??以꾩엯?덈떎.
        scenario.GroupAction();
    }
}


