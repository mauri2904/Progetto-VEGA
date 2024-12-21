using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;
using TMPro;

public class HomeManager : MonoBehaviour
{
    // pannelli della Home
    public GameObject homePanel1;
    public GameObject homePanel2;

    // pannello del Profilo
    public GameObject profilePanel;

    // pannello del Quiz
    public GameObject quizPanel;

    // risultati del Quiz
    public TMP_Text resultText;  // risultato del quiz

    // toggle per le risposte del quiz
    public Toggle[] question1Toggles;
    public Toggle[] question2Toggles;
    public Toggle[] question3Toggles;
    public Toggle[] question4Toggles;

    // pulsanti della Home
    public Button profileButton;
    public Button scannerButton;
    public Button feedbackButton;

    // pulsante del quiz per tornare alla home
    public Button returnToHomeButtonFromQuiz;

    // pulsante del profilo per tornare alla home
    public Button returnToHomeButtonFromProfile;

    void Start()
    {
        // inizialmente mostra solo i due pannelli della home
        homePanel1.SetActive(true);
        homePanel2.SetActive(true);
        profilePanel.SetActive(false);
        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);  // risultato inizialmente nascosto

        
        profileButton.onClick.AddListener(OpenProfilePanel);
        scannerButton.onClick.AddListener(OpenScannerScene);
        feedbackButton.onClick.AddListener(OpenQuizPanel);
        returnToHomeButtonFromQuiz.onClick.AddListener(ReturnToHomeFromQuiz);
        returnToHomeButtonFromProfile.onClick.AddListener(ReturnToHomeFromProfile);
    }

    // metodo per aprire il pannello del profilo
    public void OpenProfilePanel()
    {
        
        homePanel1.SetActive(false);
        homePanel2.SetActive(false);
        profilePanel.SetActive(true);
    }

    // metodo per tornare alla home dal profilo
    public void ReturnToHomeFromProfile()
    {
        
        homePanel1.SetActive(true);
        homePanel2.SetActive(true);
        profilePanel.SetActive(false);
    }

    // metodo per caricare la scena dello scanner
    public void OpenScannerScene()
    {
        // carica la scena dello scanner
        SceneManager.LoadScene("ScannerScene");
    }

    // metodo per aprire il pannello del quiz
    public void OpenQuizPanel()
    {
       
        homePanel1.SetActive(false);
        homePanel2.SetActive(false);
        quizPanel.SetActive(true);

       
        ResetQuiz();
    }

    // metodo per tornare alla home dal quiz
    public void ReturnToHomeFromQuiz()
    {
       
        homePanel1.SetActive(true);
        homePanel2.SetActive(true);
        quizPanel.SetActive(false);

        
        ResetQuiz();
    }

    // metodo per resettare il quiz quando si ritorna alla home o si apre il quiz
    public void ResetQuiz()
    {
        // Deselezione di tutte le risposte dai toggle delle domande
        foreach (var toggle in question1Toggles)
        {
            toggle.isOn = false;
        }

        foreach (var toggle in question2Toggles)
        {
            toggle.isOn = false;
        }

        foreach (var toggle in question3Toggles)
        {
            toggle.isOn = false;
        }

        foreach (var toggle in question4Toggles)
        {
            toggle.isOn = false;
        }

       
        resultText.gameObject.SetActive(false);
    }
}
