using UnityEngine;
using UnityEngine.SceneManagement; // per caricare nuove scene

public class ButtonController2 : MonoBehaviour
{
    // questo metodo verr� chiamato quando il bottone viene cliccato
    public void OnButtonClick()
    {
       
        SceneManager.LoadScene("Open3");
        Debug.Log("Bottone cliccato! Procedi con la scena successiva.");
    }
}