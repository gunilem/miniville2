using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Il y a trop de CameraScript");
            return;
        }
        instance = this;
    }

    Vector3 originalPos = new Vector3(0, 17, 0);
    Vector3[] playersPos = new Vector3[4];
    Vector3 currentTarget = new Vector3(0, 17, 0);
    [Header("Setting")]
    [SerializeField] float lerpSpeed; //la vitesse de mouvement de la camera quand elle passe d'une vue à l'autre 
    
    void Start() //ici on défini à la main les positions associé à chaque joueur
    {
        playersPos[0] = new Vector3(-5.7f, 9.5f, -4);
        playersPos[1] = new Vector3(-5.7f, 9.5f, 3.74f);
        playersPos[2] = new Vector3(6, 9.5f, 3.74f);
        playersPos[3] = new Vector3(6, 9.5f, -4);
    }

    void Update()
    {
        if (Game.instance.isPurchasing || Game.instance.PreThrowingDiceState) //empeche la vue de changer au mauvais moment 
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.mouseScrollDelta.y > 0) GoToPlayer(); //si on scroll ou appuis sur Z, on fait zoomer la camera sur le joueur qui joue actuellement 
            if (Input.GetKeyDown(KeyCode.S) || Input.mouseScrollDelta.y < 0) GoToOriginalPos(); //si à l'inverse ou appuis sur S ou qu'on scroll vers l'arière, alors on montre tout le plateau
        }
        Move();
    }

    private void Move() //le lerp qui permet de passer d'une vue à l'autre 
    {
        this.transform.position = Vector3.Lerp(transform.position, currentTarget, lerpSpeed * Time.deltaTime);
    }

    public void GoToPlayer() //peut être appelé par n'importe qu'elle script pour dire à la camera de zoomer sur le joueur qui joue actuellement
    {
        currentTarget = playersPos[Game.instance.currentPlayerIndex]; //on met la target sur le joueur actuel

        for(int i = 0; i < Game.instance.numberOfPlayers; i++)//désactive toutes les UI des players sauf celle de celui qui joue 
        {
            if (i == Game.instance.currentPlayerIndex) Game.instance.players[i].canvas.SetActive(true);
            else Game.instance.players[i].canvas.SetActive(false);
        }
    }
    public void GoToOriginalPos()//peut être appelé par n'importe qu'elle script pour dire à la camera de dézoomer sur la vue d'ensemble 
    {
        currentTarget = originalPos;
        for (int i = 0; i < Game.instance.numberOfPlayers; i++)
        {
            Game.instance.players[i].canvas.SetActive(true);
        }
    }
}
