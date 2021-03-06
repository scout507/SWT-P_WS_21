using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>This class processes the interactions of the lobby UI.</summary>
public class JoinLobbyMenu : MonoBehaviour
{
    /// <summary>Holds the game object that defines the host UI of the Lobby UI.</summary>
    [Header("UI")]
    [SerializeField]
    private GameObject landingPagePanel = null;

    /// <summary>Holds the input field for the IP address of the Lobby UI.</summary>
    [SerializeField]
    private TMP_InputField ipAdressInputField = null;

    /// <summary>Holds the Join button of the Lobby UI.</summary>
    [SerializeField]
    private Button joinButton = null;

    /// <summary>
    /// Inserts the IP address from the vaiable "ipAdressInputField" into the network manager.
    /// The client is then started via the network manager and the join button is deactivated, 
    /// so that the user does not trigger the function again afterwards.
    /// </summary>
    public void JoinLobby()
    {
        if (!string.IsNullOrEmpty(ipAdressInputField.text))
        {
            string ipAdress = ipAdressInputField.text;

            NetworkManager.singleton.networkAddress = ipAdress;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }
    }

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
    /// Activates the join button after connected to server.
    /// Deactivates its own GameObject in the UI and the game object that defines the host UI of the Lobby UI.
    /// </summary>
    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        landingPagePanel.SetActive(true);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Activates the join button after connection failed to server.
    /// </summary>
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
