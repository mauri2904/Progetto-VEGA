using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using TMPro; 
using System.Collections.Generic; // per Dictionary
using System; // per DateTime

public class ProfileManager : MonoBehaviour
{
    // riferimenti agli oggetti UI
    public TMP_Text nomeText;        // campo per il nome
    public TMP_Text cognomeText;     // campo per il cognome
    public TMP_Text usernameText;    // campo per il nome utente
    public TMP_Text emailText;       // campo per l'email
    public TMP_Text creationDateText; // campo per la data di creazione

    private FirebaseAuth auth;       // autenticazione Firebase
    private FirebaseFirestore db;    // Firestore

    private bool dataReady = false;  // flag per sapere se i dati sono pronti
    private Dictionary<string, object> userData; // dizionario per i dati utente

    void Start()
    {
        // inizializza Firebase Authentication e Firestore
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // avvia il recupero dei dati del profilo
        GetUserProfile();
    }

    private void GetUserProfile()
    {
        FirebaseUser currentUser = auth.CurrentUser;

        if (currentUser != null)
        {
            string userId = currentUser.UserId;

            // recupera i dati del documento associato all'utente
            db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot documentSnapshot = task.Result;

                    if (documentSnapshot.Exists)
                    {
                        userData = documentSnapshot.ToDictionary();
                        dataReady = true; // segnala che i dati sono pronti
                    }
                    else
                    {
                        Debug.LogWarning("Documento utente non trovato.");
                    }
                }
                else
                {
                    Debug.LogError("Errore nel recupero dei dati: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("Utente non autenticato.");
        }
    }

    void Update()
    {
        if (dataReady)
        {
            // mostra i dati nell'interfaccia utente
            nomeText.text = userData["nome"].ToString();
            cognomeText.text = userData["cognome"].ToString();
            usernameText.text = userData["username"].ToString();
            emailText.text = userData["email"].ToString();

            // recupera e mostra la data di creazione
            if (userData.ContainsKey("creationDate"))
            {
                Timestamp creationTimestamp = (Timestamp)userData["creationDate"];
                DateTime creationDate = creationTimestamp.ToDateTime();
                creationDateText.text = creationDate.ToString("dd/MM/yyyy HH:mm");
            }
            else
            {
                creationDateText.text = "Data di Creazione: Non disponibile";
            }

            dataReady = false; // reset del flag per evitare aggiornamenti multipli
        }
    }
}





