using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject, welcomePanel, pausePanel, resultsPanel, playButton;

    [SerializeField]
    private Texture2D cursorTexture;

    [SerializeField]
    private Text getReadyText, targetsHitText, shotsFiredText, accuracyText;

    // Visible panel/text values
    public static bool welcomePanelActive, pausePanelActive, resultsPanelActive, getReadyTextActive;

    // Vector2 values
    private Vector2 cursorHotspot, mousePos, targetRandomPos;

    // Int/Float values
    public static int targetsHit;
    public static float difficulty;
    private int shotsFired, targetsAmount, nbGame;
    private float accuracy;


    /*-------------------------------------------------------------------------------------------------------------------------------*/
    // Start is called before the first frame update
    void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);

        // Initial value
        nbGame = 0;
        // Can change in gamemode functions
        difficulty = 1f;

        // Show welcome if 1st game if not then hide
        if (nbGame > 1)
        {
            welcomePanelActive = false;
            resultsPanelActive = true;
        }
        else
        {
            welcomePanelActive = true;
            resultsPanelActive = false;
        }

        // welcomePanel
        welcomePanel.SetActive(welcomePanelActive);

        // resultsPanel
        resultsPanel.SetActive(resultsPanelActive);

        // Hide getReady
        getReadyTextActive = false;
        getReadyText.gameObject.SetActive(getReadyTextActive);
        // Hide pausePanel
        pausePanelActive = false;
        pausePanel.SetActive(pausePanelActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !resultsPanelActive && !getReadyTextActive && !welcomePanelActive && !pausePanelActive)
            shotsFired++;
        if (Input.GetKeyDown("escape") && !welcomePanelActive && !resultsPanelActive && !getReadyTextActive)
            PauseGame();
    }
    /*-------------------------------------------------------------------------------------------------------------------------------*/


    /*-------------------------------------------------------------------------------------------------------------------------------*/
    private IEnumerator GetReady()
    {
        for (int i = 3; i > 0; i--)
        {
            getReadyText.text = "Get Ready!\n" + i.ToString();
            yield return new WaitForSeconds(1f);
        }

        getReadyText.text = "Go!";
        yield return new WaitForSeconds(1f);

        StartCoroutine("SpawnTargets");
    }

    private IEnumerator SpawnTargets()
    {
        // Hide getReady
        getReadyTextActive = false;
        getReadyText.gameObject.SetActive(getReadyTextActive);

        for (int i = targetsAmount; i > 0; i--)
        {
            targetRandomPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f));
            Instantiate(targetObject, targetRandomPos, Quaternion.identity);

            yield return new WaitForSeconds(difficulty);
        }

        // Show resultsPanel
        resultsPanelActive = true;
        resultsPanel.SetActive(resultsPanelActive);

        targetsHitText.text = "Targets hit: " + targetsHit + "/" + targetsAmount;
        shotsFiredText.text = "Shots fired: " + shotsFired;

        accuracy = ((targetsHit / (float) shotsFired) - ((targetsAmount - targetsHit) / (float)targetsAmount)) * 100f;
        accuracyText.text = "Accuracy: " + accuracy.ToString("N2") + "%";
    }
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    
    
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    public void StartGame()
    {
        // Initial values
        targetsAmount = 10; // MUST CHANGE
        shotsFired = 0;
        targetsHit = 0;
        accuracy = 0f;

        // Add game
        nbGame++;

        // Hide resultsPanel and welcomePanel
        resultsPanelActive = false;
        resultsPanel.SetActive(resultsPanelActive);
        welcomePanelActive = false;
        welcomePanel.SetActive(welcomePanelActive);
        // Show getReady
        getReadyTextActive = true;
        getReadyText.gameObject.SetActive(getReadyTextActive);

        StartCoroutine("GetReady");
    }

    public void PauseGame()
    {
        // Show pausePanel
        pausePanelActive = true;
        pausePanel.SetActive(pausePanelActive);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        // Hide pausePanel
        pausePanelActive = false;
        pausePanel.SetActive(pausePanelActive);
        Time.timeScale = 1f;
    }

    public void ReturnMenu()
    {
        // No need to stop coroutines because they are already finish
        // Reset game number
        nbGame = 0;

        // Hide pausePanel and resultsPanel
        pausePanelActive = resultsPanelActive = false;
        pausePanel.SetActive(pausePanelActive);
        resultsPanel.SetActive(resultsPanelActive);
        // Show welcomePanel
        welcomePanelActive = true;
        welcomePanel.SetActive(welcomePanelActive);
    }
    /*-------------------------------------------------------------------------------------------------------------------------------*/


    /*-------------------------------------------------------------------------------------------------------------------------------*/
    public void gamemodeEasy()
    {
        difficulty = 1.5f;
        Color green = new Color(0f, 1f, 0f);
        playButton.GetComponent<Image>().color = green;
    }
    public void gamemodeMedium()
    {
        difficulty = 1f;
        Color orange = new Color(1f, 0.4f, 0f);
        playButton.GetComponent<Image>().color = orange;
    }
    public void gamemodeHard()
    {
        difficulty = 0.5f;
        Color red = new Color(1f, 0f, 0f);
        playButton.GetComponent<Image>().color = red;
    }
    /*-------------------------------------------------------------------------------------------------------------------------------*/
}
