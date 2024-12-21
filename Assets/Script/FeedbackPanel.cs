using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMenuScene : MonoBehaviour
{
    // metodo da assegnare al pulsante
    public void GoToMenu()
    {
        // carica la scena "MenuScene"
        SceneManager.LoadScene("MenuScene");
    }
}

