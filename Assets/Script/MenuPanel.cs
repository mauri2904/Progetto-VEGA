using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; // per caricare una scena

public class PanelManager : MonoBehaviour
{
    // riferimenti ai pannelli e ai pulsanti
    public GameObject profilePanel;  // pannello da mostrare
    public Button buttonProfile;     // pulsante per il profilo
    public Button buttonScanner;     // pulsante per lo scanner
    public Button buttonFeedback;    // pulsante per il feedback

    void Start()
    {
        // imposta l'azione del pulsante Profile
        buttonProfile.onClick.AddListener(ToggleProfilePanel);

        // imposta l'azione del pulsante Scanner
        buttonScanner.onClick.AddListener(LoadScannerScene);

        // imposta l'azione del pulsante Feedback
        buttonFeedback.onClick.AddListener(LoadFeedbackScene);

        // il pannello del profilo è inizialmente nascosto
        profilePanel.SetActive(false);
    }

    // funzione per attivare o disattivare il pannello del profilo
    public void ToggleProfilePanel()
    {
        // per mostrare/nascondere il pannello del profilo
        profilePanel.SetActive(!profilePanel.activeSelf);
    }

    // funzione per caricare la scena dello scanner
    public void LoadScannerScene()
    {
        // carica la scena "ScannerScene"
        SceneManager.LoadScene("ScannerScene");
    }

    // funzione per caricare la scena del feedback
    public void LoadFeedbackScene()
    {
        // carica la scena "FeedbackScene"
        SceneManager.LoadScene("FeedbackScene");
    }
}


