using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public int _nbHumanPlayer { get; private set; }
    public int _nbAIPlayer { get; private set; }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void SetNbPlayer(int nbHumanPlayer, int nbAIPlayer)
    {
        _nbAIPlayer = nbAIPlayer;
        _nbHumanPlayer = _nbHumanPlayer;
    }
}
