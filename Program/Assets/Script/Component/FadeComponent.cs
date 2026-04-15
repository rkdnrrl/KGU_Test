using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeComponent : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject fadeUI;
    [SerializeField] private Image fadeImage;

    [Header("Fade")]
    [SerializeField] private float defaultDuration = 0.5f;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool hideFadeUIOnFadeInComplete = true;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // 프리팹 변형마다 참조를 둘 다 넣지 않아도 동작하도록 자동 연결합니다.
        if (fadeImage == null && fadeUI != null)
            fadeImage = fadeUI.GetComponent<Image>();

        if (fadeUI == null && fadeImage != null)
            fadeUI = fadeImage.gameObject;
    }

    public void FadeIn()
    {
        StartFade(0f, defaultDuration, hideFadeUIOnFadeInComplete);
    }

    public void FadeOut()
    {
        if (fadeUI != null)
            fadeUI.SetActive(true);

        StartFade(1f, defaultDuration, false);
    }

    public void FadeIn(float duration)
    {
        StartFade(0f, duration, hideFadeUIOnFadeInComplete);
    }

    public void FadeOut(float duration)
    {
        if (fadeUI != null)
            fadeUI.SetActive(true);

        StartFade(1f, duration, false);
    }

    public void SetAlphaImmediate(float alpha)
    {
        if (fadeImage == null)
            return;

        Color c = fadeImage.color;
        c.a = Mathf.Clamp01(alpha);
        fadeImage.color = c;
    }

    private void StartFade(float targetAlpha, float duration, bool hideAfterFinish)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("FadeHandler: fadeImage is not assigned.");
            return;
        }

        // 숨김 상태에서 페이드를 시작할 때 첫 프레임 깜빡임을 줄이기 위해 먼저 활성화합니다.
        if (fadeUI != null)
            fadeUI.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration, hideAfterFinish));
    }

    private System.Collections.IEnumerator FadeRoutine(float targetAlpha, float duration, bool hideAfterFinish)
    {
        float safeDuration = Mathf.Max(0.01f, duration);
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < safeDuration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / safeDuration);
            SetAlphaImmediate(Mathf.Lerp(startAlpha, targetAlpha, t));
            yield return null;
        }

        SetAlphaImmediate(targetAlpha);

        if (hideAfterFinish && Mathf.Approximately(targetAlpha, 0f) && fadeUI != null)
            // 투명 오버레이가 클릭을 가로채는 문제를 막기 위해 페이드인 완료 후 비활성화합니다.
            fadeUI.SetActive(false);

        fadeCoroutine = null;
    }

    public void OnEnable()
    {
        // 런타임 생성 순서 차이로 인한 구독 레이스를 줄이기 위해 지연 구독합니다.
        StartCoroutine(DelayAction(() =>
        {
            ActionHandler.instance.OnActionPlay += Play;
        }));
    }

    public void OnDisable()
    {
        StartCoroutine(DelayAction(() =>
        {
            ActionHandler.instance.OnActionPlay -= Play;
        }));



    }
    public void Play(TableDataItem data)
    {
        // 시나리오 시트의 명령 문자열만 바꿔도 연출 순서를 조정할 수 있도록 문자열 기반으로 분기합니다.
        if(data.GetColumnName("Command") == "FadeIn")
        {
            FadeIn();
        }
        else if(data.GetColumnName("Command") == "FadeOut")
        {
            FadeOut();
        }
    }

    IEnumerator DelayAction(Action action)
    {
        while (ActionHandler.instance == null)
        {
            yield return null;
        }


        if (action != null)
            action();
    }
}


