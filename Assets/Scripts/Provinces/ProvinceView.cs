using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProvinceView : MonoBehaviour
{
    public static ProvinceView singleton;

    [Tooltip("0 - mark; 1 - mountain; 2 - ice mountain; 3 - desert")]
    [SerializeField] Texture2D[] provinceTerrainPicture;
    public UnityEngine.UI.RawImage terrainImage;
    public string provinceName;
    public TMP_Text provinceNameText;
    public int towerLevel = 0;
    public GameObject provinceClicked;


    private void Awake()
    {
        singleton = this;
        gameObject.SetActive(false);
    }




    public void LoadProvinceView(int provinceTerrain, GameObject clickedProvince,string provincesName, int towerLevel = 0)
    {
       

        gameObject.SetActive(true);
        terrainImage.texture = provinceTerrainPicture[provinceTerrain];
        provinceClicked = clickedProvince;
        provinceName = provincesName;
        provinceNameText.text = provinceName;

    }




}
