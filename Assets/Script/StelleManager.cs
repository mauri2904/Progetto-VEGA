using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StelleManager : MonoBehaviour
{
    // lista dei bottoni delle stelle
    public List<Button> stelle;

    // immagini delle stelle vuote e piene
    public Sprite StellaVuota;
    public Sprite StellaPiena;

    // punteggio selezionato
    private int punteggio = 0;

    void Start()
    {
        
        for (int i = 0; i < stelle.Count; i++)
        {
            int index = i + 1; // l'indice dei punteggi va da 1 a 5
            stelle[i].onClick.RemoveAllListeners();  
            stelle[i].onClick.AddListener(() => ImpostaPunteggio(index));  
        }
    }

    public void ImpostaPunteggio(int valore)
    {
        punteggio = valore;

        // aggiorna le immagini delle stelle
        for (int i = 0; i < stelle.Count; i++)
        {
            Image immagineStella = stelle[i].GetComponent<Image>();
            
            if (immagineStella == null)
            {
                Debug.LogError("La stella " + (i + 1) + " non ha un componente Image!");
                continue;
            }

            // aggiorna la stella in base al punteggio
            if (i < punteggio) // se la stella è sotto il punteggio selezionato
            {
                immagineStella.sprite = StellaPiena;
                Debug.Log("Stella " + (i + 1) + " accesa.");
            }
            else // altrimenti lascia la stella vuota
            {
                immagineStella.sprite = StellaVuota;
                Debug.Log("Stella " + (i + 1) + " spenta.");
            }
        }

        Debug.Log("Punteggio selezionato: " + punteggio);
    }

    public int GetPunteggio()
    {
        return punteggio;
    }
}