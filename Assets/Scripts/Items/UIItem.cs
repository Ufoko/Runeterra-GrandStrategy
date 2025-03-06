using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IPointerClickHandler
{
    ShopUI shop;
    public LoLItems itemType;
    public GameObject itemText;
    public GameObject buyButton;

    private void Start() {
        shop = FindObjectOfType<ShopUI>();
    }

    public void OnPointerClick(PointerEventData eventdata) {
        switch (gameObject.name) {
            case "Ludens2":
                shop.changeItemUI(itemText, LoLItems.Mage, buyButton);
                break;
            case "Hullbreaker":
                shop.changeItemUI(itemText, LoLItems.Bruiser, buyButton);
                break;
            case "Warmogs":
                shop.changeItemUI(itemText, LoLItems.Tank, buyButton);
                break;
            case "Shurelya":
                shop.changeItemUI(itemText, LoLItems.Support, buyButton);
                break;
            case "Plated Steelcaps":
                shop.changeItemUI(itemText, LoLItems.HealthBoots, buyButton);
                break;
            case "Greaves":
                shop.changeItemUI(itemText, LoLItems.AttackSpeedBoots, buyButton);
                break;
        }
    }
}
