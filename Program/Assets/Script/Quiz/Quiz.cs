using UnityEngine;
using System.Linq;

public class Quiz : MonoBehaviour
{
    TableDataItem tdi;
    QuizComponent component;
    public void Init(TableDataItem tdi, QuizComponent component)
    {
        this.tdi = tdi;
        this.component = component;
    }

    private void Start()
    {
        
    }

    public void Button(string num)
    {
        if(GetCorrect() == num)
        {
            // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
            component.Finish();
            Destroy(gameObject);
        }
    }

    public string GetCorrect()
    {
        return GetParams()[0];
    }

    public string[] GetParams()
    {
        // 유지보수 시 의도를 빠르게 파악할 수 있도록 정리한 주석입니다.
        string[] strs = (tdi.GetColumnName("Params") ?? "")
        .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .ToArray();

        return strs;
    }
}


