using UnityEngine;
using UnityEngine.UI;
using TMPro;
using V1king;
using Mirror;

public class BattleIndicator : NetworkBehaviour
{
    public UIPosFollower posFollower;

    public Image strengthBg;
    public Image strengthFill;

    [SerializeField] private Image attackerBg = default;
    [SerializeField] private Image attackerCrest = default;
    [SerializeField] private TextMeshProUGUI attackerMinionText = default;

    [SerializeField] private Image defenderBg = default;
    [SerializeField] private Image defenderCrest = default;
    [SerializeField] private TextMeshProUGUI defenderMinionText = default;

    [ClientRpc]
    public void RpcSetAttackerFaction(int factionId)
    {
        Faction faction = ObjectManager.singleton.factions[factionId];
        strengthFill.color = faction.color;
        attackerBg.color = faction.color;
        attackerCrest.sprite = faction.crest;
    }
    [ClientRpc]
    public void RpcSetDefenderFaction(int factionId)
    {
        Faction faction = ObjectManager.singleton.factions[factionId];
        strengthBg.color = faction.color;
        defenderBg.color = faction.color;
        defenderCrest.sprite = faction.crest;
    }

    [ClientRpc]
    public void RpcSetAttackerMinions(int minions)
    {
        attackerMinionText.text = minions.ToString();
    }
    [ClientRpc]
    public void RpcSetDefenderMinions(int minions)
    {
        defenderMinionText.text = minions.ToString();
    }

[Server]
public void UpdateStrength(Entity[] attackerEntities, int attackerAmount, Entity[] defenderEntities, int defenderAmount)
{
    float attackerHP = 0f;
    for(int i = 0; i < attackerAmount; ++i)
        attackerHP += attackerEntities[i].stats.currentHealth;

    float defenderHP = 0f;
    for(int i = 0; i < defenderAmount; ++i)
        defenderHP += defenderEntities[i].stats.currentHealth;

    float totalHP = attackerHP + defenderHP;
    float interp = MathConversions.ConvertNumberRange((attackerHP / totalHP) - (defenderHP / totalHP), -1f, 1f, 0f, 1f);
    RpcUpdateStrength(interp);
}
[ClientRpc]
public void RpcUpdateStrength(float interp)
{
    strengthFill.transform.localScale = new Vector3(interp, strengthFill.transform.localScale.y, strengthFill.transform.localScale.z);
}
}
