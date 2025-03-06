using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class ProvinceScript : MonoBehaviour
{
    TerrainType terrainType;
   public string terrainTT;
    int terrainInt = -1;
    public int x;
    public int z;
    public int towerLevel = 0;

    float boundDifference = 4096;
    float imageSize = 512;
    public List<Color> terrainColor = new List<Color>();

    [System.NonSerialized] public List<Army> armies = new List<Army>();
    private Battle battle;
    public string provinceName;
    [SerializeField] TMP_Text provinceTextName;

    [System.NonSerialized] public Nexus nexus;

    private void Start()
    {
        
        Vector3 position;
        position.x = x * ((boundDifference / imageSize));
        position.z = z * ((boundDifference / imageSize));

        position.x -= 2048;
        position.z -= 2048;

        position.y = 0;

        transform.localPosition = position;

        terrainType = DetermineTerrain();
        provinceTextName.text = provinceName;
        provinceTextName.gameObject.GetComponent<PositionProvinceText>().SetPosition();
        provinceTextName.gameObject.SetActive(false);
    }
    /*
     grassLands               green
     mountain                 brown
     desert                   yellow
     frozenTerrain            blue white
     water                    dark blue
    */

    private void Update()
    {
        
    }

    TerrainType DetermineTerrain()
    {
       TerrainType tempTerrain;

        int greenlands = 0, mountain = 0, desert = 0, frozenMountian = 0, water = 0, otherTerrain = 0;
        foreach (Color color in terrainColor)
        {
            Color32 newColor = color;
            float r, g, b;
            
            //Color.RGBToHSV(color, out h, out s, out v);

            //covert HSV from 0-1 to 0-360/100
            r = newColor.r;
            g = newColor.g;
            b = newColor.b;
            //ice
            if (r == 143 && g == 172 && b == 167)
            {
                frozenMountian++;
            }
            //mountain
            else if(r == 134 && g == 114 && b == 93)
                {
                mountain++;
            }
            //yellow orange
            else if (r == 191 && g == 147 && b == 99)
            {
                desert++;
            }
            //green
            else if(r == 90 && g == 108 && b == 48)
            {
                greenlands++;
            }
            //light blue
            else if(r == 210 && g == 212 && b == 212)
            {
                frozenMountian++;
            }
            //water
            else if(r == 61 && g == 87 && b == 93)
            {
                water++;
            }
            else
            {
                otherTerrain++;
              //  Debug.LogError("Colour shouldn't exist: " + "H: " + h + " S: " + s + " V: " + v);
            }
        }
        int[] terrains = { greenlands, mountain, frozenMountian, desert, water, otherTerrain };
       
        int tempHighest = 0;

        for (int i = 0; i < terrains.Length; i++)
        {
            if(terrains[i] > tempHighest)
            {
                tempHighest = terrains[i];
            }
            else if(terrains[i] == tempHighest)
            {
               // Debug.LogError("WTF");
            }
        }

        if(terrains[0] == tempHighest)
        {
            terrainInt = 0;
            terrainTT = "Grass";
            tempTerrain = TerrainType.grassLands;
        }
        else if( terrains[1] == tempHighest)
        {
            terrainInt = 1;
            terrainTT = "Mountain";
            tempTerrain = TerrainType.mountain;
        }
        else if (terrains[2] == tempHighest)
        {
            terrainInt = 2;
            terrainTT = "Frozen";
            tempTerrain = TerrainType.frozen;
        }
        else if (terrains[3] == tempHighest)
        {
            terrainInt = 3;
            terrainTT = "Desert";
            tempTerrain = TerrainType.desert;
        }
        else if (terrains[4] == tempHighest)
        {
            terrainInt = 4;
            terrainTT = "Water";
            tempTerrain = TerrainType.water;
        }
        else if (terrains[5] == tempHighest)
        {
            terrainInt = 4;
            terrainTT = "OTHERWater";
            tempTerrain = TerrainType.water;
        }
        else
        {
            terrainInt = 4;
            terrainTT = "Other";
            return TerrainType.water;
        }

        return tempTerrain;
    }

    public void OpenProvinceInterface()
    {
        ProvinceView.singleton.LoadProvinceView(terrainInt,this.gameObject,provinceName,towerLevel);
    }

    [Server]
    public void NewArmy(Army army)
    {
        if(battle) // Enter existing battle
        {
            if(army.owner == battle.playerAttacker)
                battle.attackingArmies.Add(army);
            else if(army.owner == battle.playerDefender)
                battle.defendingArmies.Add(army);
            //army.movement.TargetAllowMovement(army.owner.connectionToClient, false);
            //army.animScript.ServerSetAttackingAnim(true);
        }
        else // Start new battle
        {
            for(int i = 0; i < armies.Count; ++i)
            {
                if(armies[i].isDead)
                    continue;
                if(armies[i].owner != army.owner)
                {
                    battle = new Battle(army.owner, armies[i].owner, army, armies, transform.position, terrainType);
                    StartCoroutine(battle.RunBattle(this));
                    break;
                }
            }
        }

        armies.Add(army);
    }

    public void EndBattle()
    {
        Debug.Log("Battle ended");
        battle = null;
    }
}
