using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Chatbox : NetworkBehaviour
{
    /// <summary>The UI component holding the textbox.</summary>
    [SerializeField] GameObject textBoxUI;
    //  <summary>The UI component holding the input field.</summary>
    [SerializeField] TMP_InputField chatBoxUI;
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

    string userInput = "";
    string playerName;
    bool typing;

    /// <summary>
    /// Fetches dependencies
    /// </summary>
    void Start()
    {
        if (!isLocalPlayer) return;

        textBox = textBoxUI.GetComponent<TextMeshProUGUI>();
        chatText = chatBoxUI.GetComponentInChildren<TextMeshProUGUI>();
        playerName = GetComponent<Player>().displayName;

    }

    void Update()
    {
        if (!isLocalPlayer) return;
        UpdateTimers();
        UpdateText();


        if(typing)
        {
            if(Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                typing = false;
                if(chatBoxUI.text.Length >0) SubmitMessage(chatText.text);
            }
            
        }
        if(Input.GetKeyDown(KeyCode.KeypadEnter) && !typing)
        {
            typing = true;
            chatBoxUI.Select();
        }
        
    }

    /// <summary>
    /// Adds a new message to the UI. Can hold a maximum of 5 messages before it overrides the oldes one.
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(string message)
    {
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
    /// Updates the timers for message deletion
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
        Debug.Log(messages[0]);
        for (int i = 4; i > -1; i--)
        {
            text += messages[i] + "\n";
        }

        textBox.text = text;
    }

    void SubmitMessage(string message)
    {
        List<GameObject> nearbyPlayers = FindNearbyPlayers();

        if(nearbyPlayers.Count > 0)
        {
            foreach(GameObject player in nearbyPlayers)
            {
                CmdSendMessage(playerName + ": " + message, player);
            }
        }

        AddMessage("You: " + message);
        chatBoxUI.text = "";
    }


    List<GameObject> FindNearbyPlayers()
    {
        List<GameObject> nearbyPlayers = new List<GameObject>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 40f);
        
        foreach (Collider hitCollider in hitColliders)
        {
            if(hitCollider.gameObject.tag == "Player" && !nearbyPlayers.Contains(hitCollider.gameObject) && hitCollider.gameObject != this.gameObject)
            {
                nearbyPlayers.Add(hitCollider.gameObject);
            } 
        }
        return nearbyPlayers;
    }

    [Command]
    void CmdSendMessage(string message, GameObject target)
    {
        TargetRPCSendMessage(target.GetComponent<NetworkIdentity>().connectionToClient, message);
    }

    [TargetRpc]
    void TargetRPCSendMessage(NetworkConnection target, string message)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>().AddMessage(message);
    }

}
