using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager singleton;

    public Faction[] factions;
    public Transform[] entities;

    public GameObject prefabBattleIndicator;

    private void Awake()
    {
        singleton = this;
    }
}
