using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Healthbar : NetworkBehaviour
{

    public Slider slider;

    /// <summary>
    /// Sets the maximum value of the healthbar to scale appropriatly.
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    /// <summary>
    /// Sets the current health value to scale down the filling of the healthbar.
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
