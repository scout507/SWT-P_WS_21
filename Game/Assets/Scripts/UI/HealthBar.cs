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
    Slider slider;
    public Image HPColor;
    Health healthScript;

    /// <summary>
    /// sets the max health of the slider
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        slider = GetComponent<Slider>();
        slider.maxValue = health;
        slider.value = health;
        healthScript = GetComponentInParent<Health>();
        ColorChanger();
    }

    void Update()
    {
        if(healthScript.health <= 0) slider.value = healthScript.health;
        else slider.value = 0;
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
