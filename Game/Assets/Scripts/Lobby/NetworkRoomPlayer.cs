using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private Toggle[] playerReadyToggles;
    [SerializeField] private Button startGameButton;

    [SyncVar(hook = nameof(handleDisplayNameChanged))]
    public string displayName = "Loading...";
    [SyncVar(hook = nameof(handleReadyStatusChanged))]
    public bool isReady = false;

    private bool isLeader;
    private NetworkManagerLobby room;

    public void handleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void handleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(true);
        }
    }

    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.playerName);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.roomPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.roomPlayers.Remove(this);
        UpdateDisplay();
    }

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
            playerReadyToggles[i].isOn = false;
        }

        for (int i = 0; i < Room.roomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.roomPlayers[i].displayName;
            playerReadyToggles[i].isOn = Room.roomPlayers[i].isReady ? true : false;
        }

    }

    public void handleReadyToStart(bool readyToStart)
    {
        if (!isLeader) return;
        startGameButton.interactable = readyToStart;
    }

    public void stopGame()
    {
        NetworkManagerLobby test = GameObject.FindGameObjectWithTag("NetworkManagerLobby").GetComponent<NetworkManagerLobby>();
        test.StopClient();
    }

    [Command]
    public void CmdSetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        isReady = !isReady;

        Room.notifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.roomPlayers[0].connectionToClient != connectionToClient) return;
        //Start Game
    }

}
