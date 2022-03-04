using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class EndGameMenu : NetworkBehaviour
{
    /// <summary>Holds the canvas of the endgame canvas in the game.</summary>
    public Canvas endGameCanvas;
    public TextMeshProUGUI title;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI impostorText;

    public float time;
    float timer = 0;
    bool timerStarted = false;

    void Start()
    {
        timer = time;
    }

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

    public void toggleCanvas(Winner winner)
    {
        if (winner == Winner.Team)
            title.text = "Team wins";
        else
            title.text = "Imposter wins";

        impostorText.text = FindObjectOfType<RoundManager>().imposterNames;
        endGameCanvas.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
