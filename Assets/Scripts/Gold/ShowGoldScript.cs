using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowGoldScript : MonoBehaviour
{
    public TMP_Text goldIndicator;
    bool localGold = false;
    bool globalGold = false;


    private void Update() {
        if (Player.local) {
            if (localGold && !globalGold) {
                goldIndicator.text = Player.general.gold.ToString();
            } else if (!localGold && globalGold) {
                goldIndicator.text = Player.local.gold.ToString();
            }
        }
    }
    public void ChangeToLocalGold() {
        goldIndicator.gameObject.SetActive(true);
        localGold = true;
        globalGold = false;
    }

    public void ChangeToGlobalGold() {
        goldIndicator.gameObject.SetActive(true);
        globalGold = true;
        localGold = false;
    }

}
