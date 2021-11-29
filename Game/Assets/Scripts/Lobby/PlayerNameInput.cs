using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInput = null;
    [SerializeField] private GameObject hostMenu = null;
    [SerializeField] private GameObject nameMenu = null;

    public static string playerName {get; private set;} = "";

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