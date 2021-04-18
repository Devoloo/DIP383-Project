using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameControl : MonoBehaviour
{
    // Object
    [SerializeField]
    private Target targetObject;
    [SerializeField]
    private InputField usernameInput, passwordInput;

    // Panel/Button
    [SerializeField]
    private GameObject loginPanel, welcomePanel, pausePanel, resultsPanel, playButton;

    [SerializeField]
    private Text getReadyText, targetsHitText, shotsFiredText, accuracyText, actualScoreText, wrongPasswordText;

    [SerializeField]
    private Texture2D cursorTexture;

    // Visible panel/text values
    public static bool loginPanelActive, welcomePanelActive, pausePanelActive, resultsPanelActive, getReadyTextActive;

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

        // Show loginPanel the first time
        loginPanelActive = true;
        loginPanel.SetActive(loginPanelActive);

        // Hide welcomePanel
        welcomePanelActive = false;
        welcomePanel.SetActive(welcomePanelActive);

        // Hide resultsPanel
        resultsPanelActive = false;
        resultsPanel.SetActive(resultsPanelActive);

        // Hide getReadyText
        getReadyTextActive = false;
        getReadyText.gameObject.SetActive(getReadyTextActive);

        // Hide pausePanel
        pausePanelActive = false;
        pausePanel.SetActive(pausePanelActive);

        // Hide actualScoreText
        actualScoreText.gameObject.SetActive(false);
        actualScoreText.text = "Score";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !welcomePanelActive && !getReadyTextActive && !resultsPanelActive && !pausePanelActive)
            shotsFired++;
        if (Input.GetKeyDown("escape") && !welcomePanelActive && !resultsPanelActive && !getReadyTextActive)
            PauseGame();

        // Always update score ?
        UpdateScoreValue();
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

        // Show actualScoreText
        actualScoreText.gameObject.SetActive(true);

        for (int i = targetsAmount; i > 0; i--)
        {
            targetRandomPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f));
            Instantiate(targetObject, targetRandomPos, Quaternion.identity);

            yield return new WaitForSeconds(difficulty);
        }

        // Hide actualScoreText
        actualScoreText.gameObject.SetActive(false);

        // Show resultsPanel
        resultsPanelActive = true;
        resultsPanel.SetActive(resultsPanelActive);

        targetsHitText.text = "Targets hit: " + targetsHit + "/" + targetsAmount;
        shotsFiredText.text = "Shots fired: " + shotsFired;

        accuracy = ((targetsHit / (float)shotsFired) - ((targetsAmount - targetsHit) / (float)targetsAmount)) * 100f;
        accuracyText.text = "Accuracy: " + accuracy.ToString("N2") + "%";
    }

    private void UpdateScoreValue()
    {
        actualScoreText.text = "Score: " + targetsHit + "/" + targetsAmount;
    }
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    
    
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    public void ConnectGame()
    {
        // REGISTER...
        string usernamevalue = usernameInput.text;
        string passwordvalue = passwordInput.text;

        // Check if already exist
        string[] actualdata = File.ReadAllLines("Assets/Data/datasheet.txt");

        foreach (string s in actualdata)
        {
            // if user don't exist create it
            if (!s.Contains(usernamevalue))
            {
                /* Create data
                string datavalues = ";1" + usernamevalue + ";2" + passwordvalue + ";3" + ";4";
                // Read file
                using StreamWriter sw = File.AppendText("Assets/Data/datasheet.txt");
                // Write on it
                sw.WriteLine(datavalues);*/

                print("don't exist, create");
                break;
            }
            // if not check password
            else
            {
                // if password is good
                if (s.Contains(passwordvalue))
                {
                    /* Hide loginPanel
                    loginPanelActive = false;
                    loginPanel.SetActive(loginPanelActive);

                    // Show welcomePanel
                    welcomePanelActive = true;
                    welcomePanel.SetActive(welcomePanelActive);*/
                    print("password is good, launch");
                    break;
                }
                // is not then show an error message
                else
                {
                    //wrongPasswordText.text = "Wrong password... Retry";
                    print("password not good retry");
                    break;
                }
            }
        }


        // Hide loginPanel
        loginPanelActive = false;
        loginPanel.SetActive(loginPanelActive);

        // Show welcomePanel
        welcomePanelActive = true;
        welcomePanel.SetActive(welcomePanelActive);
    }
    
    public void StartGame()
    {
        // Initial values
        targetsAmount = 10; // MUST CHANGE
        shotsFired = 0;
        targetsHit = 0;
        accuracy = 0f;

        // Add game
        nbGame++;

        // Hide loginPanel and welcomePanel and resultsPanel
        loginPanelActive = false;
        loginPanel.SetActive(loginPanelActive);
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
        // Hide actualScoreText
        actualScoreText.gameObject.SetActive(false);

        // Show pausePanel
        pausePanelActive = true;
        pausePanel.SetActive(pausePanelActive);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        // Show actualScoreText
        actualScoreText.gameObject.SetActive(true);

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

        // Hide actualScoreText
        actualScoreText.gameObject.SetActive(false);
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
