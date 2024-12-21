using UnityEngine;
using UnityEngine.SceneManagement; // per caricare nuove scene

public class ButtonController3 : MonoBehaviour
{
    // questo metodo verrà chiamato quando il bottone viene cliccato
    public void OnButtonClick()
    {

        SceneManager.LoadScene("LoginScene");
        Debug.Log("Bottone cliccato! Procedi con la scena successiva.");
    }
}

