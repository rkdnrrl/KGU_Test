using UnityEngine;

[System.Serializable]
public class Item
{
    // 데이터 객체를 분리해 인벤토리와 씬 오브젝트 수명을 독립적으로 관리합니다.
    public string id;
    public string name;
}
public class MonoItem : MonoBehaviour
{
    public Item item;
}


