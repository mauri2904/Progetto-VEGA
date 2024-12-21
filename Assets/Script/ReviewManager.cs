using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;

public class ReviewManager : MonoBehaviour
{
    public TMP_InputField reviewInputField; // campo di input per la recensione
    public Button submitButton; // pulsante per inviare la recensione
    public TMP_Text debugText; // campo di testo per mostrare i messaggi di debug

    private FirebaseFirestore db;
    private FirebaseAuth auth;

    void Start()
    {
        // nasconde il campo di debug all'avvio
        debugText.gameObject.SetActive(false);

        // inizializza Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase configurato correttamente!");
            }
            else
            {
                Debug.LogError("Errore di configurazione Firebase: " + task.Result);
            }
        });

        // assegna l'evento al pulsante
        submitButton.onClick.AddListener(SubmitReview);
    }

    public void SubmitReview()
    {
        // recupera il testo della recensione
        string reviewText = reviewInputField.text;

        if (string.IsNullOrEmpty(reviewText))
        {
            ShowDebugMessage("La recensione è vuota!");
            return;
        }

        // recupera l'utente autenticato
        FirebaseUser currentUser = auth.CurrentUser;

        if (currentUser == null)
        {
            ShowDebugMessage("Nessun utente autenticato!");
            return;
        }

        string userId = currentUser.UserId;

        // legge i dati dell'utente dalla collezione "users"
        db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                var userData = task.Result;

                // ottiene l' username
                string username = userData.ContainsField("username") ? userData.GetValue<string>("username") : "sconosciuto";

                // crea l'oggetto recensione
                var reviewData = new
                {
                    userId = userId,
                    username = username,
                    review = reviewText,
                    timestamp = Timestamp.GetCurrentTimestamp().ToDateTime()
                };

                // salva la recensione nella collezione "reviews"
                db.Collection("reviews").AddAsync(reviewData).ContinueWith(reviewTask =>
                {
                    if (reviewTask.IsCompletedSuccessfully)
                    {
                        ShowDebugMessage("Recensione salvata con successo!");
                        reviewInputField.text = ""; // resetta il campo
                    }
                    else
                    {
                        ShowDebugMessage("Errore durante il salvataggio della recensione.");
                    }
                });
            }
            else
            {
                ShowDebugMessage("Impossibile trovare i dati dell'utente.");
            }
        });
    }

    // metodo per mostrare un messaggio di debug
    void ShowDebugMessage(string message)
    {
        if (debugText != null)
        {
            debugText.text = message;

            // rende visibile il testo
            debugText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Il DebugText non è impostato!");
        }
    }
}

