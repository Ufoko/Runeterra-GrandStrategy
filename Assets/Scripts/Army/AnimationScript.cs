using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationScript : NetworkBehaviour
{
    float walkingAudioTimer = 3f;
    float walkingAudioTime = 3f;
    bool audioIsPlaying = false;

    [System.NonSerialized] public float volume = 0.3f;

   [SerializeField]private Army army = default;

    private void Update() {
        if (audioIsPlaying) {
            walkingAudioTimer -= Time.deltaTime;
        }

        if (walkingAudioTimer <= 0) {
            walkingAudioTimer = walkingAudioTime;
            audioIsPlaying = false;
        }
    }

   [Command]
   public void CmdSetWalkingAnim(bool value)
   {
        RpcSetWalkingAnim(value);
        RpcSetAttackingAnim(!value);
        RpcSetRecallingAnim(!value);
        ServerSetAllDyingAnim(!value);

   }
   [ClientRpc]
   private void RpcSetWalkingAnim(bool value)
   {
        for (int i = 0; i < army.entities.Count; i++) {
            if(!army.entities[i].anim)
                break;
            army.entities[i].anim.SetBool("IsWalking", value);
            if (value && army.entities[i].walkingAudioClips[i] && !audioIsPlaying) {
                int clip = Random.Range(0, army.entities[i].walkingAudioClips.Length);
                army.entities[i].audioSource.PlayOneShot(army.entities[i].walkingAudioClips[clip]);
                army.entities[i].audioSource.volume = volume;
                audioIsPlaying = true;
            }
        }
   }

   [Command]
   public void CmdSetAttackingAnim(bool value) {
        RpcSetWalkingAnim(!value);
        RpcSetAttackingAnim(value);
        RpcSetRecallingAnim(!value);
        ServerSetAllDyingAnim(!value);
    }
    [Server]
    public void ServerSetAttackingAnim(bool value)
    {
        RpcSetWalkingAnim(!value);
        RpcSetAttackingAnim(value);
        RpcSetRecallingAnim(!value);
        ServerSetAllDyingAnim(!value);
    }
    [ClientRpc]
   private void RpcSetAttackingAnim(bool value) {
        for (int i = 0; i < army.entities.Count; i++) {
            if(!army.entities[i].anim)
                break;
            army.entities[i].anim.SetBool("IsAttacking", value);
            if (value && army.entities[i].attackingAudioClips[i] && !audioIsPlaying) {
                int clip = Random.Range(0, army.entities[i].attackingAudioClips.Length);
                army.entities[i].audioSource.PlayOneShot(army.entities[i].attackingAudioClips[clip]);
                army.entities[i].audioSource.volume = volume;
                audioIsPlaying = true;
            }
        }
    }

    [Command]
    public void CmdSetRecallingAnim(bool value) {
        RpcSetWalkingAnim(!value);
        RpcSetAttackingAnim(!value);
        RpcSetRecallingAnim(value);
        ServerSetAllDyingAnim(!value);
    }
    [ClientRpc]
    private void RpcSetRecallingAnim(bool value) {
        for (int i = 0; i < army.entities.Count; i++) {
            if(!army.entities[i].anim)
                break;
            army.entities[i].anim.SetBool("IsRecalling", value);
            if (value && army.entities[i].recallAudio && !audioIsPlaying) {
                army.entities[i].audioSource.PlayOneShot(army.entities[i].recallAudio);
                army.entities[i].audioSource.volume = volume;
                audioIsPlaying = true; 
            }
            if(army.entities[i].weapon)
                army.entities[i].weapon.SetActive(!value);
        }
    }

    [Command]
    public void CmdSetDyingAnim(bool value) {
        ServerSetAllDyingAnim(value);
        RpcSetAttackingAnim(!value);
        RpcSetWalkingAnim(!value);
        RpcSetRecallingAnim(!value);
    }
    [Server]
    public void ServerSetDyingAnim(bool value, int entityIndex)
    {
        RpcSetDyingAnim(value, entityIndex);
        RpcSetAttackingAnim(!value);
        RpcSetWalkingAnim(!value);
        RpcSetRecallingAnim(!value);
    }
    [Server]
    private void ServerSetAllDyingAnim(bool value)
    {
        for(int i = 0; i < army.entities.Count; ++i)
            RpcSetDyingAnim(value, i);
    }
    [ClientRpc]
    private void RpcSetDyingAnim(bool value, int entityIndex) {
        if(army.entities.Count == 0)
            return;
        Entity entity = army.entities[0];
        if(entity.anim)
            entity.anim.SetBool("IsDead", value);
        if(value && entity.dieAudio) {
            entity.audioSource.PlayOneShot(entity.dieAudio);
            entity.audioSource.volume = volume;
        }
    }

    [Command]
    public void CmdSetIdleAnim(bool value) {
        RpcSetWalkingAnim(!value);
        RpcSetAttackingAnim(!value);
        RpcSetRecallingAnim(!value);
        ServerSetAllDyingAnim(!value);
    }
}
