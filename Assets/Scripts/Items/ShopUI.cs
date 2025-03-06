using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopUI : MonoBehaviour { 
    [SerializeField]GameObject Iventory;
    GameObject itemPrefab;
    Vector3 itemPos;
    float itemPosHelp = 190.75f;
    int numOfItems;
    public GameObject ShopDebugText;
    public GameObject generalDebugText;
    TMP_Text debugText;
    public GameObject generalIcon;
    public GameObject generalHUD;
    public GameObject ItemHUD;

    public Sprite generalLuxSprite;
    public Sprite generalDariusSprite;

    public int maxDistance;

    bool boughtBoots = false;

    bool button1Clicked = false;
    bool button2Clicked = false;

    public GameObject[] iventoryUI = new GameObject[4];
    public GameObject[] buyButtonUI = new GameObject[4];

    Items items;

    void Start() {
        items = FindObjectOfType<Items>();
        itemPos = Iventory.transform.position;
        debugText = ShopDebugText.GetComponent<TMP_Text>();
    }

    public void BuyItem(LoLItems item, GameObject go) {
        ShopDebugText.SetActive(false);

        if (!boughtBoots)
        {
            if (numOfItems < 5)
            {
                if (Player.general.gold >= items.GetCost(item))
                {
                    //Place Item in HUD
                    PlaceItemInUI(item, go);
                    //Stats
                    AddStats(item, go);
                }
                else
                {
                    ShopDebugText.SetActive(true);
                    debugText.text = "not enough gold";
                }
            }
            else {
                ShopDebugText.SetActive(true);
                debugText.text = "No more item slots";
            }
        } else {
            if (item == LoLItems.HealthBoots || item == LoLItems.AttackSpeedBoots) {
                ShopDebugText.SetActive(true);
                debugText.text = "You already have boots";
            } else {
                if (numOfItems < 5) {
                    if (Player.general.gold >= items.GetCost(item)) {
                        //Place Item in HUD
                        PlaceItemInUI(item, go);
                        //Stats
                        AddStats(item, go);
                    } else {
                        ShopDebugText.SetActive(true);
                        debugText.text = "not enough gold";
                    }
                } else {
                    ShopDebugText.SetActive(true);
                    debugText.text = "No more item slots";
                }
            }
        }
 
    }

    public void AddStats(LoLItems item, GameObject go) {
        /*Player.general.entity.stats.attackDamage += items.GetAttackDamage(item);
        Player.general.entity.stats.maxHealth += items.GetMaxHealth(item);
        Player.general.healthBar.SetMaxHealth(Player.general.entity.stats.maxHealth);
        Player.general.movementSpeed += items.GetMovementSpeed(item);
        Player.general.entity.stats.attackSpeed += items.GetAttackSpeed(item);*/
        Player.local.AddStats(item);
    }

    public void changeItemUI(GameObject TextToSetActive, LoLItems item, GameObject buyButtonToSetActive) {
        for (int i = 0; i < iventoryUI.Length; i++) {
            iventoryUI[i].SetActive(false);
        }
        for (int i = 0; i < buyButtonUI.Length; i++) {
            buyButtonUI[i].SetActive(false);
        }
        buyButtonToSetActive.SetActive(true);
        TextToSetActive.SetActive(true);
        TMP_Text itemText = TextToSetActive.gameObject.GetComponent<TMP_Text>();
        itemText.text =
        items.GetName(item) + "\n" +
        "Attack: " + items.GetAttackDamage(item) + "\n" +
        "Health: " + items.GetMaxHealth(item) + "\n" +
        "Attackspeed: " + items.GetAttackSpeed(item) + "\n" +
        "Movementspeed: " + items.GetMovementSpeed(item) + "\n" +
        "Cost: " + items.GetCost(item) + " Local Gold";
    }

    void PlaceItemInUI(LoLItems item, GameObject go) {
        GameObject temp = Instantiate(go, itemPos, Quaternion.identity);
        temp.transform.localScale = Vector3.one * 0.5f;
        temp.transform.position = new Vector3(itemPos.x - itemPosHelp, itemPos.y, itemPos.z);
        temp.transform.parent = Iventory.transform;
        Player.general.gold -= items.GetCost(item);
        itemPosHelp -= 100;
        numOfItems++;
    }

    public void GoToItems() {
        generalHUD.SetActive(false);
        ItemHUD.SetActive(true);
        generalDebugText.SetActive(false);
    }

    void Update () {
        if (Nexus.clickedNexus) {
            if (Vector3.Distance(Player.general.transform.position, Nexus.clickedNexus.transform.position) < maxDistance) {
                generalHUD.SetActive(true);
                generalIcon.SetActive(true);
                generalDebugText.SetActive(false);

                if (Player.general.isLux) {
                    generalIcon.GetComponent<Image>().sprite = generalLuxSprite;
                } else {
                    generalIcon.GetComponent<Image>().sprite = generalDariusSprite;
                }
            } else {
                generalIcon.SetActive(false);
                generalDebugText.SetActive(true);
                generalDebugText.GetComponent<TMP_Text>().text = "Your Champion is too far away";
            }
        }
    }

    LoLItems item;
    public void SetItemPrefab(GameObject itemPrefab2) {
        button1Clicked = true;
        itemPrefab = itemPrefab2;
        if (button1Clicked == true && button2Clicked == true) {
            BuyItem(item, itemPrefab);
            button1Clicked = false;
            button2Clicked = false;
        }
    }

    public void SetEnumItem(int enumNum) {
        item = (LoLItems)enumNum;
        button2Clicked = true;
        if (button1Clicked == true && button2Clicked == true) {
            BuyItem(item, itemPrefab);
            button1Clicked = false;
            button2Clicked = false;
        }
    }

    public void BoughtBoots()
    {
        boughtBoots = true;
    }
}
