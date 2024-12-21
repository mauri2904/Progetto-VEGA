using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class DailyProductManager : MonoBehaviour
{
    public TextMeshProUGUI nomeText; 
    public TextMeshProUGUI descrizioneText; 
    public Image prodottoImage; 

    private FirebaseFirestore db; // riferimento al database Firestore
    private List<string> productIds = new List<string>(); // lista per memorizzare gli ID dei prodotti

    void Start()
    {
        // inizializza Firebase
        InitializeFirebase();
    }

    // inizializza Firebase e Firestore
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance; // connessione a Firestore
                Debug.Log("Firebase Firestore inizializzato correttamente!");
                // carica la lista di prodotti
                LoadProductIdsFromFirestore();
            }
            else
            {
                Debug.LogError("Errore nell'inizializzazione di Firebase.");
            }
        });
    }

    // carica tutti gli ID dei prodotti da Firestore
    void LoadProductIdsFromFirestore()
    {
        db.Collection("ProdottiDelGiorno") // nome della raccolta
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    QuerySnapshot snapshot = task.Result;

                    
                    productIds.Clear();

                    // salva tutti gli ID disponibili
                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        productIds.Add(document.Id);
                    }

                    // carica il prodotto del giorno
                    LoadDailyProduct();
                }
                else
                {
                    Debug.LogError("Errore nel caricamento degli ID dei prodotti.");
                }
            });
    }

    // carica il prodotto del giorno da Firestore
    void LoadDailyProduct()
    {
        if (productIds.Count == 0)
        {
            Debug.LogError("Nessun prodotto trovato nella collezione.");
            return;
        }

        // calcola l'indice del prodotto del giorno
        int dayOfMonth = DateTime.Now.Day;
        int productIndex = (dayOfMonth) % productIds.Count;

        // recupera l'ID del prodotto del giorno
        string dailyProductId = productIds[productIndex];

        // ottiene il prodotto dal documento corrispondente
        db.Collection("ProdottiDelGiorno")
            .Document(dailyProductId)
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    DocumentSnapshot document = task.Result;

                    // ottiene i dati del prodotto
                    string nome = document.GetValue<string>("Nome");
                    string descrizione = document.GetValue<string>("Descrizione");
                    string immagineUrl = document.GetValue<string>("Immagine");

                    // sostituisce il "\n" messo su Firebase con un vero salto di linea
                    descrizione = descrizione.Replace("\\n", "\n");

                    // mostra i dati del prodotto
                    nomeText.text = nome;
                    descrizioneText.text = descrizione;

                    // carica l'immagine
                    StartCoroutine(LoadImage(immagineUrl));
                }
                else
                {
                    Debug.LogError("Errore nel caricamento del prodotto del giorno o prodotto non trovato.");
                }
            });
    }

    // carica l'immagine e l' adatta alle dimensioni specifiche
    System.Collections.IEnumerator LoadImage(string url)
    {
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;

                // crea lo sprite dall'immagine scaricata
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                // imposta lo sprite nel componente Image
                prodottoImage.sprite = sprite;

                // adatta l'immagine per coprire esattamente la larghezza e altezza specificata
                prodottoImage.preserveAspect = false; 

                // imposta le dimensioni del RectTransform dell'immagine alle dimensioni specifiche
                RectTransform rt = prodottoImage.rectTransform;
                rt.sizeDelta = new Vector2(1200f, 1300f); // imposta le dimensioni precise

                prodottoImage.type = Image.Type.Simple; // si usa Simple per evitare distorsioni
            }
            else
            {
                Debug.LogError("Errore nel caricamento dell'immagine: " + url);
            }
        }
    }
}








