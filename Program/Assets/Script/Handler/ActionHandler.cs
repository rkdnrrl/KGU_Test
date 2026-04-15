using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionHandler : HandlerHelper
{
    public static ActionHandler instance;
    public event Action<TableDataItem> OnActionPlay;

    private void Awake()
    {
        instance = this;
    }

    public override void Execute(TableDataItem data)
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        OnActionPlay?.Invoke(data);

    }
}


