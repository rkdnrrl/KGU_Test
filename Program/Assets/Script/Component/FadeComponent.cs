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
        // ?꾨━??蹂?뺣쭏??李몄“瑜??????ｌ? ?딆븘???숈옉?섎룄濡??먮룞 ?곌껐?⑸땲??
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

        // ?④? ?곹깭?먯꽌 ?섏씠?쒕? ?쒖옉????泥??꾨젅??源쒕묀?꾩쓣 以꾩씠湲??꾪빐 癒쇱? ?쒖꽦?뷀빀?덈떎.
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
            // ?щ챸 ?ㅻ쾭?덉씠媛 ?대┃??媛濡쒖콈??臾몄젣瑜?留됯린 ?꾪빐 ?섏씠?쒖씤 ?꾨즺 ??鍮꾪솢?깊솕?⑸땲??
            fadeUI.SetActive(false);

        fadeCoroutine = null;
    }

    public void OnEnable()
    {
        // ?고????앹꽦 ?쒖꽌 李⑥씠濡??명븳 援щ룆 ?덉씠?ㅻ? 以꾩씠湲??꾪빐 吏??援щ룆?⑸땲??
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
        // ?쒕굹由ъ삤 ?쒗듃??紐낅졊 臾몄옄?대쭔 諛붽퓭???곗텧 ?쒖꽌瑜?議곗젙?????덈룄濡?臾몄옄??湲곕컲?쇰줈 遺꾧린?⑸땲??
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


