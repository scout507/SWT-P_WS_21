using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>Required for the menu in the game.</summary>
public class IngameMenu : NetworkBehaviour
{
    /// <summary>Holds the canvas of the menu in the game.</summary>
    public Canvas menuCanvas;

    /// <summary>
    /// Check if the player presses Escape to open the ingame menu.
    /// </summary>
    void Update()
    {
        if (isLocalPlayer)
            if (Input.GetKeyDown(KeyCode.Escape))
                toggleMenu();
    }

    /// <summary>
    /// Opens the ingame menu.
    /// </summary>
    public void toggleMenu()
    {
        menuCanvas.enabled = !menuCanvas.enabled;
    }

    /// <summary>
    /// Cancels the connection and takes the player back to the main menu.
    /// </summary>
    public void loadMainMenu()
    {
        NetworkManager.singleton.StopHost();
        SceneManager.LoadScene(0);
    }
}
