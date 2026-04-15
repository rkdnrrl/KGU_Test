using UnityEngine;

public class DoComponent_Reset : MonoBehaviour
{

    public void AllReset()
    {
        foreach (Transform item in transform)
        {
            DoComponent doco = GetComponent<DoComponent>();

            if(doco != null)
            {
                // ?붿옄?대꼫媛 ?몃━嫄곕줈 由ъ뀑???몄텧???뚮룄 ??긽 媛숈? 珥덇린??寃곌낵瑜?蹂댁옣?섍린 ?꾪빐?쒖엯?덈떎.
                item.gameObject.SetActive(false);
            }
        }
    }
}


