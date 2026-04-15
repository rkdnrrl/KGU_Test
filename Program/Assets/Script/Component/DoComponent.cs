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

        // 단순 지연 실행을 공용화해서 타임라인성 연출을 별도 스크립트 없이 재사용할 수 있습니다.
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
                // 그룹 전환 시 자식 오브젝트 상태가 남아 다음 연출에 섞이는 것을 막기 위해 비활성화합니다.
                item.gameObject.SetActive(false);
            }
        }
    }
}


