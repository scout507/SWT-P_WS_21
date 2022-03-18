/* created by: SWT-P_WS_21/22 */
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controlls the change of the Healthbar.
/// </summary>
public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// Slider to adjust the size
    /// </summary>
    Slider slider;

    /// <summary>
    /// Image of the ColorFilling
    /// </summary>
    public Image HPColor;

    /// <summary>
    /// Reference to the HealthScript
    /// </summary>
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
        if (healthScript.health >= 0)
            slider.value = healthScript.health;
        else
            slider.value = 0;
        ColorChanger();
    }

    /// <summary>
    /// Changes the Color of the healthbar based on the value in it.
    /// </summary>
    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (slider.value / slider.maxValue));
        HPColor.color = healthColor;
    }
}
