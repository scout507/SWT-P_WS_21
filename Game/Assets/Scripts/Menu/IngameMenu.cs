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
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<RoundManager>().hasWon == Winner.None)
            toggleMenu();
    }

    /// <summary>
    /// Opens the ingame menu.
    /// </summary>
    public void toggleMenu()
    {
        menuCanvas.enabled = !menuCanvas.enabled;
        Cursor.visible = menuCanvas.enabled;
        Cursor.lockState = menuCanvas.enabled ? CursorLockMode.Confined : CursorLockMode.Locked;
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
