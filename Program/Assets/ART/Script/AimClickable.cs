using UnityEngine;
using UnityEngine.Events;

public class AimClickable : MonoBehaviour
{
    [Header("Optional")]
    public string hintText = "";

    [Header("Events")]
    public UnityEvent onClick;

    public void Interact()
    {
        onClick?.Invoke();
    }
}

