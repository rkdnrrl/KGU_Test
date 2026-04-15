using UnityEngine;

// 시나리오 실행기는 이 기본 계약을 통해 구체 타입을 몰라도 핸들러를 호출할 수 있습니다.
public class HandlerHelper : MonoBehaviour
{
    public string HandlerType;

    public virtual void Execute(TableDataItem data)
    {

    } 
    
}



