using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class processes the interactions of the lobby UI.
/// </summary>
public class JoinLobbyMenu : MonoBehaviour
{
    /// <summary>Holds the Lobby UI network manager</summary>
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    /// <summary>Holds the game object that defines the host UI of the Lobby UI.</summary>
    [SerializeField] private GameObject landingPagePanel = null;

    /// <summary>Holds the input field for the IP address of the Lobby UI.</summary>
    [SerializeField] private TMP_InputField ipAdressInputField = null;

    /// <summary>Holds the Join button of the Lobby UI.</summary>
    [SerializeField] private Button joinButton = null;

    /// <summary>
    /// Called when the object of the class in the scene will activated.
    /// Adds the functions "HandleClientConnected" and "HandleClientDisconnected" to the lobby network manager.
    /// </summary>
    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    /// <summary>
    /// Called when the object of the class in the scene will deactivated.
    /// Removes the functions "HandleClientConnected" and "HandleClientDisconnected" from the lobby network manager.
    /// </summary>
    private void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    /// <summary>
    /// Inserts the IP address from the vaiable "ipAdressInputField" into the network manager.
    /// The client is then started via the network manager and the Join button is deactivated, 
    /// so that the user does not trigger the function again afterwards.
    /// </summary>
    public void JoinLobby()
    {
        string ipAdress = ipAdressInputField.text;

        networkManager.networkAddress = ipAdress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    /// <summary>
    /// Activates the join button.
    /// Deactivates its own GameObject in the UI and the game object that defines the host UI of the Lobby UI.
    /// </summary>
    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    /// <summary>
    /// Activates the join button.
    /// </summary>
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}