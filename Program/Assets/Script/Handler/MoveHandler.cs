using System;
using UnityEngine;

public class MoveHandler : HandlerHelper
{
    public static MoveHandler instance;
    public event Action<TableDataItem> OnMovePlay;

    private void Awake()
    {
        instance = this;
    }

    public override void Execute(TableDataItem data)
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        OnMovePlay?.Invoke(data);
    }
}


