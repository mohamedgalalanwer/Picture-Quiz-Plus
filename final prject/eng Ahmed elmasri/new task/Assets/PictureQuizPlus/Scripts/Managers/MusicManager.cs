using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour //Handle music and sounds in the game
{
    public AudioClip[] sounds; //Array of sounds
    public AudioSource musicPlayer; //Source to play music
    public AudioSource soundsPlayer; //Source to play sounds
    public bool isMusic;
    public bool isSounds;
    Dictionary<string, AudioClip> soundsBag;

    public static MusicManager instance { get; private set; }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        instance = this;

        //Get saved values from the registry to set up preferences
        if (PlayerPrefs.GetInt("Music", 1) == 0)
        {
            SetMusic(false);
        }
        if (PlayerPrefs.GetInt("Sounds", 1) == 0)
        {
            SetSound(false);
        }

        soundsBag = new Dictionary<string, AudioClip>(); //Collection of sounds with easy by name access 

        try
        {
            foreach (var item in sounds)
            {
                soundsBag.Add(item.name.Split('.')[0], item);
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("There are no sounds available");
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetMusic(bool state)
    {
        if (state)
        {
            PlayerPrefs.SetInt("Music", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Music", 0);
        }
        musicPlayer.mute = !state;
        isMusic = state;
    }

    public void SetSound(bool state)
    {
        if (state)
        {
            PlayerPrefs.SetInt("Sounds", 1);
        }
        else PlayerPrefs.SetInt("Sounds", 0);
        soundsPlayer.mute = !state;
        isSounds = state;
    }
    public void PlaySound(string name) //Main public method to play sounds
    {
        soundsPlayer.PlayOneShot(soundsBag[name]);
    }
}
