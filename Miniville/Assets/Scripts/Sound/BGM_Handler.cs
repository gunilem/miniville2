using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Handler : MonoBehaviour
{
    [SerializeField] private EventReference eventMusic;
    private EventInstance eventInstance;

    public static BGM_Handler instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Il y a trop de MainMenuScript");
            return;
        }
        instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        eventInstance = RuntimeManager.CreateInstance(eventMusic);
        eventInstance.start();
    }

    public void ChangeMusic(string music)
    {
       // POSSIBLE MUSICS : Start, Game, End
       switch (music)
        {
            case "Start":
                eventInstance.setParameterByName("Music_Selector", 0.0F);
                break;
            case "Game":
                eventInstance.setParameterByName("Music_Selector", 1.0F);
                break;
            case "End":
                eventInstance.setParameterByName("Music_Selector", 2.0F);
                break;
        }
    }
}
