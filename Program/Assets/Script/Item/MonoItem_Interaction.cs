using UnityEngine;
using System;

public class MonoItem_Interaction : MonoItem
{
    public static event Action<MonoItem_Interaction> OnTouch;
    public bool canClick;

    public bool isDestroy;

    private void OnMouseDown()
    {
        if (!canClick)
            return;

        // ?꾩씠???ㅻ툕?앺듃媛 吏곸젒 ?쒕굹由ъ삤 ?곹깭瑜??뚯? ?딅룄濡??대깽?몃줈留??곗튂 ?ъ떎???꾨떖?⑸땲??
        OnTouch?.Invoke(this);

        PlayerInventory.instance.AddItem(item);

        if(isDestroy)
            Destroy(gameObject);
    }
}


