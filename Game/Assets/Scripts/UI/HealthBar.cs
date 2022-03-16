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


    private void Start()
    {
        healthScript = GetComponentInParent<Health>();
        slider = GetComponent<Slider>();
        slider.maxValue = healthScript.health;
        slider.value = healthScript.health;
        ColorChanger();
    }

    void Update()
    {
        if(healthScript.health >= 0) slider.value = healthScript.health;
        else slider.value = 0;
        ColorChanger();
    }


    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (slider.value / slider.maxValue));
        HPColor.color = healthColor;
    }
}
