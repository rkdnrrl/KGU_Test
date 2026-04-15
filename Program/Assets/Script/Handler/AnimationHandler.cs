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
        // ?쒕굹由ъ삤 ?됱뿉?쒕뒗 ?ㅻ쭔 ?곌퀬 ?ㅼ젣 ?좊땲硫붿씠??留ㅽ븨? 蹂꾨룄 ?뚯씠釉붿뿉??愿由ы빐 ?ъ궗?⑹꽦???믪엯?덈떎.
        TableDataItem aniData = TableManager.GetValue("Animation", "AnimID", data.GetColumnName("Command"));

        OnAnimationPlay?.Invoke(aniData);
        Debug.Log("animation");
    }
}


