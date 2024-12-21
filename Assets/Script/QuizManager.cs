using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public GameObject quizPanel; // il pannello che contiene tutte le domande
    public TMP_Text questionText1;
    public TMP_Text questionText2;
    public TMP_Text questionText3;
    public TMP_Text questionText4;

    public TMP_Text[] answerTexts1; // risposte per la prima domanda
    public TMP_Text[] answerTexts2; // risposte per la seconda domanda
    public TMP_Text[] answerTexts3; // risposte per la terza domanda
    public TMP_Text[] answerTexts4; // risposte per la quarta domanda

    public Toggle[] answerToggles1; // toggle per la prima domanda
    public Toggle[] answerToggles2; // toggle per la seconda domanda
    public Toggle[] answerToggles3; // toggle per la terza domanda
    public Toggle[] answerToggles4; // toggle per la quarta domanda

    public Button submitButton; // pulsante per inviare i risultati
    public TMP_Text resultText; // testo per mostrare il risultato

    private string[] questions = {
        "Cosa significa la dicitura “MSC” su alcuni prodotti ittici?",
        "Perché i prodotti biologici sono più sostenibili?",
        "Quale delle seguenti scelte aiuta a ridurre l’impatto ambientale?",
        "Cosa significa \"chilometro zero\"?"
    };

    private string[][] answers = {
        new string[] { " Garantisce una pesca sostenibile", "Significa che il pesce è stato allevato in Italia", " Indica la qualità del sapore" },
        new string[] { "Sono più economici da produrre", "Non usano pesticidi chimici e proteggono la biodiversità", "Hanno un impatto minore solo nel trasporto" },
        new string[] { "Acquistare cibi preconfezionati", "Acquistare prodotti con imballaggi eco-friendly", "Comprare solo prodotti confezionati in plastica" },
        new string[] { "Cibo prodotto lontano dal luogo di consumo", "Cibo prodotto localmente vicino al consumatore", "Cibo senza imballaggi" }
    };

    private int[] correctAnswers = { 0, 1, 1, 1 }; // indici delle risposte corrette
    private int score = 0;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitClicked);

        DisplayQuestions();
        resultText.gameObject.SetActive(false);
    }

    void DisplayQuestions()
    {
        // mostra le domande e le risposte
        questionText1.text = questions[0];
        questionText2.text = questions[1];
        questionText3.text = questions[2];
        questionText4.text = questions[3];

        for (int i = 0; i < answerTexts1.Length; i++)
        {
            answerTexts1[i].text = answers[0][i];
            answerToggles1[i].isOn = false;
        }

        for (int i = 0; i < answerTexts2.Length; i++)
        {
            answerTexts2[i].text = answers[1][i];
            answerToggles2[i].isOn = false;
        }

        for (int i = 0; i < answerTexts3.Length; i++)
        {
            answerTexts3[i].text = answers[2][i];
            answerToggles3[i].isOn = false;
        }

        for (int i = 0; i < answerTexts4.Length; i++)
        {
            answerTexts4[i].text = answers[3][i];
            answerToggles4[i].isOn = false;
        }
    }

    public void OnSubmitClicked()
    {
        // calcola il punteggio
        score = 0;

        if (IsAnswerCorrect(answerToggles1, correctAnswers[0]))
            score++;
        if (IsAnswerCorrect(answerToggles2, correctAnswers[1]))
            score++;
        if (IsAnswerCorrect(answerToggles3, correctAnswers[2]))
            score++;
        if (IsAnswerCorrect(answerToggles4, correctAnswers[3]))
            score++;

        // mostra il punteggio
        resultText.gameObject.SetActive(true);
        resultText.text = "Hai completato il quiz! Punteggio totale: " + score + "/" + questions.Length;
    }

    private bool IsAnswerCorrect(Toggle[] toggles, int correctIndex)
    {
        // controlla se il Toggle corretto è selezionato
        return toggles[correctIndex].isOn;
    }
}













