using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoLItems {
    Mage,
    Bruiser,
    Tank,
    Support,
    HealthBoots,
    AttackSpeedBoots
}

public class Items : MonoBehaviour
{
    public int GetCost(LoLItems item) {
        switch(item) {
            case LoLItems.Mage: return 100;
            case LoLItems.Bruiser: return 100;
            case LoLItems.Tank: return 100;
            case LoLItems.Support: return 100;
            case LoLItems.HealthBoots: return 50;
            case LoLItems.AttackSpeedBoots: return 50;
            default: return 69;
        }
    }

    public string GetName(LoLItems item) {
        switch(item) {
            case LoLItems.Mage: return "Ludens Echo";
            case LoLItems.Bruiser: return "Hullbreaker";
            case LoLItems.Tank: return "Warmog's Armor";
            case LoLItems.Support: return "Shurelya's Battlesong";
            case LoLItems.HealthBoots: return "Plated Steelcaps";
            case LoLItems.AttackSpeedBoots: return "Beserker's Greaves";
            default: return "BRUH";
        }
    }

    public int GetAttackDamage(LoLItems item) {
        switch (item) {
            case LoLItems.Mage: return 300;
            case LoLItems.Bruiser: return 200;
            case LoLItems.Tank: return 40;
            case LoLItems.Support: return 60;
            case LoLItems.HealthBoots: return 0;
            case LoLItems.AttackSpeedBoots: return 0;
            default: return 69;
        }
    }

    public int GetMaxHealth(LoLItems item) {
        switch(item) {
            case LoLItems.Mage: return 400;
            case LoLItems.Bruiser: return 1000;
            case LoLItems.Tank: return 2000;
            case LoLItems.Support: return 800;
            case LoLItems.HealthBoots: return 2000;
            case LoLItems.AttackSpeedBoots: return 0;
            default: return 69;
        }
    }

    public float GetAttackSpeed(LoLItems item) {
        switch (item) {
            case LoLItems.Mage: return 0.25f;
            case LoLItems.Bruiser: return 0.2f;
            case LoLItems.Tank: return 0.15f;
            case LoLItems.Support: return 0.3f;
            case LoLItems.HealthBoots: return 0;
            case LoLItems.AttackSpeedBoots: return 0.6f;
            default: return 69f;
        }
    }

    public int GetMovementSpeed(LoLItems items) {
        switch(items) {
            case LoLItems.Mage: return 8;
            case LoLItems.Bruiser: return 4;
            case LoLItems.Tank: return 10;
            case LoLItems.Support: return 16;
            case LoLItems.HealthBoots: return 20;
            case LoLItems.AttackSpeedBoots: return 20;
            default: return 69;
        }
    }
}
