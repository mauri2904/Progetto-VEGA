using UnityEngine;
using UnityEngine.SceneManagement;  

public class LoginManager : MonoBehaviour
{
    // metodo che verrà chiamato quando il bottone verrà premuto
    public void OnLoginButtonPressed()
    {
        // carica la scena "FeedbackScene"
        SceneManager.LoadScene("FeedbackScene");
    }
}
