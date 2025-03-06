using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SLET DETTE LORT
public class FightingManager : MonoBehaviour
{
    public static FightingManager singleton;

    private float timeBetweenFight = 2;

    private List<IEnumerator> fightingNumerators = new List<IEnumerator>();

    private void Awake()
    {
        singleton = this;
    }

    public void Fight(GameObject defender, GameObject attaker, int terrainModifier)
    {

        fightingNumerators.Add(FightNumerator(defender,attaker,terrainModifier,fightingNumerators.Count-1));


    }

    private IEnumerator FightNumerator(GameObject defender, GameObject attacker, int terrainModifier, int listNumber)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenFight);

            //fighting ting


            //if(defender.menLeft <= 0 || attacker.menLeft <= 0)
            {
                StopCoroutine(fightingNumerators[listNumber]);
                yield return false;
            }
        }
    }


}
