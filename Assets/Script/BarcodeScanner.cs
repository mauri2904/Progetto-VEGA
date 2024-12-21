using UnityEngine;
using UnityEngine.UI; 
using ZXing; // per leggere i codici a barre
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro; 
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;


public class BarcodeFirestoreIntegration : MonoBehaviour
{
    // scanner di codici a barre
    public RawImage cameraDisplay; // mostrerà il feed della fotocamera
    public GameObject scannerUI; // UI dello scanner
    public GameObject productInfoPanel; // pannello delle informazioni del prodotto

    private WebCamTexture webcamTexture;
    private IBarcodeReader barcodeReader;

    public static string scannedCode; // codice scansionato

    // campi per il prodotto principale
    public TextMeshProUGUI brandText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI originText;
    public TextMeshProUGUI ingredients;
    public TextMeshProUGUI productionTypeText;
    public TextMeshProUGUI environmentalImpactText;
    public TextMeshProUGUI nutritionalValuesText;
    public Image productImage;
    public Image ecoScoreImage;

    // campi per le alternative
    public TextMeshProUGUI brandTextAlternative1;
    public TextMeshProUGUI descriptionTextAlternative1;
    public Image alternativeImage1;
    public Image ecoScoreImage1;

    public TextMeshProUGUI brandTextAlternative2;
    public TextMeshProUGUI descriptionTextAlternative2;
    public Image alternativeImage2;
    public Image ecoScoreImage2;

    public GameObject alternativePanel;
    public GameObject mainPanel;

    private FirebaseFirestore db;

    void Start()
    {
        // inizializza lo scanner UI
        if (scannerUI != null) scannerUI.SetActive(true);
        if (productInfoPanel != null) productInfoPanel.SetActive(false);

        // configura la webcam
        webcamTexture = new WebCamTexture();
        cameraDisplay.texture = webcamTexture;
        AdjustCameraOrientation();
        webcamTexture.Play();

        // configura il lettore di codici a barre
        barcodeReader = new BarcodeReader
        {
            AutoRotate = true,
            Options =
            {
                TryHarder = true,
                PossibleFormats = new System.Collections.Generic.List<BarcodeFormat>
                {
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.EAN_13
                }
            }
        };

        // configura Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase inizializzato correttamente!");
            }
            else
            {
                Debug.LogError("Errore nell'inizializzazione di Firebase.");
            }
        });
    }

    void Update()
    {
        if (webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            try
            {
                var pixels = new Color32[webcamTexture.width * webcamTexture.height];
                webcamTexture.GetPixels32(pixels);
                var result = barcodeReader.Decode(pixels, webcamTexture.width, webcamTexture.height);

                if (result != null)
                {
                    scannedCode = result.Text;
                    Debug.Log("Codice trovato: " + scannedCode);

                    if (scannerUI != null) scannerUI.SetActive(false);
                    if (productInfoPanel != null) productInfoPanel.SetActive(true);

                    ReadProductData(scannedCode);
                }
            }
            catch
            {
                Debug.LogWarning("Errore durante la scansione.");
            }
        }
    }

    private void AdjustCameraOrientation()
    {
        cameraDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, -90);
        cameraDisplay.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
    }

    private void ReadProductData(string productId)
    {
        db.Collection("Prodotti").Document(productId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot document = task.Result;
                if (document.Exists)
                {
                    var productData = document.ToDictionary();
                    UpdateProductUI(productData);
                    LoadAlternatives(productId);
                }
                else
                {
                    Debug.LogError("Prodotto non trovato.");
                }
            }
            else
            {
                Debug.LogError("Errore nel recupero dei dati da Firestore.");
            }
        });
    }

    private void UpdateProductUI(System.Collections.Generic.Dictionary<string, object> productData)
    {
        if (productData.ContainsKey("Marca"))
            brandText.text = productData["Marca"].ToString().Replace("\\n", "\n");
        if (productData.ContainsKey("Nome"))
            nameText.text = productData["Nome"].ToString().Replace("\\n", "\n");
        if (productData.ContainsKey("Provenienza"))
            originText.text = productData["Provenienza"].ToString().Replace("\\n", "\n");
        if (productData.ContainsKey("Ingredienti"))
            ingredients.text = productData["Ingredienti"].ToString().Replace("\\n", "\n");
        if (productData.ContainsKey("Tipo di produzione"))
            productionTypeText.text = productData["Tipo di produzione"].ToString().Replace("\\n", "\n");
        if (productData.ContainsKey("Impatto ambientale"))
            environmentalImpactText.text = productData["Impatto ambientale"].ToString().Replace("\\n", "\n");
        if (productData.ContainsKey("Valori nutrizionali"))
            nutritionalValuesText.text = productData["Valori nutrizionali"].ToString().Replace("\\n", "\n");

        if (productData.ContainsKey("Immagine"))
            StartCoroutine(LoadImageFromUrl(productData["Immagine"].ToString(), productImage));
        if (productData.ContainsKey("EcoScore"))
            StartCoroutine(LoadImageFromUrl(productData["EcoScore"].ToString(), ecoScoreImage));
    }

    private void LoadAlternatives(string productId)
    {
        db.Collection("Prodotti").Document(productId).Collection("Alternative").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                var alternativesSnapshot = task.Result;

                if (alternativesSnapshot.Count > 0)
                    LoadAlternativeData(alternativesSnapshot[0], 1);

                if (alternativesSnapshot.Count > 1)
                    LoadAlternativeData(alternativesSnapshot[1], 2);
            }
            else
            {
                Debug.LogError("Errore nel recupero delle alternative.");
            }
        });
    }

    private void LoadAlternativeData(DocumentSnapshot alternativeDocument, int alternativeIndex)
    {
        var alternativeData = alternativeDocument.ToDictionary();

        if (alternativeIndex == 1)
        {
            if (alternativeData.ContainsKey("Marca"))
                brandTextAlternative1.text = alternativeData["Marca"].ToString().Replace("\\n", "\n");
            if (alternativeData.ContainsKey("Descrizione"))
                descriptionTextAlternative1.text = alternativeData["Descrizione"].ToString().Replace("\\n", "\n");
            if (alternativeData.ContainsKey("Immagine"))
                StartCoroutine(LoadImageFromUrl(alternativeData["Immagine"].ToString(), alternativeImage1));
            if (alternativeData.ContainsKey("EcoScore1"))
                StartCoroutine(LoadImageFromUrl(alternativeData["EcoScore1"].ToString(), ecoScoreImage1));
        }
        else if (alternativeIndex == 2)
        {
            if (alternativeData.ContainsKey("Marca"))
                brandTextAlternative2.text = alternativeData["Marca"].ToString().Replace("\\n", "\n");
            if (alternativeData.ContainsKey("Descrizione"))
                descriptionTextAlternative2.text = alternativeData["Descrizione"].ToString().Replace("\\n", "\n");
            if (alternativeData.ContainsKey("Immagine"))
                StartCoroutine(LoadImageFromUrl(alternativeData["Immagine"].ToString(), alternativeImage2));
            if (alternativeData.ContainsKey("EcoScore2"))
                StartCoroutine(LoadImageFromUrl(alternativeData["EcoScore2"].ToString(), ecoScoreImage2));
        }
    }

    private IEnumerator LoadImageFromUrl(string url, Image targetImage)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
                targetImage.SetNativeSize();
            }
            else
            {
                Debug.LogError("Errore nel caricamento dell'immagine: " + request.error);
            }
        }
    }

    public void SwitchToAlternativePanel()
    {
        mainPanel.SetActive(false);
        alternativePanel.SetActive(true);
    }

    public void SwitchToMainPanel()
    {
        alternativePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    void OnDestroy()
    {
        if (webcamTexture != null) webcamTexture.Stop();
        if (scannerUI != null) scannerUI.SetActive(false);
    }
}



