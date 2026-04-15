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
        // ?대룞? ?≪뀡 ?먮낯 ?곗씠?곕? 洹몃?濡??곕뒗 ?몄씠 醫뚰몴 ?ъ씤???대쫫 愿由ъ뿉 ?좊━?⑸땲??
        OnMovePlay?.Invoke(data);
    }
}


