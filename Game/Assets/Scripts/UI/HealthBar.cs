using UnityEngine;
using UnityEngine.UI;

/* created by: SWT-P_WS_21/22 */

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

    /// <summary>
    /// Gets dependencies and sets up the healthbar.
    /// </summary>
    private void Start()
    {
        healthScript = GetComponentInParent<Health>();
        slider = GetComponent<Slider>();
        slider.maxValue = healthScript.health;
        slider.value = healthScript.health;
        ColorChanger();
    }

    /// <summary>
    /// Updates the healthbar values.
    /// </summary>
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
