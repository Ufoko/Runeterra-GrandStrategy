using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void SetMaxHealth(float health) {

        slider.maxValue = health;
        slider.value = health;
    }

    public  void SetHealth(float health) {
        slider.value = health;
    }

}
