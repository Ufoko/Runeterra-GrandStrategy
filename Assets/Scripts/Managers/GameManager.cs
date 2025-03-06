using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager singleton;

    [SyncVar] private int mapSeed;

    private void Start()
    {
        if(isServer)
        {
            mapSeed = Random.Range(int.MinValue, int.MaxValue);
        }
        GroupedSorter.singleton.GenerateMap(mapSeed);
    }
}
