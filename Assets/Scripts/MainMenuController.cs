using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MainMenuController : MonoBehaviour
{
    public VolumeExpoLerper globalVolume;
    public TextMeshProUGUI title;
    AudioManager audioManager;
    void Start()
    {
        audioManager = AudioManager.GetInstance();
        audioManager.PlaySound("reverb_1");
        globalVolume.ChangeFromTo(-10f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (title != null) // prevent that single frame null error (bruh)
        {
            string titleTextMATH = "MATH";
            string titleTextBALL = "BALL";
            if (Random.Range(0f, 1f) < Time.deltaTime * 30)
            {
                int randomNumber1 = Random.Range(0, 10);
                titleTextMATH = "M" + randomNumber1.ToString() + "TH";
                audioManager.PlaySound("hover_1");
            }
            if (Random.Range(0f, 1f) < Time.deltaTime * 30)
            {
                int randomNumber1 = Random.Range(0, 10);
                titleTextBALL = "B" + randomNumber1.ToString() + "LL";
                audioManager.PlaySound("hover_1");
            }
            title.text = titleTextMATH + "\n" + titleTextBALL;
        }

    }


    public void StartGame()
    {

        audioManager.PlaySound("reverb_2");
        globalVolume.ChangeFromTo(0f, -10f, 1f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            });

    }
}
