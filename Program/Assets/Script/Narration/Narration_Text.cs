using UnityEngine;
using UnityEngine.UI;

public class Narration_Text : MonoBehaviour
{
    // 나레이션 텍스트 갱신을 전용 컴포넌트로 분리해 다른 UI와 업데이트 책임을 분리합니다.
    public Text narrationText;

    public void SetText(string text)
    {
        narrationText.text = text;
    }
}


