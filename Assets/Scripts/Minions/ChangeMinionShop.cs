using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeMinionShop : MonoBehaviour, IPointerClickHandler
{
    MinionsShopUI minionsShop;

    private void Start() {
        minionsShop = FindObjectOfType<MinionsShopUI>();
    }
    public void OnPointerClick(PointerEventData eventdata) {
        switch(gameObject.name) {
            case "Cannon Minion":
                minionsShop.changeToCannonMinionUI(gameObject.name);
                break;
            case "Super Minion":
                minionsShop.changeToSuperMinionUI(gameObject.name);
                break;
        }
    }
}
