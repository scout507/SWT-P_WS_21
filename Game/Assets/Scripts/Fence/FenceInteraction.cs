using System.Collections.Generic;
using UnityEngine;
using Mirror;


/* created by: SWT-P_WS_21/22 */

/// <summary>
/// Responsible for player-fence interaction.
/// </summary>
public class FenceInteraction : NetworkBehaviour
{
    /// <summary>true = when a player has started the repair mechanism</summary>
    [SyncVar]
    public bool used = false;

    /// <summary>
    /// Needed to add the game objects of the planks in the Inspector.
    /// </summary>
    /// <typeparam name="GameObject">The individual planks</typeparam>
    /// <returns>List of game objects</returns>
    public List<GameObject> planksInspector = new List<GameObject>();

    /// <summary>The required time to repair a plank.</summary>
    public float timePerPlankInSeconds = 3;

    /// <summary>The player who uses the repair mechanism.</summary>
    public uint activPlayer = 0;

    /// <summary>
    /// Holds the game objects of the "planksInspector" list, as a SyncList.
    /// </summary>
    /// <typeparam name="GameObject">The individual planks</typeparam>
    /// <returns>List of game objects</returns>
    readonly SyncList<GameObject> planks = new SyncList<GameObject>();

    /// <summary>
    /// Holds the game objects of the "planks" list, where the planks are broken, as a SyncList.
    /// </summary>
    /// <typeparam name="GameObject">The individual broken planks</typeparam>
    /// <returns>List of game objects</returns>
    readonly SyncList<GameObject> brokenPlanks = new SyncList<GameObject>();

    /// <summary>
    /// Keeps the players network ids, which are in the action radius.
    /// </summary>
    /// <typeparam name="uint">The players' individual ids</typeparam>
    /// <returns>List of uint´s</returns>
    readonly SyncList<uint> players = new SyncList<uint>();

    /// <summary>Is counted up with time. </summary>
    float timer = 0;

    /// <summary>
    /// Executed only by the server.
    /// Transfers the individual game objects from the "planksInspector" list to the "planks" list.
    /// </summary>
    void Start()
    {
        if (isServer)
        {
            foreach (var item in planksInspector)
            {
                planks.Add(item);
            }
        }
    }

    /// <summary>
    /// Executed by client and server.
    /// When a player enters the action radius, he receives an output. 
    /// When he presses [E], he activates a command to activate the repair.
    /// The server repairs the fence only when a player starts this.
    /// The action is cancelled when the player performing the action moves away.
    /// In addition, it is always checked whether a new board is broken, 
    /// otherwise it is added to the "brokenPlanks" list.
    /// </summary>
    void Update()
    {
        if (isClient)
        {
            uint playerId =
                NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>().netId;

            if (players.Contains(playerId) && !used && brokenPlanks.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CmdActive(playerId);
                }
            }
        }

        if (isServer)
        {
            if (used)
            {
                timer += Time.deltaTime;

                if (timer >= timePerPlankInSeconds)
                {
                    timer = 0;
                    brokenPlanks[0].GetComponent<DestructableObject>().health =
                        brokenPlanks[0].GetComponent<DestructableObject>().maxHealth;
                    brokenPlanks.RemoveAt(0);
                }
            }

            if (players.Count == 0 || brokenPlanks.Count == 0 || !players.Contains(activPlayer))
            {
                used = false;
                activPlayer = 0;
            }

            foreach (var item in planks)
            {
                if (!item.GetComponent<MeshRenderer>().enabled && !brokenPlanks.Contains(item))
                    brokenPlanks.Add(item);
            }
        }
    }

    /// <summary>
    /// Adds the player to the "players" list.
    /// Only executed on the server.
    /// </summary>
    /// <param name="player">Contains the player's collider.</param>
    void OnTriggerEnter(Collider player)
    {
        if (
            player.tag == "Player"
            && !players.Contains(player.GetComponent<NetworkIdentity>().netId)
            && isServer
        )
        {
            players.Add(player.GetComponent<NetworkIdentity>().netId);
            TargetRpcSendMessage(player.GetComponent<NetworkIdentity>().connectionToClient, "Press [E] to repair.");
        }
    }

    /// <summary>
    /// Removes the player from the "players" list.
    /// Only executed on the server.
    /// </summary>
    /// <param name="player">Contains the player's collider.</param>
    void OnTriggerExit(Collider player)
    {
        if (player.tag == "Player" && isServer)
        {
            players.Remove(player.GetComponent<NetworkIdentity>().netId);
        }
    }

    /// <summary>
    /// Triggered by the player to tell the server that he has started the launch mechanism.
    /// This command does not require any authority from the player.
    /// </summary>
    /// <param name="netidPlayer">Requires the Network ID from the player.</param>
    [Command(requiresAuthority = false)]
    public void CmdActive(uint netidPlayer)
    {
        used = true;
        activPlayer = netidPlayer;
    }

    [TargetRpc]
    void TargetRpcSendMessage(NetworkConnection target, string message)
    {
        if (NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>()) NetworkClient.localPlayer.gameObject.GetComponent<Chatbox>().AddMessage(message);
    }
}
