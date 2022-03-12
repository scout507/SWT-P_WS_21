using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// Slider to adjust the Size
    /// </summary>
    public Slider slider;
    public Image HPColor;

    /// <summary>
    /// sets the max health of the slider
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        ColorChanger();
    }

    /// <summary>
    /// sets the current value of the slider
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        slider.value = health;
        ColorChanger();
    }

    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (slider.value / slider.maxValue));
        HPColor.color = healthColor;
    }
}
