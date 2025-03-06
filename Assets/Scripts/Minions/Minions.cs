using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Minions : NetworkBehaviour
{
    public static Minions singleton;

    [Header("GameObjects")]
    public GameObject cannonMinionPrefab;
    public GameObject SuperMinionPrefab;
    public GameObject armyPrefab;
    public TMP_Text walkMinionsText;
    public GameObject stopPlacingMinionsHUD;
    [System.NonSerialized] public Army army;

    [Header("cost")]
    public int cannonMinionCost = 5;
    int cannonMinionOriginalCost = 5;
    public int superMinionCost = 10;
    int superMinionOriginalCost = 10;
    [System.NonSerialized]public bool minionWalkPhase = false;

    public List<GameObject> minions = new List<GameObject>();
    [System.NonSerialized] public Vector3 armyPos;
    [System.NonSerialized] public Vector3 armyRot;

    [SerializeField] private LayerMask groundLayer = default;

    private void Awake() {
        singleton = this;
    }

    public void ChangeArmyPos() {
        if (Nexus.clickedNexus.isBlueNexus) {
            armyPos = Player.local.myNexus.transform.position + new Vector3(19.7f, 5, -23.2f);
            armyRot = new Vector3(-90, 0, 0);
        } else {
            armyPos = Player.local.myNexus.transform.position + new Vector3(-39.7f, 5, -23.2f);
            armyRot = new Vector3(-90, 180, 0);
        }
    }

    private void Update() {
        if (minionWalkPhase) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer) && Input.GetMouseButtonDown(1)) {
                Debug.DrawLine(ray.origin, hit.point, Color.green, 2f);
                army.movement.destinations.Insert(army.movement.destinations.Count - 1, hit.point);
                army.movement.SetNewDestination(army.movement.destinations[0]);
            }
        }
    }

    private void SpawnArmy(int wantedMinions, int minionType) {
        Player.local.CmdSpawnArmy(wantedMinions, Player.local.netId, Nexus.clickedNexus.netId, armyPos, minionType);

        //Add det der skift kamera noget når en minions dør

        Nexus.clickedNexus.CloseShopHUD();
        walkMinionsText.gameObject.SetActive(true);
        walkMinionsText.text = "Choose a path for the minions";
        stopPlacingMinionsHUD.SetActive(true);
        minionWalkPhase = true;
    }
    [Server]
    public void ServerSpawnArmy(int minionAmount, uint playerId, uint nexusId, Vector3 armyPos, int minionType)
    {
        Player player = NetworkServer.spawned[playerId].GetComponent<Player>();
        NetworkConnection playerConnection = player.connectionToClient;

        GameObject armyPre = Instantiate(armyPrefab, armyPos, Quaternion.identity);
        NetworkServer.Spawn(armyPre, playerConnection);

        Army army = armyPre.GetComponent<Army>();
        army.owner = player;

        if(isServerOnly)
        {
            for(int i = 0; i < minionAmount; ++i)
            {
                GameObject theMinion = Instantiate(minions[minionType], armyPos, Quaternion.Euler(armyRot), army.transform);
                army.entities.Add(theMinion.GetComponent<Entity>());
            }
        }

        RpcSpawnArmy(minionAmount, army.netId, nexusId, armyPos, minionType);
        TargetSpawnArmy(playerConnection, army.netId);
    }
    [ClientRpc]
    private void RpcSpawnArmy(int minionAmount, uint armyId, uint nexusId, Vector3 armyPos, int minionType)
    {
        NetworkIdentity armyIdentity = NetworkClient.spawned[armyId];
        Army army = armyIdentity.GetComponent<Army>();

        float increaseRowsx = 0;
        float increaseRowsz = 0;
        for(int i = 0; i < minionAmount; i++)
        {
            if(i % 3 == 0)
            {
                increaseRowsx += 10f;
            }

            if(increaseRowsz == 36)
            {
                increaseRowsz = 12f;
            }
            else
            {
                increaseRowsz += 12f;
            }
            GameObject theMinion = Instantiate(minions[minionType], armyPos, Quaternion.Euler(armyRot), army.transform);
            theMinion.transform.position = armyPos + new Vector3(increaseRowsz, 6f, increaseRowsx);
            theMinion.GetComponent<MeshRenderer>().material = NetworkClient.spawned[nexusId].GetComponent<Nexus>().minionMaterial;
            army.entities.Add(theMinion.GetComponent<Entity>());
        }
        Debug.Log("New minions " + army.entities.Count);

        army.cameraIcon = army.transform.GetChild(3).GetChild(0).gameObject;
    }
    [TargetRpc]
    private void TargetSpawnArmy(NetworkConnection ownerConnection, uint armyId)
    {
        NetworkIdentity armyIdentity = NetworkClient.spawned[armyId];
        army = armyIdentity.GetComponent<Army>();
        army.movement.destinations.Add(Player.local.enemyNexus.transform.position);
        army.movement.SetNewDestination(army.movement.destinations[0]);
        army.movement.AllowMovement(false);

        Player.local.commanding.SelectArmy(army.GetComponent<Army>());
    }

    public void BuyCannonMinion(int wantedMinions) {
        cannonMinionCost = cannonMinionOriginalCost;
        cannonMinionCost = cannonMinionCost * wantedMinions;

        if (Player.local.gold >= cannonMinionCost) {
            Player.local.gold -= cannonMinionCost;
            /*for (int i = 0; i < wantedMinions; i++) {
                minions.Add(cannonMinionPrefab);
            }*/
            SpawnArmy(wantedMinions, 0);
        }
    }

    public void BuySuperMinion(int wantedMinions) {
        superMinionCost = superMinionOriginalCost;
        superMinionCost = superMinionCost * wantedMinions;

        if (Player.local.gold >= superMinionCost) {
            Player.local.gold -= superMinionCost;
            /*for (int i = 0; i < wantedMinions; i++) {
                minions.Add(SuperMinionPrefab);
            }*/
            SpawnArmy(wantedMinions, 1);
        }
    }

    public void StopMinionWalkPhase() {
        walkMinionsText.gameObject.SetActive(false);
        stopPlacingMinionsHUD.SetActive(false);
        minionWalkPhase = false;
        army.movement.AllowMovement(true);
    }
}
