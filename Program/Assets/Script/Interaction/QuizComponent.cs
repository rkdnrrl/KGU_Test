using System;
using System.Collections;
using UnityEngine;

public class QuizComponent : MonoBehaviour
{
    public Transform parent;
    QuizHandler handler;

    public void OnEnable()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
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
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
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
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        handler.FinishQuiz();
    }
}


