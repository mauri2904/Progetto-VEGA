using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class ScannerPanel : MonoBehaviour
{
    // riferimenti ai pulsanti e al pannello
    public Button showButton;   // pulsante per mostrare il pannello
    public Button hideButton;   // pulsante per nascondere il pannello
    public Button backButton;   // pulsante per tornare al menu
    public GameObject panel;    //  pannello da mostrare/nascondere

    public void Start()
    {
        // pannello nascosto inizialmente
        panel.SetActive(false);

        
        showButton.onClick.AddListener(ShowPanel);
        hideButton.onClick.AddListener(HidePanel);
        backButton.onClick.AddListener(BackToMenu); 
    }

    // funzione per mostrare il pannello
    public void ShowPanel()
    {
        panel.SetActive(true); // mostra il pannello
    }

    // funzione per nascondere il pannello
    public void HidePanel()
    {
        panel.SetActive(false); // nasconde il pannello
    }

    // funzione per tornare al menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene"); // carica la scena MenuScene
    }
}


