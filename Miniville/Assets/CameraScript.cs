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
    [SerializeField] float lerpSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        playersPos[0] = new Vector3(-5.7f, 9.5f, -4);
        playersPos[1] = new Vector3(-5.7f, 9.5f, 3.74f);
        playersPos[2] = new Vector3(6, 9.5f, 3.74f);
        playersPos[3] = new Vector3(6, 9.5f, -4);
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.instance.isPurchasing || Game.instance.PreThrowingDiceState)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.mouseScrollDelta.y > 0) GoToPlayer();
            if (Input.GetKeyDown(KeyCode.S) || Input.mouseScrollDelta.y < 0) GoToOriginalPos();
        }
        Move();
    }

    private void Move()
    {
        this.transform.position = Vector3.Lerp(transform.position, currentTarget, lerpSpeed * Time.deltaTime);
    }

    public void GoToPlayer()
    {
        currentTarget = playersPos[Game.instance.currentPlayerIndex];
        //désactive toutes les UI des players sauf celle de celui qui joue 
        for(int i = 0; i < Game.instance.numberOfPlayers; i++)
        {
            if (i == Game.instance.currentPlayerIndex) Game.instance.players[i].canvas.SetActive(true);
            else Game.instance.players[i].canvas.SetActive(false);
        }
    }
    public void GoToOriginalPos()
    {
        currentTarget = originalPos;
        for (int i = 0; i < Game.instance.numberOfPlayers; i++)
        {
            Game.instance.players[i].canvas.SetActive(true);
        }
    }
}
