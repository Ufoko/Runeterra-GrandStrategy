using UnityEngine;

public class DynamicCanvas : MonoBehaviour
{
    public static DynamicCanvas singleton;

    private void Awake()
    {
        singleton = this;
    }
}
