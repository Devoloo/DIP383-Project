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

    // Data location
    private string datalocation = @"C:\Users\macha\Desktop\DIP383-Project\unityData\data.txt";

    // Player data
    private string username, password;


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
        if (Input.GetMouseButtonDown(0) && !loginPanelActive && !welcomePanelActive && !getReadyTextActive && !resultsPanelActive && !pausePanelActive)
            shotsFired++;
        if (Input.GetKeyDown("escape") && !loginPanelActive && !welcomePanelActive && !resultsPanelActive && !getReadyTextActive)
            PauseGame();

        // Connect game by using Return
        if (Input.GetKeyDown(KeyCode.Return) && username != "" && password != "")
            ConnectGame();

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

        // Calcute the new player data
        // data sheet
        string[] dataSheet = File.ReadAllLines(datalocation);
        int pos = 0;
        string line = dataSheet[pos];

        for (pos = 0; pos < dataSheet.Length; pos++)
        {
            line = dataSheet[pos];
            if (line.Contains(";1" + username + ";2" + password + ";3"))
            {
                // game value
                int gamepos1 = line.IndexOf(";3") + ";3".Length;
                int gamepos2 = line.LastIndexOf(";4");
                string gamevalueString = line.Substring(gamepos1, gamepos2 - gamepos1);
                int gamevalue;
                // check if the value is not x
                if (gamevalueString == "x")
                    gamevalue = 0;
                else
                    gamevalue = System.Int16.Parse(gamevalueString);

                // accuracy value
                int accuracypos1 = line.IndexOf(";4") + ";4".Length;
                string accuracyvalueString = line.Substring(accuracypos1);
                int accuracyvalue;
                // check if the value is not x
                if (accuracyvalueString == "x")
                    accuracyvalue = 0;
                else
                    accuracyvalue = System.Int16.Parse(accuracyvalueString);
                
                // new data
                line = ";1" + username + ";2" + password + ";3" + (gamevalue + 1) + ";4" + ((accuracyvalue + accuracy) / 2);
                break;
            }
        }

        dataSheet[pos] = line;
        File.WriteAllLines(datalocation, dataSheet);
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
        string[] actualdata = File.ReadAllLines(datalocation);
        int numberdata = actualdata.Length;

        foreach (string s in actualdata)
        {
            // -1 number of data
            numberdata--;

            // username exist and password is good
            if (s.Contains(";1" + usernamevalue + ";2") && s.Contains(";2" + passwordvalue + ";3"))
            {
                //print("username+password good");

                // Update username and password data
                username = usernamevalue;
                password = passwordvalue;

                // Hide loginPanel
                loginPanelActive = false;
                loginPanel.SetActive(loginPanelActive);

                // Show welcomePanel
                welcomePanelActive = true;
                welcomePanel.SetActive(welcomePanelActive);
                break;
            }
            // username exist but password is not good
            // so we continue search
            if (s.Contains(";1" + usernamevalue + ";2") && !s.Contains(";2" + passwordvalue + ";3"))
            {
                // print("username good but password not");
                continue;
            }
            // username doesn't exist
            // so we continue search
            if (!s.Contains(";1" + usernamevalue + ";2"))
            {
                // print("username not good");
                continue;
            }
        }

        // if number of data == 0 then we don't find the username/password so we reset password text and add a new text
        if (numberdata == 0)
        {
            // change passwordInput text
        }
    }

    public void CreateAccount()
    {
        // Check if username already exist
        string usernamevalue = usernameInput.text;
        string passwordvalue = passwordInput.text;

        string[] actualdata = File.ReadAllLines(datalocation);
        int numberdata = actualdata.Length;

        foreach (string s in actualdata)
        {
            // if username already exsit then exit
            if (s.Contains(";1" + usernamevalue + ";2"))
                break;

            // -1 number of data
            numberdata--;
        }

        if (numberdata == 0)
        {
            using StreamWriter file = File.AppendText(datalocation);
            file.Write(";1" + usernamevalue + ";2" + passwordvalue + ";3x" + ";4x" + "\n");

            // Update username and password data
            username = usernamevalue;
            password = passwordvalue;

            // Hide loginPanel
            loginPanelActive = false;
            loginPanel.SetActive(loginPanelActive);

            // Show welcomePanel
            welcomePanelActive = true;
            welcomePanel.SetActive(welcomePanelActive);
        }
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

    public void ReturnLoginScreen()
    {
        // Hide welcomePanel
        welcomePanelActive = false;
        welcomePanel.SetActive(welcomePanelActive);
        // Show loginPanel
        loginPanelActive = true;
        loginPanel.SetActive(loginPanelActive);
        // Reset input values
        usernameInput.text = "";
        passwordInput.text = "";
    }

    public void QuitGame()
    {
        Application.Quit();
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
