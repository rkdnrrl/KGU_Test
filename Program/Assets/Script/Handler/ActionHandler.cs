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
        // ?≪뀡 ?먮낯 ?됱쓣 洹몃?濡??꾨떖???щ윭 ?쒕툕?쒖뒪?쒖씠 異붽? 議고쉶 ?놁씠 ?숈떆??諛섏쓳?????덇쾶 ?⑸땲??
        OnActionPlay?.Invoke(data);

    }
}


