using FMODUnity;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuScript : MonoBehaviour
{
    public static MainMenuScript instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Il y a trop de MainMenuScript");
            return;
        }
        instance = this;
    }
    [Header("Refs")]
    [SerializeField] TMP_Text numberOfPlayerUI;
    [SerializeField] TMP_Text numberOf_IA_UI;
    [SerializeField] TMP_Text gameMode_UI;
    [Header("Data")]
    [SerializeField] private int _numberOfPlayers = 2;
    [SerializeField] private string _gameMode = "Classique";
    [Header("Invoke")]
    [SerializeField] public UnityEvent onChangeScene;
    public int numberOfPlayers //cette propri�t� est celle qui d�fini combien de joueur humain l'utilisateur veut mettre, elle ne peut rester que entre 1 et 4
    { 
        get { 
            return _numberOfPlayers;
        }
        set { 
            if(value > 4) _numberOfPlayers = 4;
            else if(value < 1) _numberOfPlayers = 1;
            else _numberOfPlayers = value;
            numberOfPlayerUI.text = _numberOfPlayers.ToString();
            if(numberOfIA > _numberOfPlayers) numberOfIA = _numberOfPlayers;
        } 
    }
    [SerializeField] private int _numberOfIA = 0;
    public int numberOfIA
    {
        get
        {
            return _numberOfIA;
        }
        set
        {
            if (value > _numberOfPlayers) _numberOfIA = _numberOfPlayers;
            else if (value < 0) _numberOfIA = 0;
            else _numberOfIA = value;
            numberOf_IA_UI.text = _numberOfIA.ToString();
        }
    }
    public string gameMode //cette propri�t� modifie automatiquement la UI associ� 
    {
        get
        {
            return _gameMode;
        }
        set
        {
            _gameMode = value;
            gameMode_UI.text = gameMode.ToString();
        }
    }

    private void Start()
    {
        Debug.Log("Start appel�");
        DontDestroyOnLoad(this); //ce script est utilis� sur la scene mainGame et est donc d�lcar� dans DontDestroyOnLoad
    }


    //les m�thodes suivant sont utilis� par des bouton Unity
    public void AddPlayer()
    {
        numberOfPlayers++;
    }
    public void RemovePlayer()
    {
        numberOfPlayers--;
    }
    public void AddIA()
    {
        numberOfIA++;
    }
    public void RemoveIA()
    {
        numberOfIA--;
    }

    public void SetGameMode(bool currentOrder)
    {
        switch(gameMode)
        {
            case "Classique":
                if (!currentOrder)
                {
                    gameMode = "Expert";
                    break;
                }
                gameMode = "Rapide";
                break;
            case "Rapide":
                if (!currentOrder)
                {
                    gameMode = "Classique";
                    break;
                }
                gameMode = "Normal";
                break;
            case "Normal":
                if (!currentOrder)
                {
                    gameMode = "Rapide";
                    break;
                }
                gameMode = "Long";
                break;
            case "Long":
                if (!currentOrder)
                {
                    gameMode = "Normal";
                    break;
                }
                gameMode = "Expert";
                break;
            case "Expert":
                if (!currentOrder)
                {
                    gameMode = "Long";
                    break;
                }
                gameMode = "Classique";
                break;
        }
    }

    public void PlayButton()
    {
        onChangeScene.Invoke();
        SceneManager.LoadScene("MAIN_GAME");
        Debug.Log("Cette ligne � �t� appeler apr�s le load de scene");

    }

}
