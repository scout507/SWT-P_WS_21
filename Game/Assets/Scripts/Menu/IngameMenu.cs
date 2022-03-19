using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

/* created by: SWT-P_WS_21/22 */

/// <summary>Required for the menu in the game.</summary>
public class IngameMenu : NetworkBehaviour
{
    /// <summary>Holds the canvas of the menu in the game.</summary>
    public Canvas inGameMenuCanvas;

    /// <summary>Holds the volume slider of the UI.</summary>
    public Slider volumeSlider;

    /// <summary>Contains the user interface dropdown for the resolution of game</summary>
    public TMP_Dropdown dropdownResolution;

    /// <summary>Contains the user interface dropdown for the quality of game</summary>
    public TMP_Dropdown dropdownQuality;

    /// <summary>Keeps the toggel element out of the UI for fullscreen mode.</summary>
    public Toggle toggle;

    /// <summary>true, if the window is to be fullscreen.</summary>
    private bool isFullscreen;

    /// <summary>Holds all possible resolutions for the client.</summary>
    private Resolution[] resolutions;

    /// <summary>
    /// Sets all default values and call the method to generates the list for the resolutions.
    /// </summary>
    void Start()
    {
        toggle.isOn = Screen.fullScreen;
        isFullscreen = Screen.fullScreen;
        dropdownQuality.value = QualitySettings.GetQualityLevel();
        GenerateResolutionList();
    }

    /// <summary>
    /// Check if the player presses Escape to open the ingame menu.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (
            Input.GetKeyDown(KeyCode.Escape)
            && FindObjectOfType<RoundManager>().hasWon == Winner.None
        )
            ToggleMenu();
    }

    /// <summary>
    /// Opens/closes the ingame menu.
    /// Stops/starts the inputs for movement etc.
    /// Restores or hides the mouse cursor again.
    /// Sets the value for the volume slider in the ui.
    /// </summary>
    public void ToggleMenu()
    {
        NetworkIdentity player = connectionToServer.identity;
        inGameMenuCanvas.enabled = !inGameMenuCanvas.enabled;

        if (player.GetComponent<PlayerMovement>() != null)
        {
            player.GetComponent<PlayerMovement>().active = !inGameMenuCanvas.enabled;
            foreach (var item in player.GetComponents<ShootGun>())
                item.canInteract = !inGameMenuCanvas.enabled;
        }
        else
            player.GetComponent<Spectator>().active = !inGameMenuCanvas.enabled;

        if (player.GetComponent<AudioListener>())
            volumeSlider.value = AudioListener.volume;

        Cursor.visible = inGameMenuCanvas.enabled;
        Cursor.lockState = inGameMenuCanvas.enabled
            ? CursorLockMode.Confined
            : CursorLockMode.Locked;
    }

    /// <summary>
    /// Cancels the connection and takes the player back to the main menu.
    /// </summary>
    public void LoadMainMenu()
    {
        NetworkManager.singleton.StopHost();
    }

    /// <summary>
    /// Method to change the volume.
    /// </summary>
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    /// <summary>
    /// Changes the mode from window to fullscreen and the other way around.
    /// </summary>
    public void SetFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    /// <summary>
    /// Sets the quality of the game to the passed index.
    /// </summary>
    /// <param name="index">Given index for the quality. (identifiable in "project settings -> quality")</param>
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    /// <summary>
    /// Sets the resolution of the game to the passed index.
    /// </summary>
    /// <param name="index">Pass from the UI which option was selected.</param>
    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Generates for the UI all elements for the resolutions.
    /// </summary>
    private void GenerateResolutionList()
    {
        resolutions = Screen.resolutions;
        dropdownResolution.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        int index = 0;

        foreach (var item in resolutions)
        {
            options.Add(item.width + " x " + item.height + " @ " + item.refreshRate + " Hz");
            if (item.height == Screen.height && item.width == Screen.width)
                currentResolutionIndex = index;
            index++;
        }
        dropdownResolution.AddOptions(options);
        dropdownResolution.value = currentResolutionIndex;
    }
}
