using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralScript : MonoBehaviour
{
    public Entity entity;
    public HealthBarScript healthBar;
    public bool isLux;

    [Header("General stats")]
    public float movementSpeed = 5;
    public float experincePoints = 0;
    public int level = 1;
    private float experiencePointsNeededForLevel;
    public float gold = 100;


    public virtual void LevelUp()
    {
        // increast stats, play lvl up animation
    }






}
