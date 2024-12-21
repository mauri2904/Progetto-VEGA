using Firebase;
using Firebase.Auth;
using Firebase.Firestore; // importa Firestore
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement; // importa il gestore delle scene

public class FirebaseAuthManager : MonoBehaviour
{
    // riferimenti ai pannelli
    public GameObject loginPanel;      // pannello di login
    public GameObject registerPanel;   // pannello di registrazione

    // campi di input e testo di stato
    public TMP_InputField loginEmailField;     // campo email nel pannello di login
    public TMP_InputField loginPasswordField;  // campo password nel pannello di login
    public TMP_InputField registerEmailField;  // campo email nel pannello di registrazione
    public TMP_InputField registerPasswordField; // campo password nel pannello di registrazione
    public TMP_InputField registerNomeField;      // campo nome nel pannello di registrazione
    public TMP_InputField registerCognomeField;   // campo cognome nel pannello di registrazione
    public TMP_InputField registerUsernameField;  // campo nome utente nel pannello di registrazione
    public TMP_Text statusText;                // testo di stato per i messaggi di login
    public TMP_Text statusText2;               // testo di stato per i messaggi di registrazione

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    void Start()
    {
        // inizializza Firebase Authentication e Firestore
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        ShowLoginPanel(); // mostra inizialmente il pannello di login
    }

    // metodo per mostrare il pannello di registrazione
    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);  // nasconde il pannello di login
        registerPanel.SetActive(true); // mostra il pannello di registrazione
        statusText.text = "";          // resetta il testo di stato del login
        statusText2.text = "";         // resetta il testo di stato della registrazione
    }

    // metodo per mostrare il pannello di login
    public void ShowLoginPanel()
    {
        registerPanel.SetActive(false); // nasconde il pannello di registrazione
        loginPanel.SetActive(true);     // mostra il pannello di login
        statusText.text = "";           // resetta il testo di stato del login
        statusText2.text = "";          // resetta il testo di stato della registrazione
    }

    // funzione per avviare il processo di login
    public void LoginWithEmail()
    {
        string email = loginEmailField.text;
        string password = loginPasswordField.text;

        // avvia il login 
        SignInWithEmailAndPasswordAsync(email, password);
    }

    // funzione per avviare il processo di registrazione
    public void RegisterWithEmail()
    {
        string email = registerEmailField.text;
        string password = registerPasswordField.text;
        string nome = registerNomeField.text;
        string cognome = registerCognomeField.text;
        string username = registerUsernameField.text;

        // controlla che i campi non siano vuoti
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cognome) || string.IsNullOrEmpty(username))
        {
            statusText2.text = "Compila tutti i campi!";
            return;
        }

        // avvia la registrazione
        RegisterWithEmailAndPasswordAsync(email, password, nome, cognome, username);
    }

    // funzione per il login
    private async void SignInWithEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            // esegue il login con Firebase Authentication
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);

            if (authResult.User != null)
            {
                FirebaseUser user = authResult.User;

                // recupera i dati dell'utente da Firestore 
                var userData = await GetUserDataFromFirestore(user.UserId);
                string username = userData["username"].ToString();

                statusText.text = "Login riuscito! Benvenuto " + username;

                // attende un momento per mostrare il messaggio
                await Task.Delay(1500);

                // cambia scena a "MenuScene"
                SceneManager.LoadScene("MenuScene");
            }
        }
        catch (FirebaseException ex)
        {
            // mostra il messaggio di errore specifico
            AuthError errorCode = (AuthError)ex.ErrorCode;
            statusText.text = GetErrorMessage(errorCode);
        }
        catch (System.Exception ex)
        {
            statusText.text = "Errore durante il login: " + ex.Message;
        }
    }

    // funzione asincrona per la registrazione
    private async void RegisterWithEmailAndPasswordAsync(string email, string password, string nome, string cognome, string username)
    {
        try
        {
            // esegue la registrazione con Firebase Authentication
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            if (authResult.User != null)
            {
                FirebaseUser newUser = authResult.User;

                // salva i dati aggiuntivi in Firestore, includendo la data di creazione
                await SaveUserDataToFirestore(newUser.UserId, email, nome, cognome, username);

                statusText2.text = "Registrazione avvenuta con successo! Benvenuto " + username;

                // attende un momento per mostrare il messaggio
                await Task.Delay(1500);

                // torna automaticamente al pannello di login dopo la registrazione
                ShowLoginPanel();
            }
        }
        catch (FirebaseException ex)
        {
            // mostra il messaggio di errore specifico
            AuthError errorCode = (AuthError)ex.ErrorCode;
            statusText2.text = GetErrorMessage(errorCode);
        }
        catch (System.Exception ex)
        {
            statusText2.text = "Errore durante la registrazione: " + ex.Message;
        }
    }

    // funzione per salvare i dati aggiuntivi in Firestore, includendo la data di creazione
    private async Task SaveUserDataToFirestore(string userId, string email, string nome, string cognome, string username)
    {
        var userData = new
        {
            email = email,
            nome = nome,
            cognome = cognome,
            username = username,
            creationDate = Firebase.Firestore.Timestamp.GetCurrentTimestamp()  // aggiunge la data di creazione
        };

        await db.Collection("users").Document(userId).SetAsync(userData);
    }

    // funzione per ottenere i dati dell'utente da Firestore
    private async Task<Dictionary<string, object>> GetUserDataFromFirestore(string userId)
    {
        var snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ToDictionary();
        }
        else
        {
            throw new System.Exception("Dati utente non trovati, riprova");
        }
    }

    // metodo per ottenere un messaggio di errore basato sul codice di errore
    string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                return "L'indirizzo email non è valido.";
            case AuthError.EmailAlreadyInUse:
                return "Questa email è già in uso.";
            case AuthError.WeakPassword:
                return "La password è troppo debole.";
            case AuthError.NetworkRequestFailed:
                return "Errore di rete. Controlla la tua connessione a Internet.";
            default:
                return "Si è verificato un errore. Riprova.";
        }
    }
}
