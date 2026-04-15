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
        // ???쒖옉 ???몃뱾??珥덇린???쒖꽌媛 怨좎젙???꾨땲?댁꽌, 吏??援щ룆?쇰줈 null ?덉씠?ㅻ? 留됱뒿?덈떎.
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
        // 罹먮┃?곕쭏??而댄룷?뚰듃媛 遺숆린 ?뚮Ц?? ????꾪꽣媛 ?놁쑝硫??ㅻⅨ 罹먮┃???좊땲硫붿씠?섍퉴吏 ?ъ깮?????덉뒿?덈떎.
        if (characterID != data.GetColumnName("CharacterID"))
            return;

        playAnimation_Animancer.SetClip($"{data.GetColumnName("CharacterID")}/{data.GetColumnName("ClipName")}");
        playAnimation_Animancer.PlayAni();
    }

    IEnumerator DelayAction(Action action)
    {
        // ?ㅻ툕?앺듃 ?쒖꽦???쒖꽌媛 諛붾뚯뼱??援щ룆 ?ㅽ뙣 ?놁씠 ?숈옉?섎룄濡??湲고빀?덈떎.
        while (AnimationHandler.instance == null)
        {
            yield return null;
        }


        if (action != null)
            action();
    }
}


