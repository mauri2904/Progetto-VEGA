using UnityEngine;
using UnityEngine.SceneManagement;  

public class LoginManager : MonoBehaviour
{
    // metodo che verr� chiamato quando il bottone verr� premuto
    public void OnLoginButtonPressed()
    {
        // carica la scena "FeedbackScene"
        SceneManager.LoadScene("FeedbackScene");
    }
}
