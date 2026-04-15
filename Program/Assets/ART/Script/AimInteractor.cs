using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AimInteractor : MonoBehaviour
{
    [Header("Ray")]
    public Camera targetCamera;
    public float maxDistance = 5f;
    public LayerMask interactLayerMask = ~0; // 기본: 전부

    [Header("Crosshair")]
    public bool showCrosshair = true;
    public Color crosshairColor = new Color(1f, 1f, 1f, 0.9f);
    public float crosshairSize = 10f;
    public float crosshairThickness = 2f;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (targetCamera == null)
            return;

        if (WasClickThisFrame())
        {
            TryInteract();
        }
    }

    private bool WasClickThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    private void TryInteract()
    {
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactLayerMask, QueryTriggerInteraction.Ignore))
        {
            // 1) AimClickable이 있으면 그걸 먼저 실행
            AimClickable clickable = hit.collider.GetComponentInParent<AimClickable>();
            if (clickable != null)
            {
                clickable.Interact();
                return;
            }

            // 2) 혹시 기존 오브젝트에 OnAimClick() 같은 함수를 만들어둔 경우를 위해 SendMessage 지원
            hit.collider.gameObject.SendMessage("OnAimClick", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnGUI()
    {
        if (!showCrosshair)
            return;

        // 게임 화면 중앙에 간단한 십자선을 그립니다.
        float x = Screen.width * 0.5f;
        float y = Screen.height * 0.5f;

        Color old = GUI.color;
        GUI.color = crosshairColor;

        // 가로 줄
        GUI.DrawTexture(new Rect(x - crosshairSize, y - crosshairThickness * 0.5f, crosshairSize * 2f, crosshairThickness), Texture2D.whiteTexture);
        // 세로 줄
        GUI.DrawTexture(new Rect(x - crosshairThickness * 0.5f, y - crosshairSize, crosshairThickness, crosshairSize * 2f), Texture2D.whiteTexture);

        GUI.color = old;
    }
}

