using System;
using UnityEngine;

public class CameraHandler : HandlerHelper
{
    public static CameraHandler instance;
    public event Action<TableDataItem> OnCamera;

    private void Awake()
    {
        instance = this;
    }

    public override void Execute(TableDataItem data)
    {
        if (data == null)
            return;

        // 시나리오는 명령 ID만 참조하고 실제 카메라 데이터는 분리해, 씬 오브젝트 변경의 영향을 줄입니다.
        TableDataItem cameraData = TableManager.GetValue("Camera", "CameraID", data.GetColumnName("Command"));

        if (cameraData == null)
        {
            Debug.LogWarning($"Camera data not found: {data.GetColumnName("Command")}");
            return;
        }

        OnCamera?.Invoke(cameraData);
        Debug.Log($"Camera move: {cameraData.GetColumnName("CameraID")}");
    }
}
