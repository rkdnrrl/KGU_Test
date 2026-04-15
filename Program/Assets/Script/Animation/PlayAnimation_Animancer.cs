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
            // ?고???肄붾뱶瑜?嫄대뱶由ъ? ?딄퀬???붿옄?대꼫媛 ?꾪솚(釉붾젋?? ?ㅼ젙??議곗젙?????덇쾶 ?⑸땲??
            clipTransition = clip.Transition as ClipTransition;
        }
    }
    public override void PlayAni()
    {
        _Animancer.Play(clipTransition);
    }
}


