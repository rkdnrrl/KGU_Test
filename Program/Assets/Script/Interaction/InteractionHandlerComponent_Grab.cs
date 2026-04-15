using UnityEngine;
using System.Collections.Generic;


public class InteractionHandlerComponent_Grab : InteractionHandlerComponent
{
    [SerializeField] private List<string> requiredItemIDs = new List<string>();

    private readonly List<string> remainingItemIDs = new List<string>();

    public List<MonoItem_Interaction> itemList = new List<MonoItem_Interaction>();
    private bool isRunning;

    protected override void OnEnable()
    {
        base.OnEnable();
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        MonoItem_Interaction.OnTouch += HandleItemGrabbed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        MonoItem_Interaction.OnTouch -= HandleItemGrabbed;
    }

    public override void InteractionStart(TableDataItem data)
    {
        base.InteractionStart(data);

        foreach (var item in itemList)
        {
            item.canClick = true;
        }

        remainingItemIDs.Clear();
        isRunning = true;

        for (int i = 0; i < requiredItemIDs.Count; i++)
        {
            string id = requiredItemIDs[i];
            if (!string.IsNullOrWhiteSpace(id))
                remainingItemIDs.Add(id);
        }

        if (remainingItemIDs.Count == 0)
        {
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
            Debug.LogWarning("No required grab items configured. Finishing interaction immediately.");
            InteractionFinish();
        }
    }

    public override void InteractionFinish()
    {
        if (!isRunning)
            return;

        isRunning = false;
        remainingItemIDs.Clear();

        if (InteractionHandler.instance != null)
            InteractionHandler.instance.InteractionFinish();

        gameObject.SetActive(false);
    }

    private void HandleItemGrabbed(MonoItem_Interaction grabbedItem)
    {
        if (!isRunning || grabbedItem == null)
            return;

        if (grabbedItem.item == null)
            return;

        string grabbedID = grabbedItem.item.name;
        if (string.IsNullOrWhiteSpace(grabbedID))
            return;

        int index = remainingItemIDs.FindIndex(id => string.Equals(id, grabbedID, System.StringComparison.OrdinalIgnoreCase));
        if (index < 0)
            return;

        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        remainingItemIDs.RemoveAt(index);

        if (remainingItemIDs.Count == 0)
            InteractionFinish();
    }
}


