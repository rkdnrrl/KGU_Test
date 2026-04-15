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
            // ?뺣떟 泥섎━ 吏곹썑 利됱떆 ?뚭눼??以묐났 ?대┃?쇰줈 ?꾨즺 ?대깽?멸? 諛섎났 ?몄텧?섎뒗 ?곹솴??留됱뒿?덈떎.
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
        // Params瑜?怨듬갚/鍮?媛??뺣━源뚯? ??踰덉뿉 泥섎━???쒗듃 ?щ㎎ ?ㅼ감????誘쇨컧?섍쾶 留뚮벊?덈떎.
        string[] strs = (tdi.GetColumnName("Params") ?? "")
        .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .ToArray();

        return strs;
    }
}


