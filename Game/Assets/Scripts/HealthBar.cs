using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
<<<<<<< HEAD
{ 
    public Slider slider;

    // Sets the vaximum value of the healthbar slider when initialized
=======
{
    public Slider slider;

    /// <summary>
    /// sets the max health of the slider
    /// </summary>
    /// <param name="health"></param>
>>>>>>> origin/main
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

<<<<<<< HEAD
    //sets the current value of the slider
=======
    /// <summary>
    /// sets the current value of the slider
    /// </summary>
    /// <param name="health"></param>
>>>>>>> origin/main
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
