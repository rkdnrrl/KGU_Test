using System;
using System.Collections;
using UnityEngine;

public class QuizComponent : MonoBehaviour
{
    public Transform parent;
    QuizHandler handler;

    public void OnEnable()
    {
        // ?댁쫰 ?몃뱾???앹꽦 ?쒖꽌? 臾닿??섍쾶 ?곌껐?섎룄濡?吏??援щ룆?⑸땲??
        StartCoroutine(DelayAction(() =>
        {
            QuizHandler.instance.OnActionPlay += Play;
        }));
    }

    public void OnDisable()
    {
        StartCoroutine(DelayAction(() =>
        {
            QuizHandler.instance.OnActionPlay -= Play;
        }));



    }

    public void Play(TableDataItem data, QuizHandler handler)
    {
        this.handler = handler;
        // ?댁쫰 ?꾨━?밴낵 ?곗씠??ID瑜?遺꾨━??UI 援먯껜 ???≪뀡 ?쒗듃 ?섏젙 踰붿쐞瑜?以꾩엯?덈떎.
        TableDataItem quizData = TableManager.GetValue("Quiz", " QuizID", data.GetColumnName("Command"));

        if (quizData == null)
            return;

        string quizName = data.GetColumnName("Command");

        GameObject go = Resources.Load<GameObject>($"Quiz/{quizName}");

        if (go != null)
        {
            GameObject copyGo = Instantiate(go, parent);

            if (copyGo != null)
            {
                Quiz quiz = copyGo.GetComponent<Quiz>();
                if (quiz != null)
                {
                    quiz.Init(quizData, this);
                }
            }


        }
    }

    IEnumerator DelayAction(Action action)
    {
        while (QuizHandler.instance == null)
        {
            yield return null;
        }


        if (action != null)
            action();
    }

    public void Finish()
    {
        // ?ㅼ젣 ?쒕굹由ъ삤 吏꾪뻾 ?좏샇???몃뱾?щ? ?듯빐 ?щ젮 怨듯넻 ?꾨즺 ?먮쫫???좎??⑸땲??
        handler.FinishQuiz();
    }
}


