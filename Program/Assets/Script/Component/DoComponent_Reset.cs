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
                // 디자이너가 트리거로 리셋을 호출할 때도 항상 같은 초기화 결과를 보장하기 위해서입니다.
                item.gameObject.SetActive(false);
            }
        }
    }
}


