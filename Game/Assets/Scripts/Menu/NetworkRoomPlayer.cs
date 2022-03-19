using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

/* created by: SWT-P_WS_21/22 */

/// <summary>This class is needed for the prefab of a client in the lobby.</summary>
public class NetworkRoomPlayer : NetworkBehaviour
{
    /// <summary>A synchronised variable containing the name of the player or a placeholder.</summary>
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string displayName = "Loading...";

    /// <summary>Has the Boolean state whether a player is ready or not.</summary>
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;

    /// <summary>Contains all playernems text UI elements.</summary>
    [Header("UI")]
    [SerializeField]
    private TMP_Text[] playerNameTexts;

    /// <summary>Contains all ready text UI elements.</summary>
    [SerializeField]
    private TMP_Text[] playerReadyTexts;

    /// <summary>Holds the UI start button.</summary>
    [SerializeField]
    private Button startGameButton;

    /// <summary>Contains the Canvas of the prefab.</summary>
    [SerializeField]
    private GameObject canvas;

    /// <summary>true, if player is the lobby leader.</summary>
    private bool isLeader;

    /// <summary>Holds the network manager of the UI Lobby.</summary>
    private NetworkManagerLobby room;

    /// <summary>
    /// Triggers when the variable "isReady" changes.
    /// Then calls "UpdateDisplay()".
    /// </summary>
    /// <param name="oldValue">Old boolean value</param>
    /// <param name="newValue">New boolean value</param>
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    /// <summary>
    /// Triggers when the variable "isLeader" changes.
    /// Then calls "UpdateDisplay()".
    /// </summary>
    /// <param name="oldValue">Old display name</param>
    /// <param name="newValue">New display name</param>
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    /// <summary>
    /// Set funcion for isLeader.
    /// Activates the "startGameButton" in the UI, if player is lobby leader.
    /// </summary>
    /// <value>Set value for "isLeader". Is a boolean.</value>
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Triggers the method "CmdSetDisplayName()" when the object is started if the client has the authority.
    /// </summary>
    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.playerName);
    }

    /// <summary>
    /// Triggers when the client/host starts.
    /// Adds the client to the "roomPlayers" list of the network manager.
    /// If the client has the authorisation, the variable "canvas" is set to Activ.
    /// The method "UpdateDisplay()" is then called.
    /// </summary>
    public override void OnStartClient()
    {
        Room.roomPlayers.Add(this);
        if (hasAuthority)
            canvas.SetActive(true);
        UpdateDisplay();
    }

    /// <summary>
    /// Triggers when the client/host stops.
    /// Removes the client from the "roomPlayers" list of the network manager.
    /// The method "UpdateDisplay()" is then called.
    /// </summary>
    public override void OnStopClient()
    {
        Room.roomPlayers.Remove(this);
        UpdateDisplay();
    }

    /// <summary>Method to leave the lobby, stops the networkmanager and reset it.</summary>
    public void Leave()
    {
        Room.StopHost();
    }

    /// <summary>
    /// Makes the "startGameButton" interactive if the client is lobby leader and
    /// all the conditions for starting the game are met.
    /// Called from the Method "notifyPlayersOfReadyState" from "NetworkManagerLobby" class.
    /// </summary>
    /// <param name="readyToStart">true, when all the conditions for starting the game are met.</param>
    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
            return;
        startGameButton.interactable = readyToStart;
    }

    /// <summary>
    /// Get funcion for the "room" variable. 
    /// Checks whether the variable "room" is null:
    /// If true, the varibale is assigned an instance by the network manager.
    /// If False, then the variable is returned.
    /// </summary>
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null)
                return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    /// <summary>
    /// Updated the player names and their ready status in the UI.
    /// The authority check, prevents access to the method without having the authority.
    /// </summary>
    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.roomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for Player";
            playerReadyTexts[i].text = "";
        }

        for (int i = 0; i < Room.roomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.roomPlayers[i].displayName;
            playerReadyTexts[i].text = Room.roomPlayers[i].isReady
                ? "<color=green>Ready</color>"
                : "<color=red>Not Ready</color>";
        }
    }

    /// <summary>
    /// This command sets the value of "displayName" to the synchronised variable "this.displayName".
    /// </summary>
    /// <param name="displayName">Contains the name of the client or the placeholder.</param>
    [Command]
    public void CmdSetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    /// <summary>
    /// The command changes the boolean "isReady" and informs all other clients about the change.
    /// </summary>
    [Command]
    public void CmdReadyUp()
    {
        isReady = !isReady;

        Room.notifyPlayersOfReadyState();
    }

    /// <summary>
    /// This command starts the game. It checks whether the client calling this command 
    /// is the leader.
    /// </summary>
    [Command]
    public void CmdStartGame()
    {
        if (Room.roomPlayers[0].connectionToClient != connectionToClient)
            return;

        Room.startGame();
    }
}
