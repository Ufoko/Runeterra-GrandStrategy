using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using UnityEngine;
using Mirror;

public class Battle
{
    private BattleIndicator indicator;

    public Player playerAttacker { private set; get; }
    public Player playerDefender { private set; get; }

    public ObservableCollection<Army> attackingArmies = new ObservableCollection<Army>();
    public ObservableCollection<Army> defendingArmies = new ObservableCollection<Army>();

    private TerrainType terrainType;

    public Battle(Player playerAttacker, Player playerDefender, Army initiatingArmy, List<Army> armies, Vector3 position, TerrainType terrainType)
    {
        // Track collections
        attackingArmies.CollectionChanged += OnArmyChange;
        defendingArmies.CollectionChanged += OnArmyChange;

        // Set vars
        this.playerAttacker = playerAttacker;
        this.playerDefender = playerDefender;

        this.terrainType = terrainType;

        // Add all armies to the battle
        attackingArmies.Add(initiatingArmy);
        for(int i = 0; i < armies.Count; ++i)
        {
            if(armies[i].owner == playerAttacker)
                attackingArmies.Add(armies[i]);
            else if(armies[i].owner == playerDefender)
                defendingArmies.Add(armies[i]);
        }

        // Instantiate indicator
        GameObject newIndicatorObj = GameObject.Instantiate(ObjectManager.singleton.prefabBattleIndicator, DynamicCanvas.singleton.transform);
        NetworkServer.Spawn(newIndicatorObj);
        BattleIndicator battleIndicator = newIndicatorObj.GetComponent<BattleIndicator>();
        indicator = battleIndicator;
        CmdSetIndicatorPos(position, battleIndicator.netId);
        indicator.RpcSetAttackerFaction(playerAttacker.factionId);
        indicator.RpcSetDefenderFaction(playerDefender.factionId);

        // Count entities
        int attackerEntityAmount = 0;
        for(int i = 0; i < attackingArmies.Count; ++i)
        {
            attackerEntityAmount += attackingArmies[i].entities.Count;
            indicator.RpcSetAttackerMinions(attackingArmies[i].entities.Count);
        }
        int defenderEntityAmount = 0;
        for(int i = 0; i < defendingArmies.Count; ++i)
        {
            defenderEntityAmount += defendingArmies[i].entities.Count;
            indicator.RpcSetDefenderMinions(defendingArmies[i].entities.Count);
        }

        // Assign entities
        Entity[] attackerEntities = new Entity[attackerEntityAmount];
        int index = 0;
        for(int i = 0; i < attackingArmies.Count; ++i)
        {
            for (int j = 0; j < attackingArmies[i].entities.Count; ++j)
            {
                attackerEntities[index++] = attackingArmies[i].entities[j];
            }
        }
        Entity[] defenderEntities = new Entity[defenderEntityAmount];
        index = 0;
        for(int i = 0; i < defendingArmies.Count; ++i)
        {
            for (int j = 0; j < defendingArmies[i].entities.Count; ++j)
            {
                defenderEntities[index++] = defendingArmies[i].entities[j];
            }
        }
        Debug.Log("Battle started: " + armies.Count + " armies. Defenders: " + defenderEntityAmount + " Attackers: " + attackerEntityAmount);
        indicator.UpdateStrength(attackerEntities, attackerEntities.Length, defenderEntities, defenderEntities.Length);
    }

    public static implicit operator bool(Battle battle)
    {
        return battle != null;
    }

    [Command]
    private void CmdSetIndicatorPos(Vector3 pos, uint battleIndicatorId)
    {
        BattleIndicator battleIndicator = NetworkServer.spawned[battleIndicatorId].GetComponent<BattleIndicator>();
        battleIndicator.posFollower.destination = pos;
    }

    [Server]
    private void OnArmyChange(object sender, NotifyCollectionChangedEventArgs e)
    {
        if(e.Action != NotifyCollectionChangedAction.Add)
            return;
        Army army = (Army)e.NewItems[0];
        army.movement.TargetAllowMovement(army.owner.connectionToClient, false);
        army.animScript.ServerSetAttackingAnim(true);
    }

    private static readonly WaitForSeconds battleInterval = new WaitForSeconds(1f);

    [Server]
    public IEnumerator RunBattle(ProvinceScript province)
    {
        while(attackingArmies.Count > 0 && defendingArmies.Count > 0)
        {
            // Calculate damage dealt

            // Attacker damage
            float totalAttackerDamage = 0f;
            int totalAttackers = 0;
            for(int i = 0; i < attackingArmies.Count; ++i)
            {
                totalAttackers += attackingArmies[i].entities.Count;
                for(int j = 0; j < attackingArmies[i].entities.Count; ++j)
                {
                    totalAttackerDamage += attackingArmies[i].entities[j].stats.attackDamage * attackingArmies[i].entities[j].stats.attackSpeed;
                }
            }
            indicator.RpcSetAttackerMinions(totalAttackers);
            totalAttackerDamage /= terrainType.defenseMultiplier;
            totalAttackerDamage *= Random.Range(0.5f, 2f);
            // Defender damage
            float totalDefenderDamage = 0f;
            int totalDefenders = 0;
            for(int i = 0; i < defendingArmies.Count; ++i)
            {
                totalDefenders += defendingArmies[i].entities.Count;
                for(int j = 0; j < defendingArmies[i].entities.Count; ++j)
                {
                    totalDefenderDamage += defendingArmies[i].entities[j].stats.attackDamage * defendingArmies[i].entities[j].stats.attackSpeed;
                }
            }
            indicator.RpcSetDefenderMinions(totalDefenders);
            totalDefenderDamage *= Random.Range(0.5f, 2f);
            Debug.Log($"Defenders issue {totalDefenderDamage} damage. Attackers issue {totalAttackerDamage} damage.");

            // Calculate damage taken

            // Attacker take damage
            Entity[] attackerEntities = new Entity[totalAttackers];
            int livingEntityIndex = 0;
            for(int i = attackingArmies.Count - 1; i >= 0; --i)
            {
                bool hasLivingEntity = false;
                for(int j = attackingArmies[i].entities.Count - 1; j >= 0; --j)
                {
                    float individualDefenderDamage = Random.Range(0f, totalDefenderDamage);
                    if(j >= 0)
                        individualDefenderDamage = totalDefenderDamage;
                    totalDefenderDamage -= individualDefenderDamage;
                    if(attackingArmies[i].entities[j].stats.TakeDamage(individualDefenderDamage))
                    {
                        attackingArmies[i].RpcUpdateHealthbar(attackingArmies[i].entities[j].stats.currentHealth);
                        attackerEntities[livingEntityIndex++] = attackingArmies[i].entities[j];
                        hasLivingEntity = true;
                    }
                    else
                    {
                        Army generalArmy = null;
                        for(int k = 0; k < defendingArmies.Count; ++k) // Detect if general is in battle, to determine if general should get gold too
                        {
                            GeneralScript defendingGeneral = defendingArmies[k].GetComponentInChildren<GeneralScript>();
                            if(defendingGeneral != null)
                            {
                                generalArmy = defendingArmies[k];
                                break;
                            }
                        }
                        Debug.Log("Attacker died, give defender gold");
                        if(generalArmy)
                            playerDefender.TargetAddGeneralGold(attackingArmies[i].entities[j].cost, playerDefender.netId, generalArmy.netId); // Award gold
                        else
                            playerDefender.TargetAddGold(attackingArmies[i].entities[j].cost, playerDefender.netId);

                        --totalAttackers;
                        ServerKillEntity(attackingArmies[i], j);
                    }
                }
                if(!hasLivingEntity)
                    attackingArmies.RemoveAt(i);
            }
            // Defender take damage
            Entity[] defenderEntities = new Entity[totalDefenders];
            livingEntityIndex = 0;
            for(int i = defendingArmies.Count - 1; i >= 0; --i)
            {
                bool hasLivingEntity = false;
                for(int j = defendingArmies[i].entities.Count - 1; j >= 0; --j)
                {
                    float individualAttackerDamage = Random.Range(0f, totalAttackerDamage);
                    if(j >= 0)
                        individualAttackerDamage = totalAttackerDamage;
                    totalAttackerDamage -= individualAttackerDamage;
                    if(defendingArmies[i].entities[j].stats.TakeDamage(individualAttackerDamage))
                    {
                        defendingArmies[i].RpcUpdateHealthbar(defendingArmies[i].entities[j].stats.currentHealth);
                        defenderEntities[livingEntityIndex++] = defendingArmies[i].entities[j];
                        hasLivingEntity = true;
                    }
                    else
                    {
                        Army generalArmy = null;
                        for(int k = 0; k < attackingArmies.Count; ++k) // Detect if general is in battle, to determine if general should get gold too
                        {
                            GeneralScript attackingGeneral = attackingArmies[k].GetComponentInChildren<GeneralScript>();
                            if(attackingGeneral != null)
                            {
                                generalArmy = attackingArmies[k];
                                break;
                            }
                        }
                        Debug.Log("Defender died, give attacker gold");
                        if(generalArmy)
                            playerAttacker.TargetAddGeneralGold(defendingArmies[i].entities[j].cost, playerAttacker.netId, generalArmy.netId); // Award gold
                        else
                            playerAttacker.TargetAddGold(defendingArmies[i].entities[j].cost, playerAttacker.netId);

                        --totalDefenders;
                        ServerKillEntity(defendingArmies[i], j);
                    }
                }
                if(!hasLivingEntity)
                    defendingArmies.RemoveAt(i);
            }
            /*if(defendingArmies.Count == 0 || attackingArmies.Count == 0)
                break;*/
            if(attackingArmies.Count == 0)
                indicator.RpcUpdateStrength(0f);
            else if(defendingArmies.Count == 0)
                indicator.RpcUpdateStrength(1f);
            else
                indicator.UpdateStrength(attackerEntities, totalAttackers, defenderEntities, totalDefenders);
            yield return battleInterval;
        }
        NetworkServer.Destroy(indicator.gameObject);
        for(int i = 0; i < attackingArmies.Count; ++i)
            attackingArmies[i].movement.TargetAllowMovement(attackingArmies[i].owner.connectionToClient, true);
        for(int i = 0; i < defendingArmies.Count; ++i)
            defendingArmies[i].movement.TargetAllowMovement(defendingArmies[i].owner.connectionToClient, true);
        province.EndBattle();
    }

    [Server]
    private void ServerKillEntity(Army army, int entityIndex)
    {
        if(army.entities[entityIndex].general)
            army.TargetKillEntity(army.owner.connectionToClient);
        else
            army.ServerKillEntity(entityIndex);
        army.animScript.ServerSetDyingAnim(true, entityIndex);
        army.isDead = true;
    }
}
