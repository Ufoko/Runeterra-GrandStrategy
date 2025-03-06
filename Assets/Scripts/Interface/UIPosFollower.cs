using UnityEngine;
using Mirror;

public class UIPosFollower : NetworkBehaviour
{
    [SyncVar] [System.NonSerialized] public Vector3 destination;

    private void Awake()
    {
        transform.SetParent(DynamicCanvas.singleton.transform);
    }

    private void LateUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(destination);
    }
}
