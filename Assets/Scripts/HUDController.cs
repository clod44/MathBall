using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using Random = UnityEngine.Random;
using Button = UnityEngine.UI.Button;

public class HUDController : MonoBehaviour
{
    public bool isQuestionActive = false;

    public CanvasGroup questionAreaCanvasGroup;
    public TextMeshProUGUI question;
    public TMP_InputField answer;
    public Button btnSubmit;
    public int correctAnswer = 0;

    public TextMeshProUGUI score;
    public BallControl ball;
    private bool questionShouldBeVisible = false;
    void Start()
    {
        HideQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        isQuestionActive = questionShouldBeVisible;
        questionAreaCanvasGroup.interactable = questionShouldBeVisible;
        questionAreaCanvasGroup.blocksRaycasts = questionShouldBeVisible;
        questionAreaCanvasGroup.alpha = Mathf.Lerp(questionAreaCanvasGroup.alpha, questionShouldBeVisible ? 1f : 0f, 3f * Time.deltaTime);
    }


    public void ShowQuestion()
    {
        isQuestionActive = true;
        int n1 = Random.Range(2, 10);
        int n2 = Random.Range(2, 10);
        int operatorIndex = Random.Range(0, 3);
        string operatorSymbol = "?";
        switch (operatorIndex)
        {
            case 0:
                operatorSymbol = "+";
                correctAnswer = n1 + n2;
                break;
            case 1:
                operatorSymbol = "-";
                correctAnswer = n1 - n2;
                break;
            case 2:
                operatorSymbol = "x";
                correctAnswer = n1 * n2;
                break;

        }

        btnSubmit.enabled = true;
        question.text = n1 + operatorSymbol + n2;
        answer.text = "";
        questionShouldBeVisible = true;
    }

    public void CheckAnswer()
    {
        btnSubmit.enabled = false;
        //clean the answer from whitespaces and such
        string strGuessedAnswer = answer.text;

        //check if the answer can be converted to integer
        bool success = int.TryParse(strGuessedAnswer, out int intGuessedAnswer);

        //if no, make the ball's launchAccuracy variable = 0;
        if (!success)
        {
            // Parsing failed, handle it here
            // For example, set ball's launchAccuracy to 0
            ball.launchAccuracy = 0;
        }
        else
        {
            // Parsing succeeded, calculate the accuracy
            float accuracy = 1.0f - Math.Abs((float)intGuessedAnswer / correctAnswer - 1.0f);
            // Ensure the accuracy is within the range [0, 1]
            accuracy = Math.Max(0, Math.Min(accuracy, 1));
            // Set the ball's launchAccuracy
            ball.launchAccuracy = accuracy;
            question.text = correctAnswer + ":" + intGuessedAnswer;

        }

        StartCoroutine(HideQuestionAfterDelay());
    }
    IEnumerator HideQuestionAfterDelay()
    {

        // WaitForSeconds will pause the execution of the Coroutine for 1 second
        yield return new WaitForSeconds(0.01f); //we dont need this anymore because fade out is more than enough

        // This line will be executed after 1 second
        HideQuestion();
    }
    public void HideQuestion()
    {
        questionShouldBeVisible = false;
    }

    public void SetScore(int scr = 0)
    {
        score.text = "" + scr;
    }
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
