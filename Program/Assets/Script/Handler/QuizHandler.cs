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
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        OnActionPlay?.Invoke(data, this);

    }

    public void FinishQuiz()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        Scenario scenario = FindAnyObjectByType<Scenario>();
        if(scenario != null)
        {
            scenario.GroupAction();
        }
    }
}


