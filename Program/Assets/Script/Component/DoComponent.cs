using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DoComponent : MonoBehaviour
{
    public UnityEvent e;
    public bool onAwake;
    public float delay;

    Coroutine co;

    private void OnEnable()
    {
        if (co != null)
            StopCoroutine(co);

        // ?⑥닚 吏???ㅽ뻾??怨듭슜?뷀빐????꾨씪?몄꽦 ?곗텧??蹂꾨룄 ?ㅽ겕由쏀듃 ?놁씠 ?ъ궗?⑺븷 ???덉뒿?덈떎.
        co =StartCoroutine(DoDelay(delay));
    }

    IEnumerator DoDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Do();
    }

    public void Do()
    {
        if(e != null)
        {
            e.Invoke();
        }
    }

    public void ResetComponent()
    {
        foreach (Transform item in transform)
        {
            DoComponent doco = GetComponent<DoComponent>();

            if (doco != null)
            {
                // 洹몃９ ?꾪솚 ???먯떇 ?ㅻ툕?앺듃 ?곹깭媛 ?⑥븘 ?ㅼ쓬 ?곗텧???욎씠??寃껋쓣 留됯린 ?꾪빐 鍮꾪솢?깊솕?⑸땲??
                item.gameObject.SetActive(false);
            }
        }
    }
}


