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
        // ?꾩씠??履??낅젰 ?대깽?몃? 援щ룆??UI/?낅젰 援ы쁽怨??곹샇?묒슜 ?꾨즺 議곌굔??遺꾨━?⑸땲??
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
            // ?ㅼ젙 ?꾨씫 ?곹깭?먯꽌 ?명꽣?숈뀡???곴뎄 ?湲고븯吏 ?딅룄濡?利됱떆 醫낅즺?⑸땲??
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

        // ?숈씪 ID瑜??щ윭 踰??붽뎄?????덉뼱 移댁슫??諛⑹떇(泥???ぉ 1媛??쒓굅)?쇰줈 泥섎━?⑸땲??
        remainingItemIDs.RemoveAt(index);

        if (remainingItemIDs.Count == 0)
            InteractionFinish();
    }
}


