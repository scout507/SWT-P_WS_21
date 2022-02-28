using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* created by: SWT-P_WS_21/22 */

/// <summary>This class is needed to validate the name input in the lobby UI.</summary>
public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    /// <summary>Holds the input field for the name input.</summary>
    [SerializeField] private TMP_InputField nameInput = null;

    /// <summary>Holds the next UI element, after the name input.</summary>
    [SerializeField] private GameObject hostMenu = null;

    /// <summary>Contains the UI of the class itself.</summary>
    [SerializeField] private GameObject nameMenu = null;

    /// <summary>Static variable containing the player name.</summary>
    public static string playerName = "";

    /// <summary>
    /// Reacts to the interaction of a button.
    /// Checks if the input field of the name is empty.
    /// If the input is not empty, the playername is set and the
    /// UI elmente are changed by de- and activating them.
    /// </summary>
    public void continueButtonPressed()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            playerName = nameInput.text;
            this.nameMenu.SetActive(false);
            this.hostMenu.SetActive(true);
        }
    }
}