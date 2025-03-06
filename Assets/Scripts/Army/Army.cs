using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Army : NetworkBehaviour
{
    public static List<Army> armies = new List<Army>();
    public List<Entity> entities = new List<Entity>();
    public Player owner;
    public ArmyMovement movement;
    public GameObject cameraIcon;
    public AnimationScript animScript;

    private int numOfMinions;

    [System.NonSerialized] public bool isDead;
    [System.NonSerialized] public bool isSelected;

    private void Awake()
    {
        armies.Add(this);
    }
    void Update() {
        //numOfMinions = mins.minions.Count;
    }

    private void OnMouseDown()
    {
        if(!Player.local)
            return;
        if(!hasAuthority)
            return;
        /*if(EventSystem.current.IsPointerOverGameObject())
            return;*/

        Player.local.commanding.SelectArmy(this);
    }

    /*public void GeneralDie() {
       entities[0].general.gameObject.SetActive(false);
       animScript.CmdSetDyingAnim(true);
       Invoke(nameof(Respawn),3f);
    }*/

    [TargetRpc]
    public void TargetKillEntity(NetworkConnection conn)
    {
        Invoke(nameof(Respawn), 7f);
    }

    [Client]
    void Respawn() {
        transform.position = Minions.singleton.armyPos;
        entities[0].general.gameObject.SetActive(true);
        animScript.CmdSetIdleAnim(true);
        entities[0].audioSource.PlayOneShot(entities[0].respawnAudio);
        movement.AllowMovement(true);
        movement.ClearPath();

        CmdRespawn();
    }

    [Command]
    private void CmdRespawn()
    {
        isDead = false;
        GeneralScript general = GetComponentInChildren<GeneralScript>();
        general.entity.stats.currentHealth = general.entity.stats.maxHealth;
        RpcUpdateHealthbar(general.entity.stats.currentHealth);
    }

    [ClientRpc]
    public void RpcUpdateHealthbar(float health)
    {
        GeneralScript general = GetComponentInChildren<GeneralScript>();
        if(!general) // Minions dont have healthbars
            return;
        general.healthBar.SetHealth(health);
    }

    [Server]
    public void ServerKillEntity(int entityIndex)
    {
        //Entity entity = entities[entityIndex];
        /*if(!entity.general)
        {
            entities.RemoveAt(entityIndex);
        }*/
        RpcKillEntity(entityIndex);
        //Destroy(entity);
    }

    // For minions
    [ClientRpc]
    public void RpcKillEntity(int entityIndex)
    {
        if(entities.Count == 0)
            return;
        Entity entity = entities[0];
        /*if(entity.anim)
            entity.anim.SetBool("IsDead", true);
        if(entity.dieAudio)
        {
            entity.audioSource.PlayOneShot(entity.dieAudio);
            entity.audioSource.volume = animScript.volume;
        }*/

        entities.RemoveAt(0);
        Destroy(entity.gameObject);
    }
    // For generals
    

    
}
