using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;

    /// <summary>
    /// sets the max health of the slider
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    /// <summary>
    /// sets the current value of the slider
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
