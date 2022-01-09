using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    public Slider slider;

    // Sets the maximum value of the healthbar to scale appropriatly.
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Sets the current health value to scale down the filling of the healthbar.
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
