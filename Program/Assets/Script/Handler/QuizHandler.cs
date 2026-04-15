using System;
using UnityEngine;

public class QuizHandler : HandlerHelper
{
    public static QuizHandler instance;
    public event Action<TableDataItem, QuizHandler> OnActionPlay;

    private void Awake()
    {
        instance = this;
    }
    public override void Execute(TableDataItem data)
    {
        // ?댁쫰 UI ?앹꽦 梨낆엫? 而댄룷?뚰듃???먭퀬, ?몃뱾?щ뒗 ?몃━嫄??꾨떖留??대떦??寃고빀????땅?덈떎.
        OnActionPlay?.Invoke(data, this);

    }

    public void FinishQuiz()
    {
        // ?댁쫰 ?꾨즺 ?쒕굹由ъ삤 吏꾪뻾??以묒븰 吏꾩엯?먯쑝濡?留욎떠 ?ㅻⅨ ?명꽣?숈뀡怨?媛숈? ?먮쫫???좎??⑸땲??
        Scenario scenario = FindAnyObjectByType<Scenario>();
        if(scenario != null)
        {
            scenario.GroupAction();
        }
    }
}


