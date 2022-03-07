using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

/// <summary>Required to open a canvas at the end of the game, 
/// which tells the player who won and who was the imposter.
/// </summary>
public class EndGameMenu : NetworkBehaviour
{
    /// <summary>Holds the canvas of the endgame canvas in the game.</summary>
    public Canvas endGameCanvas;
    /// <summary>Holds the TextesMeshPro-element for the title.</summary>
    public TextMeshProUGUI title;
    /// <summary>Holds the TextesMeshPro-element for the timer.</summary>
    public TextMeshProUGUI timerText;
    /// <summary>Holds the TextesMeshPro-element for the impostor names.</summary>
    public TextMeshProUGUI impostorText;
    /// <summary>The time, in seconds, until the client is automatically returned to the menu.</summary>
    public float time;
    /// <summary>Timer that counts down to 0 when the end game canvas is opened.</summary>
    float timer = 0;
    /// <summary>Is true when the timer starts counting down.</summary>
    bool timerStarted = false;

    /// <summary>
    /// Sets the timer to variable time, at the beginning.
    /// </summary>
    void Start()
    {
        timer = time;
    }

    /// <summary>
    /// Is executed only on the local player.
    /// Checks if the game is finished or who has won.
    /// Counts down the timer and return the player to the menu at the end.
    /// </summary>
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (FindObjectOfType<RoundManager>() != null)
        {
            Winner winner = FindObjectOfType<RoundManager>().hasWon;

            if (winner != Winner.None && !timerStarted)
            {
                timerStarted = true;
                toggleCanvas(winner);
            }

            if (timerStarted)
            {
                timer -= Time.deltaTime;
                timerText.text = ((int)timer).ToString();
                if (timer <= 0)
                {
                    NetworkManager.singleton.StopHost();
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    /// <summary>
    /// Opens the canvas for the end of the game.
    /// Sets the title to the winner or to the loser.
    /// Sets the impostor names.
    /// </summary>
    /// <param name="winner">Indicates who has won.</param>
    public void toggleCanvas(Winner winner)
    {
        if (winner == Winner.Team)
            title.text = "Team wins";
        else if (winner == Winner.Imposter)
            title.text = "Imposter wins";
        else title.text = "All lose";

        impostorText.text = FindObjectOfType<RoundManager>().imposterNames;
        endGameCanvas.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
