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
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        TableDataItem intData = TableManager.GetValue("Interaction", "InteractionID", data.GetColumnName("Command"));

        OnInteraction?.Invoke(intData);
        Debug.Log("Interaction");
    }

    public void InteractionFinish()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        scenario.GroupAction();
    }
}


