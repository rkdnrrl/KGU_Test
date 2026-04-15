using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

public class InteractionHandlerComponent_Delay : InteractionHandlerComponent
{
    private Coroutine delayCoroutine;

    public override void InteractionStart(TableDataItem data)
    {
        base.InteractionStart(data);

        float delay = GetDelay(data);

        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);

        delayCoroutine = StartCoroutine(Delay(delay));
    }

    public float GetDelay(TableDataItem data)
    {
        if (data == null)
            return 0f;

        string parsing = data.GetColumnName("Params");
        if (string.IsNullOrWhiteSpace(parsing))
            return 0f;

        string[] tokens = parsing.Split(',');
        foreach (string token in tokens)
        {
            if (string.IsNullOrWhiteSpace(token))
                continue;

            // Params에 Delay_2 같은 토큰과 다른 설정이 함께 들어올 수 있어 키 기반으로 선별합니다.
            string[] pair = token.Trim().Split('_');
            if (pair.Length < 2)
                continue;

            if (!string.Equals(pair[0].Trim(), "Delay", StringComparison.OrdinalIgnoreCase))
                continue;

            string delayText = pair[1].Trim();

            if (float.TryParse(delayText, NumberStyles.Float, CultureInfo.InvariantCulture, out float delay))
                return Mathf.Max(0f, delay);

            if (float.TryParse(delayText, out delay))
                return Mathf.Max(0f, delay);
        }

        return 0f;
    }

    private IEnumerator Delay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        delayCoroutine = null;
        InteractionFinish();
    }

    public override void InteractionFinish()
    {
        // 지연 컴포넌트도 공통 종료 경로를 쓰게 해 시나리오 진행 방식이 분기되지 않게 합니다.
        if (InteractionHandler.instance != null)
            InteractionHandler.instance.InteractionFinish();
    }

    private void OnDisable()
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
    }
}


