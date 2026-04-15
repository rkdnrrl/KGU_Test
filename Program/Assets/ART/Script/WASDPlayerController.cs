using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// WASD로 걷는 "플레이어 이동" 스크립트입니다.
// 사용법: 이 스크립트를 플레이어(GameObject)에 붙이고, 그 오브젝트에 CharacterController를 추가하면 됩니다.
public class WASDPlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 3.0f;

    [Header("Look")]
    public bool enableMouseLook = true;
    public float mouseSensitivity = 2.0f;
    public Transform cameraTransform; // 비어 있으면, 자식 카메라를 자동으로 찾습니다.
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController _controller;
    private float _verticalVelocity;
    private float _pitch;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            _controller = gameObject.AddComponent<CharacterController>();
        }

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraTransform = cam.transform;
            }
        }
    }

    private void Start()
    {
        // 마우스로 시점 회전을 쓰고 싶으면, 게임 화면 클릭 시 마우스가 고정됩니다(ESC로 풀림).
        if (enableMouseLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        HandleLook();
        HandleMove();
    }

    private void HandleLook()
    {
        if (!enableMouseLook || cameraTransform == null)
            return;

#if ENABLE_INPUT_SYSTEM
        // 새 입력 방식(Input System)
        Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
        // old Input.GetAxis("Mouse X") 느낌을 내기 위해 delta에 dt를 곱해줍니다.
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;
#else
        // 옛 입력 방식(레거시 Input Manager)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
#endif

        // 좌우: 플레이어(몸) 회전
        transform.Rotate(Vector3.up * mouseX);

        // 상하: 카메라만 회전
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        cameraTransform.localEulerAngles = new Vector3(_pitch, 0f, 0f);

        // ESC를 누르면 마우스 고정 해제
#if ENABLE_INPUT_SYSTEM
        bool escDown = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        bool escDown = Input.GetKeyDown(KeyCode.Escape);
#endif
        if (escDown)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 마우스 좌클릭하면 다시 고정
#if ENABLE_INPUT_SYSTEM
        bool leftMouseDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        bool leftMouseDown = Input.GetMouseButtonDown(0);
#endif
        if (leftMouseDown)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleMove()
    {
#if ENABLE_INPUT_SYSTEM
        float inputX = 0f;
        float inputZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) inputX -= 1f;
            if (Keyboard.current.dKey.isPressed) inputX += 1f;
            if (Keyboard.current.sKey.isPressed) inputZ -= 1f;
            if (Keyboard.current.wKey.isPressed) inputZ += 1f;
        }
#else
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D
        float inputZ = Input.GetAxisRaw("Vertical");   // W/S
#endif

        Vector3 move = (transform.right * inputX + transform.forward * inputZ).normalized;
        Vector3 horizontal = move * moveSpeed;

        // 바닥에 붙어있으면, 아래로 살짝 눌러서 "떠있는" 문제를 막습니다.
        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = new Vector3(horizontal.x, _verticalVelocity, horizontal.z);
        _controller.Move(velocity * Time.deltaTime);
    }
}
