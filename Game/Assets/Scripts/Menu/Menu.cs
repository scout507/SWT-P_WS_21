using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>Required for the management of the main menu.</summary>
public class Menu : MonoBehaviour
{
    /// <summary>Holds the volume slider of the UI.</summary>
    public Slider volumeSlider;

    /// <summary>true, if the window is to be fullscreen.</summary>
    bool isFullscreen;

    /// <summary>Holds all possible resolutions for the client.</summary>
    Resolution[] resolutions;

    /// <summary>Contains the user interface dropdown for the quality of game</summary>
    public TMP_Dropdown dropdown;

    /// <summary>Keeps the toggel element out of the UI for fullscreen mode.</summary>
    public Toggle toggle;

    /// <summary>
    /// Sets all default values and generates the list for the resolutions.
    /// </summary>
    void Start()
    {
        toggle.isOn = Screen.fullScreen;
        isFullscreen = Screen.fullScreen;
        volumeSlider.value = FindObjectOfType<Soundmanager>().volume;
        resolutions = Screen.resolutions;
        dropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        int index = 0;

        foreach (var item in resolutions)
        {
            options.Add(item.width + " x " + item.height);
            if (
                item.height == Screen.currentResolution.height
                && item.width == Screen.currentResolution.width
            )
                currentResolutionIndex = index;
            index++;
        }

        dropdown.AddOptions(options);
        dropdown.value = currentResolutionIndex;
    }

    /// <summary>
    /// Method to close the game
    /// </summary>
    public void exitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Method to change the volume for the sound manager.
    /// </summary>
    public void changeVolume()
    {
        FindObjectOfType<Soundmanager>().volume = volumeSlider.value;
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
    /// <param name="index">From 0 to 5; 0 => very bad; 5 => ultra settings</param>
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
    /// Starts a local server to host the game.
    /// </summary>
    public void host()
    {
        (NetworkManager.singleton as NetworkManagerLobby).StartHost();
    }
}
