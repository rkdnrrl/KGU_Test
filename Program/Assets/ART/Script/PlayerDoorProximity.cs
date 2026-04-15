using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerDoorProximity : MonoBehaviour
{
    [Header("Find Doors")]
    public string doorTag = "Door";
    public float radius = 1.2f;
    public LayerMask doorLayerMask = ~0; // 기본: 전부

    [Header("Click To Open")]
    public bool requireProximityToClick = true; // 닿아있을 때만 클릭 허용

    private DoorInteractable _current;

    private void Update()
    {
        DoorInteractable nearest = FindNearestDoor();

        if (nearest != _current)
        {
            if (_current != null) _current.SetHighlighted(false);
            _current = nearest;
            if (_current != null) _current.SetHighlighted(true);
        }

        if (WasClickThisFrame())
        {
            if (!requireProximityToClick || _current != null)
            {
                if (_current != null)
                {
                    _current.Toggle();
                }
            }
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

    private DoorInteractable FindNearestDoor()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, doorLayerMask, QueryTriggerInteraction.Collide);
        float bestDist = float.PositiveInfinity;
        DoorInteractable best = null;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider c = hits[i];
            if (c == null) continue;

            GameObject go = c.gameObject;
            if (!go.CompareTag(doorTag) && (go.transform.parent == null || !go.transform.parent.CompareTag(doorTag)))
                continue;

            // 문에는 DoorInteractable을 "직접 붙여서" 관리합니다.
            DoorInteractable door = go.GetComponentInParent<DoorInteractable>();
            if (door == null)
                continue;

            float d = Vector3.Distance(transform.position, door.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = door;
            }
        }

        return best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}

