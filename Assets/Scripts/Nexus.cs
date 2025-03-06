using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Nexus : NetworkBehaviour
{
    public static Nexus[] nexuses = new Nexus[2];

    [Header("Stats and things")]
    public bool isBlueNexus;

    [Header("References")]
    public GameObject HUD;
    public GameObject minionHUD;
    public GameObject generalShopHUD;
    public GameObject shopHUD;
    public GameObject itemShopHUD;
    public GameObject minionBuyHUD;
    public GameObject NexusHUD;
    public GameObject shopDebugText;
    [System.NonSerialized] public static Nexus clickedNexus;
    public Material minionMaterial;

    [Header("SelfReference")]
    public int nexusId;

    private float health = 100;
    private ProvinceScript province;

    private void Awake()
    {
        nexuses[nexusId] = this;
    }

    void Start()
    {
        province = ProvinceClicking.singleton.GetNearestProvince(transform.position);
        province.nexus = this;
    }

    private void OnMouseDown() {
        if(gameObject != Player.local.myNexus)
            return;
        HUD.SetActive(true);
        clickedNexus = this;
        Minions.singleton.ChangeArmyPos();    
    }

    public void CloseUI() {
        HUD.SetActive(false);
    }

    public void OpenMinionHUD() {
        HUD.SetActive(false);
        minionHUD.SetActive(true);
    }

    public void CloseMinionHUD() {
        minionHUD.SetActive(false);
        HUD.SetActive(true);
    }

    public void OpenShopHUD() {
        HUD.SetActive(false);
        shopHUD.SetActive(true);
        itemShopHUD.SetActive(false);
        generalShopHUD.SetActive(true);
        shopDebugText.SetActive(false);
    }

    public void CloseShopHUD() {
        generalShopHUD.SetActive(false);
        shopHUD.SetActive(false);
        HUD.SetActive(false);
        minionBuyHUD.SetActive(false);
        NexusHUD.SetActive(false);
        clickedNexus = null;
    }

}
