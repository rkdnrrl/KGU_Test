using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AnimationHandler : HandlerHelper
{
    public static AnimationHandler instance;
    public event Action<TableDataItem> OnAnimationPlay;

    private void Awake()
    {
        instance = this;
    }

    public override void Execute(TableDataItem data)
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        TableDataItem aniData = TableManager.GetValue("Animation", "AnimID", data.GetColumnName("Command"));

        OnAnimationPlay?.Invoke(aniData);
        Debug.Log("animation");
    }
}


