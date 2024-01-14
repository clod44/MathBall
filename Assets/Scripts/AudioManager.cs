using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> soundPlayers = new Dictionary<string, AudioSource>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSounds("Sounds"); // Change the folder path as needed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSounds(string folderPath)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(folderPath);

        foreach (AudioClip clip in clips)
        {
            string clipName = Path.GetFileNameWithoutExtension(clip.name);
            soundLibrary.Add(clipName, clip);
        }
        PrintSoundNames(); // Print loaded sound names
    }

    private void PrintSoundNames()
    {
        Debug.Log("Loaded Sound Names:");
        foreach (string soundName in soundLibrary.Keys)
        {
            Debug.Log(soundName);
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            if (soundPlayers.TryGetValue(soundName, out AudioSource soundPlayer) && soundPlayer.isPlaying)
            {
                soundPlayer.Stop(); // Stop the sound if it's already playing
                Destroy(soundPlayer); // Destroy the previous AudioSource
            }

            AudioSource newSoundPlayer = gameObject.AddComponent<AudioSource>();
            newSoundPlayer.clip = soundLibrary[soundName];
            newSoundPlayer.Play();

            soundPlayers[soundName] = newSoundPlayer; // Store the AudioSource
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found in the library.");
        }
    }

    public void PlayLoopingSound(string soundName)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            AudioSource newSoundPlayer = gameObject.AddComponent<AudioSource>();
            newSoundPlayer.clip = soundLibrary[soundName];
            newSoundPlayer.loop = true;
            newSoundPlayer.Play();

            soundPlayers[soundName] = newSoundPlayer; // Store the AudioSource
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found in the library.");
        }
    }
    public static AudioManager GetInstance()
    {
        return instance;
    }

    public void StopSound(string soundName)
    {
        if (soundPlayers.TryGetValue(soundName, out AudioSource soundPlayer) && soundPlayer.isPlaying)
        {
            soundPlayer.Stop();
            Destroy(soundPlayer);
            soundPlayers.Remove(soundName); // Remove the AudioSource reference
        }
    }
}
