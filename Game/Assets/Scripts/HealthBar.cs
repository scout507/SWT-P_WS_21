using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{ 
    public Slider slider;

    // Sets the vaximum value of the healthbar slider when initialized
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    //sets the current value of the slider
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
