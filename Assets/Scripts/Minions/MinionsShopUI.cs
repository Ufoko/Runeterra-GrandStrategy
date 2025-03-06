using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinionsShopUI : MonoBehaviour
{
    public GameObject[] CannonMinionBuyButtons = new GameObject[4];
    public GameObject[] SuperMinionBuyButtons = new GameObject[4];

    public TMP_Text showMinion;
    public TMP_Text showCost;

    public void changeToCannonMinionUI(string name) {
        for (int i = 0; i < CannonMinionBuyButtons.Length; i++) {
            CannonMinionBuyButtons[i].SetActive(true);
            SuperMinionBuyButtons[i].SetActive(false);

            showMinion.gameObject.SetActive(true);
            showCost.gameObject.SetActive(true);
            showMinion.text = name;
            showCost.text = "Cost: " + Minions.singleton.cannonMinionCost.ToString() + " Global Gold";
        }
    }

    public void changeToSuperMinionUI(string name) {
        for (int i = 0; i < SuperMinionBuyButtons.Length; i++) {
            CannonMinionBuyButtons[i].SetActive(false);
            SuperMinionBuyButtons[i].SetActive(true);
            
            showMinion.gameObject.SetActive(true);
            showCost.gameObject.SetActive(true);
            showMinion.text = name;
            showCost.text = "Cost: " + Minions.singleton.superMinionCost.ToString() + " Global Gold";
        }
    }
}
