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

        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        OnTouch?.Invoke(this);

        PlayerInventory.instance.AddItem(item);

        if(isDestroy)
            Destroy(gameObject);
    }
}


