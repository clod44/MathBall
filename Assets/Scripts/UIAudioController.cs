using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAudioController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler
{
    public string hoverSoundName = "hover_1";
    public string clickSoundName = "select_1";
    public string inputFieldSoundName = "hover_1";

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.GetInstance();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(hoverSoundName);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(clickSoundName);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (audioManager != null)
        {
            audioManager.PlaySound(inputFieldSoundName);
        }
    }
}
