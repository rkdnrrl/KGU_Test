using UnityEngine;
using UnityEngine.UI;

public class DialogueComponent : MonoBehaviour
{
    // 타겟 ID를 분리해 같은 UI 컴포넌트로 내레이션/캐릭터 대사를 공통 처리할 수 있게 합니다.
    public string targetID;
    public Text text;

    public void SetText(string str)
    {
        text.text = str;
    }
}


