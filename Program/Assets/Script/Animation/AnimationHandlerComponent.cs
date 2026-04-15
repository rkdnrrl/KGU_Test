using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AnimationHandlerComponent : MonoBehaviour
{
    public string characterID;
    public PlayAnimation_Animancer playAnimation_Animancer;

    public void OnEnable()
    {
        // 씬 시작 시 핸들러 초기화 순서가 고정이 아니어서, 지연 구독으로 null 레이스를 줄입니다.
        StartCoroutine(DelayAction(() =>
        {
            AnimationHandler.instance.OnAnimationPlay += Play;
        }));
    }

    public void OnDisable()
    {
        StartCoroutine(DelayAction(() =>
        {
            AnimationHandler.instance.OnAnimationPlay -= Play;
        }));

       

    }
    public void Play(TableDataItem data)
    {
        // 캐릭터마다 컴포넌트가 붙기 때문에, 대상 필터가 없으면 다른 캐릭터 애니메이션까지 재생될 수 있습니다.
        if (characterID != data.GetColumnName("CharacterID"))
            return;

        playAnimation_Animancer.SetClip($"{data.GetColumnName("CharacterID")}/{data.GetColumnName("ClipName")}");
        playAnimation_Animancer.PlayAni();
    }

    IEnumerator DelayAction(Action action)
    {
        // 오브젝트 활성화 순서가 바뀌어도 구독 실패 없이 동작하도록 대기합니다.
        while (AnimationHandler.instance == null)
        {
            yield return null;
        }


        if (action != null)
            action();
    }
}


