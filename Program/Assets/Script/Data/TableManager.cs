using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TableManager : MonoBehaviour
{
    public static TableManager instance;
    public static Dictionary<string, TableData> tableDatas = new Dictionary<string, TableData>();
    void Awake()
    {
        instance = this;
        TableDataComponent[] tdcs= GetComponentsInChildren<TableDataComponent>();

        foreach (var item in tdcs)
        {
            AddTable(item.tableName);
        }

        // 시작 시 등록된 테이블을 미리 적재해 런타임 조회 지연을 줄입니다.
    }

    public static void AddTable(string tableName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Data/{tableName}");

        if (jsonFile == null)
        {
            Debug.LogError($"{tableName}.json 파일을 찾을 수 없음");
            return;
        }

        // 테이블 이름을 키로 캐싱해 모든 핸들러가 같은 데이터 인스턴스를 참조하게 합니다.
        TableData td = new TableData();
        tableDatas[tableName] = td;
        td.Parsing(jsonFile.text, tableName);
    }

    public static TableDataItem GetValue(string tableName, string header, string id)
    {
        return tableDatas[tableName].GetValueRow(header, id);
    }

    public static List<TableDataItem> GetValueList(string tableName, string header, string id)
    {
        return tableDatas[tableName].GetValueRowList(header, id);
    }
}

public class TableData
{
    public string tableName;
    public List<TableDataItem> data = new List<TableDataItem>();

    public void Parsing(string json, string name = "")
    {
        tableName = name;
        data.Clear();

        JObject root = JObject.Parse(json);
        JArray items = root["items"] as JArray;

        if (items == null || items.Count == 0)
            return;

        JObject firstItem = items[0] as JObject;
        if (firstItem == null)
            return;

        // 헤더를 고정해두면 컬럼 순서가 바뀌어도 이름 기반 조회가 유지됩니다.
        List<string> headerList = new List<string>();
        foreach (JProperty property in firstItem.Properties())
        {
            headerList.Add(property.Name);
        }

        string[] headers = headerList.ToArray();

        // data[0]을 메타(헤더) 슬롯으로 써서 별도 구조 없이 인덱스 계산을 단순화합니다.
        TableDataItem headerItem = new TableDataItem();
        headerItem.headers = headers;
        headerItem.rows = headers;
        data.Add(headerItem);

        // 각 행이 같은 헤더 배열을 공유하도록 해 파싱 이후 조회 비용을 낮춥니다.
        foreach (JObject item in items)
        {
            TableDataItem rowItem = new TableDataItem();
            rowItem.headers = headers;
            rowItem.rows = new string[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                string key = headers[i];
                JToken token = item[key];
                rowItem.rows[i] = token != null ? token.ToString() : "";
            }

            data.Add(rowItem);
        }
    }

    public int GetHeaderIndex(string headerName)
    {
        if (data == null || data.Count == 0 || data[0].rows == null)
            return -1;

        for (int i = 0; i < data[0].rows.Length; i++)
        {
            if (data[0].rows[i] == headerName)
                return i;
        }

        return -1;
    }

    public string GetValue(int rowIndex, string headerName)
    {
        int col = GetHeaderIndex(headerName);

        if (col < 0)
            return "";

        // 외부에서 보는 rowIndex와 내부 저장 인덱스를 분리해 API 사용성을 유지합니다.
        int realRowIndex = rowIndex + 1;

        if (realRowIndex < 0 || realRowIndex >= data.Count)
            return "";

        return data[realRowIndex].Get(col);
    }

    public TableDataItem GetValueRow(string headerName, string id)
    {
        int col = GetHeaderIndex(headerName);

        if (col < 0)
            return null;

        for (int i = 0; i < data.Count; i++)
        {
            string str = data[i].Get(col);
            if (str == id)
            {
                return data[i];
            }

        }

        return null;
    }

    public List<TableDataItem> GetValueRowList(string headerName, string id)
    {
        List<TableDataItem> datas = new List<TableDataItem>();

        int col = GetHeaderIndex(headerName);

        if (col < 0)
            return null;

        for (int i = 0; i < data.Count; i++)
        {
            string str = data[i].Get(col);
            if (str == id)
            {
                // 다건 조회는 그룹 액션처럼 동일 키를 여러 행에서 쓰는 케이스를 지원합니다.
                datas.Add(data[i]);
            }

        }

        return datas;
    }

    public string GetValue(string headerName, string id)
    {
        int col = GetHeaderIndex(headerName);

        if (col < 0)
            return "";

        for (int i = 0; i < data.Count; i++)
        {
            string str = data[i].Get(col);
            if(str == id)
            {
                return str;
            }
            
        }

        return "";
    }
}

[Serializable]
public class TableDataItem
{
    public string[] headers;
    public string[] rows;

    public string Get(int index)
    {
        if (rows == null || index < 0 || index >= rows.Length)
            return "";
        return rows[index];
    }

    public string GetColumnName(string header)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            string cur = headers[i];

            if (cur == header)
                return rows[i];
        }

        return "";
    }
}

