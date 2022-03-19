using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>Required for the management of the main menu.</summary>
public class Menu : MonoBehaviour
{
    /// <summary>Holds the volume slider of the UI.</summary>
    public Slider volumeSlider;

    /// <summary>Contains the user interface dropdown for the resolution of game</summary>
    public TMP_Dropdown dropdownResolution;

    /// <summary>Contains the user interface dropdown for the quality of game</summary>
    public TMP_Dropdown dropdownQuality;

    /// <summary>Keeps the toggel element out of the UI for fullscreen mode.</summary>
    public Toggle toggleFullscreen;

    /// <summary>true, if the window is to be fullscreen.</summary>
    private bool isFullscreen;

    /// <summary>Holds all possible resolutions for the client.</summary>
    private Resolution[] resolutions;

    /// <summary>
    /// Sets all default values and calls the method to generates the list for the resolutions.
    /// Makes the mouse pointer and attracts it to the game window.
    /// </summary>
    void Start()
    {
        toggleFullscreen.isOn = Screen.fullScreen;
        isFullscreen = Screen.fullScreen;
        volumeSlider.value = FindObjectOfType<AudioSource>().volume;
        dropdownQuality.value = QualitySettings.GetQualityLevel();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        GenerateResolutionList();
    }

    /// <summary>
    /// Method to close the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
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
    /// Starts a local server to host the game.
    /// </summary>
    public void Host()
    {
        NetworkManager.singleton.StartHost();
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
