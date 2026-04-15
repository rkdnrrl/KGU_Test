using UnityEngine;
using UnityEngine.Rendering;

public class SetRenderQueue : MonoBehaviour
{
    void Start()
    {
        // 렌더러 컴포넌트를 가져옵니다.
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 재질의 렌더 큐를 2500으로 강제로 변경합니다.
            // 2500은 알파 테스트(Cutout)와 투명(Transparent) 큐 사이의 값입니다.
            renderer.material.renderQueue = 999;
        }
    }
}