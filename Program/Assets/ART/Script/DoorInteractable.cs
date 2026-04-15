using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [Header("Open / Close")]
    public float openAngleZ = 90f;
    public float rotateSpeed = 360f; // degrees per second

    [Header("Outline Highlight")]
    public bool enableOutline = true;
    public Color outlineColor = new Color(1f, 0f, 0f, 1f);
    [Range(1f, 1.2f)] public float outlineScale = 1.03f;

    private Quaternion _closedLocalRotation;
    private Quaternion _openLocalRotation;
    private Quaternion _targetLocalRotation;
    private bool _isOpen;

    private DoorOutline _outline;

    private void Awake()
    {
        _closedLocalRotation = transform.localRotation;
        _openLocalRotation = _closedLocalRotation * Quaternion.Euler(0f, 0f, openAngleZ);
        _targetLocalRotation = _closedLocalRotation;

        _outline = GetComponent<DoorOutline>();
        if (_outline == null) _outline = gameObject.AddComponent<DoorOutline>();
        _outline.outlineColor = outlineColor;
        _outline.outlineScale = outlineScale;
        _outline.enableOnStart = false;
    }

    private void Update()
    {
        // 부드럽게 회전
        if (transform.localRotation != _targetLocalRotation)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                _targetLocalRotation,
                rotateSpeed * Time.deltaTime
            );
        }
    }

    public void SetHighlighted(bool on)
    {
        if (!enableOutline) return;
        if (_outline == null) return;

        _outline.outlineScale = outlineScale;
        _outline.SetColor(outlineColor);
        _outline.SetEnabled(on);
    }

    // AimInteractor가 클릭 시 SendMessage("OnAimClick")를 보내도 동작하게 해둡니다.
    public void OnAimClick()
    {
        Toggle();
    }

    public void Toggle()
    {
        if (_isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        _isOpen = true;
        _targetLocalRotation = _openLocalRotation;
    }

    public void Close()
    {
        _isOpen = false;
        _targetLocalRotation = _closedLocalRotation;
    }
}

