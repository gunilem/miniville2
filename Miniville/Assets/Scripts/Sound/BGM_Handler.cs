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
            Debug.Log("Il y a trop de d'instance");
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
                RuntimeManager.StudioSystem.setParameterByName("Music_Selector", 0.0f);
                break;
            case "Game":
                RuntimeManager.StudioSystem.setParameterByName("Music_Selector", 1.0f);
                break;
        }
    }

    public void DestroyInstance()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Destroy(this);
    }
}
