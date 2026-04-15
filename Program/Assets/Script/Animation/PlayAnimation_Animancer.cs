using Animancer;
using UnityEngine;

public class PlayAnimation_Animancer : PlayAnimation_Default
{
    [SerializeField] private AnimancerComponent _Animancer;
    private ClipTransition clipTransition;

    public void SetClip(string name)
    {
        TransitionAsset clip = Resources.Load<TransitionAsset>($"Animations/{name}");

        if (clip != null)
        {
            // 런타임 코드를 건드리지 않고도 디자이너가 전환(블렌드) 설정을 조정할 수 있게 합니다.
            clipTransition = clip.Transition as ClipTransition;
        }
    }
    public override void PlayAni()
    {
        _Animancer.Play(clipTransition);
    }
}


