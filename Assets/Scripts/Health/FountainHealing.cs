using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainHealing : MonoBehaviour
{
    public bool isOnFountain = false;

    /*private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Fountain") {
            isOnFountain = false;
        }
    }*/

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Fountain") {
            isOnFountain = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        isOnFountain = false;
    }

    private void Update() {
        if (isOnFountain) {
            Invoke(nameof(HealGeneral), 1f);
        }
    }

    void HealGeneral() {
        /*if (Player.general.entity.stats.currentHealth < Player.general.entity.stats.maxHealth) {
            Player.general.entity.stats.currentHealth += 100f;
            if(Player.general.entity.stats.currentHealth > Player.general.entity.stats.maxHealth)
                Player.general.entity.stats.currentHealth = Player.general.entity.stats.maxHealth;
        } else {
                Player.general.entity.stats.currentHealth = Player.general.entity.stats.maxHealth;
        }
        Player.general.healthBar.SetHealth(Player.general.entity.stats.currentHealth);*/
    }

}
