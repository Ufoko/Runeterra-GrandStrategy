using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    

    public PlayerCommanding commanding;

    public static GeneralScript general;
    public int gold;

    public GameObject myNexus;
    public GameObject enemyNexus;

    private Items items;

    private void Awake()
    {
        items = FindObjectOfType<Items>();
    }
    
    public static List<Player> players = new List<Player>();
    public static Player local;

    [SyncVar] public int factionId;

    [SerializeField] private GameObject[] armyObjects = null;

    private void Start()
    {
        if(hasAuthority)
        {
            local = this;
        }
        if(isServer)
        {
            GameObject obj = Instantiate(armyObjects[players.Count], ObjectManager.singleton.entities[players.Count].position, ObjectManager.singleton.entities[players.Count].rotation, null);
            NetworkServer.Spawn(obj, gameObject);
            Army army = obj.GetComponent<Army>();
            army.owner = this;

            factionId = players.Count;
            players.Add(this);
            TargetStart(netIdentity.connectionToClient, army.netId, factionId);
        }
    }

    [TargetRpc]
    private void TargetStart(NetworkConnection conn, uint generalObjId, int factionId)
    {
        general = NetworkClient.spawned[generalObjId].GetComponentInChildren<GeneralScript>();
        myNexus = Nexus.nexuses[factionId].gameObject;
        enemyNexus = Nexus.nexuses[1 - factionId].gameObject;
    }

    [Command]
    public void CmdSpawnArmy(int minionAmount, uint playerId, uint nexusId, Vector3 armyPos, int minionType)
    {
        Minions.singleton.ServerSpawnArmy(minionAmount, playerId, nexusId, armyPos, minionType);
    }

    [TargetRpc]
    public void TargetAddGold(int gold, uint playerId)
    {
        AddGold(gold, playerId);
    }
    [TargetRpc]
    public void TargetAddGeneralGold(int gold, uint playerId, uint armyId)
    {
        GeneralScript general = NetworkClient.spawned[armyId].GetComponentInChildren<GeneralScript>();
        general.gold += Mathf.RoundToInt(gold / 3f);
        AddGold(gold, playerId);
    }
    [Client]
    private void AddGold(int gold, uint playerId)
    {
        Player player = NetworkClient.spawned[playerId].GetComponent<Player>();
        player.gold += Mathf.RoundToInt(gold / 3f);
    }

    [Client]
    public void AddStats(LoLItems item)
    {
        CmdAddStats(item, general.GetComponentInParent<Army>().netId);
    }
    [Command]
    private void CmdAddStats(LoLItems item, uint armyId)
    {
        GeneralScript general = NetworkServer.spawned[armyId].GetComponentInChildren<GeneralScript>();
        general.entity.stats.attackDamage += items.GetAttackDamage(item);
        general.entity.stats.maxHealth += items.GetMaxHealth(item);
        general.healthBar.SetMaxHealth(general.entity.stats.maxHealth);
        general.movementSpeed += items.GetMovementSpeed(item);
        general.entity.stats.attackSpeed += items.GetAttackSpeed(item);
    }
}
