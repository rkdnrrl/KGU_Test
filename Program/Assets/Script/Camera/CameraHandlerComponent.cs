using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraHandlerComponent : MonoBehaviour
{
    public string cameraID;
    private CinemachineCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
    }

    private void OnEnable()
    {
        // 카메라 컴포넌트가 디스패처보다 먼저 켜질 수 있어서 지연 구독이 필요합니다.
        StartCoroutine(DelaySubscribe(() =>
        {
            CameraHandler.instance.OnCamera += MoveCamera;
        }));
    }

    private void OnDisable()
    {
        if (CameraHandler.instance != null)
            CameraHandler.instance.OnCamera -= MoveCamera;
    }

    private void MoveCamera(TableDataItem data)
    {
        if (data == null || virtualCamera == null)
            return;

        string targetCameraID = data.GetColumnName("CameraID");

        if (this.cameraID == targetCameraID)
        {
            // 타겟 카메라를 고정 우선순위로 올려야 여러 가상 카메라 중 선택이 항상 동일해집니다.
            virtualCamera.Priority = 10;
        }
        else
        {
            // 비대상 카메라 우선순위를 낮춰야 이전 상태가 남아서 잘못된 카메라가 유지되는 문제를 막습니다.
            virtualCamera.Priority = -1;
        }
    }

    private IEnumerator DelaySubscribe(Action action)
    {
        while (CameraHandler.instance == null)
        {
            yield return null;
        }

        action?.Invoke();
    }
}


