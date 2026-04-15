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
        // 移대찓??而댄룷?뚰듃媛 ?붿뒪?⑥쿂蹂대떎 癒쇱? 耳쒖쭏 ???덉뼱??吏??援щ룆???꾩슂?⑸땲??
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
            // ?寃?移대찓?쇰? 怨좎젙 ?곗꽑?쒖쐞濡??щ젮???щ윭 媛??移대찓??以??좏깮????긽 ?숈씪?댁쭛?덈떎.
            virtualCamera.Priority = 10;
        }
        else
        {
            // 鍮꾨???移대찓???곗꽑?쒖쐞瑜???떠???댁쟾 ?곹깭媛 ?⑥븘???섎せ??移대찓?쇨? ?좎??섎뒗 臾몄젣瑜?留됱뒿?덈떎.
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


