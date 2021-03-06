using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This class is responsible for handling the chatbox displayed on the UI.
/// If you want to use this class to send a message to a player,
/// please call the AddMessage function.
/// </summary>
public class Chatbox : NetworkBehaviour
{
    /// <summary>The UI component holding the textbox.</summary>
    [SerializeField]
    GameObject textBoxUI;

    //  <summary>The UI component holding the input field.</summary>
    [SerializeField]
    TMP_InputField chatBoxUI;

    /// <summary>Cooldown for the message sound effect</summary>
    [SerializeField]
    float audioCooldown = 0.5f;

    /// <summary>The text component of the textbox.</summary>
    TextMeshProUGUI textBox;

    /// <summary>The text component of the input field.</summary>
    TextMeshProUGUI chatText;

    /// <summary>Array holding the newest 5 messages</summary>
    string[] messages = new string[5];

    /// <summary>Time until a message is deleted</summary>
    float timeForDeletion = 25f;

    /// <summary>Timer for deletion</summary>
    float[] timer = new float[5];

    /// <summary>The name of the player for the chat</summary>
    string playerName;

    /// <summary>True when the player opened the chat</summary>
    bool typing;

    /// <summary>Audiocontroller for playing sounds</summary>
    AudioController audioController;

    /// <summary>Timer for tracking the last message sound</summary>
    float audioTimer;

    /// <summary>
    /// Fetches dependencies
    /// </summary>
    void Start()
    {
        if (!isLocalPlayer)
            return;

        textBox = textBoxUI.GetComponent<TextMeshProUGUI>();
        chatText = chatBoxUI.GetComponentInChildren<TextMeshProUGUI>();
        playerName = GetComponent<Player>().displayName;
        chatBoxUI.GetComponent<Image>().color -= new Color(0, 0, 0, 0.8f);
        audioController = GetComponent<AudioController>();
    }

    /// <summary>
    /// Registers input. And triggers message sending.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
            return;
        UpdateTimers();
        UpdateText();

        audioTimer += Time.deltaTime;

        if (typing)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                typing = false;
                chatBoxUI.DeactivateInputField();
                chatBoxUI.GetComponent<Image>().color -= new Color(0, 0, 0, 0.8f);
                if (chatBoxUI.text.Length > 0)
                    SubmitMessage(chatText.text);
            }
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) && !typing)
        {
            typing = true;
            chatBoxUI.GetComponent<Image>().color += new Color(0, 0, 0, 0.8f);
            chatBoxUI.ActivateInputField();
        }

        if (!typing)
            chatBoxUI.text = "";

        //Quick-Chat

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SubmitMessage("HELP");
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SubmitMessage("OK");
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SubmitMessage("STOP");
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SubmitMessage("FOLLOW ME");
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            SubmitMessage("NICE");
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SubmitMessage("KILL THE ZOMBIES");
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SubmitMessage("NEED HEAL");
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            SubmitMessage("RETREAT");
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            SubmitMessage("WATCH OUT");
        }
    }

    /// <summary>
    /// Adds a new message to the UI. If you want to send a message to a player, use this function.
    /// Can hold a maximum of 5 messages before it overrides the oldes one.
    /// </summary>
    /// <param name="message">The message you want to send</param>
    public void AddMessage(string message)
    {
        if (audioController && audioTimer >= audioCooldown)
        {
            audioController.PlayMessageSound();
            audioTimer = 0;
        }
        for (int i = 4; i > 0; i--)
        {
            messages[i] = messages[i - 1];
            timer[i] = timer[i - 1];
        }
        messages[0] = message;
        timer[0] = 0;
        UpdateText();
    }

    /// <summary>
    /// Updates the timers for message deletion.
    /// </summary>
    void UpdateTimers()
    {
        for (int i = 0; i < 5; i++)
        {
            timer[i] += Time.deltaTime;
            if (timer[i] >= timeForDeletion)
            {
                messages[i] = "";
                UpdateText();
                timer[i] = 0;
            }
        }
    }

    /// <summary>
    /// Updates the text of the chatbox.
    /// </summary>
    void UpdateText()
    {
        string text = "";
        for (int i = 4; i > -1; i--)
        {
            text += messages[i] + "\n";
        }

        textBox.text = text;
    }

    /// <summary>
    /// Submits a message to the chat. 
    /// Sends the message via command to all nearby players.
    /// </summary>
    /// <param name="message">The message to send.</param>
    void SubmitMessage(string message)
    {
        List<GameObject> nearbyPlayers = FindNearbyPlayers();

        if (nearbyPlayers.Count > 0)
        {
            foreach (GameObject player in nearbyPlayers)
            {
                CmdSendMessage(playerName + ": " + message, player);
            }
        }

        AddMessage("You: " + message);
        chatBoxUI.text = "";
    }

    /// <summary>
    /// Finds all players within a given radius
    /// </summary>
    /// <returns>Returns a list of all nearby player gameObjects, excluding the own.</returns>
    List<GameObject> FindNearbyPlayers()
    {
        List<GameObject> nearbyPlayers = new List<GameObject>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15f);

        foreach (Collider hitCollider in hitColliders)
        {
            if (
                hitCollider.gameObject.tag == "Player"
                && !nearbyPlayers.Contains(hitCollider.gameObject)
                && hitCollider.gameObject != this.gameObject
            )
            {
                nearbyPlayers.Add(hitCollider.gameObject);
            }
        }
        return nearbyPlayers;
    }

    /// <summary>
    /// For distributing a message to other players.
    /// Only used for player-to-player messages.
    /// </summary>
    /// <param name="message">Message to send.</param>
    /// <param name="target">The player GameObject to send it to.</param>
    [Command]
    void CmdSendMessage(string message, GameObject target)
    {
        TargetRPCSendMessage(target.GetComponent<NetworkIdentity>().connectionToClient, message);
    }

    /// <summary>
    /// Adds a given message to the checkbox of the target player.
    /// Only used for player-to-player messages.
    /// </summary>
    /// <param name="target">NetworkConnection of the Target.</param>
    /// <param name="message">The message to deliver.</param>
    [TargetRpc]
    void TargetRPCSendMessage(NetworkConnection target, string message)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>().AddMessage(message);
    }
}
