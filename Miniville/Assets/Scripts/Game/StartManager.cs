using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartManager : MonoBehaviour
{
    public List<UnityEngine.UI.Button> BTN_nbAi;
    private int _nbPlayer { get; set; }
    private int _nbAI { get; set; }

    public void Start()
    { 
        _nbPlayer = 0;
        _nbAI = 0;
        foreach (UnityEngine.UI.Button btn in BTN_nbAi)
            DisableButton(btn);

        EnableButton(BTN_nbAi[0]);
    }

    public void SetNbPlayer(int nbPlayer)
    {
        _nbPlayer = nbPlayer;

        switch (nbPlayer)
        {
            case 2:
                DisableButton(BTN_nbAi[1]);
                DisableButton(BTN_nbAi[2]);
                break;
            case 3:
                EnableButton(BTN_nbAi[1]);
                DisableButton(BTN_nbAi[2]);
                break;
            case 4:
                EnableButton(BTN_nbAi[1]);
                EnableButton(BTN_nbAi[2]);
                break;
        }

    }

    public void SelectedColor(GameObject btn)
    {
        foreach (GameObject otherBtn in btn.transform)
        {
            ColorBlock colorBlockEnable = otherBtn.GetComponent<UnityEngine.UI.Button>().colors;
            ColorBlock colorBlockDisable = otherBtn.GetComponent<UnityEngine.UI.Button>().colors;
            colorBlockEnable.normalColor = Color.green;
            colorBlockDisable.normalColor = Color.white;

            if (otherBtn == btn)
                btn.GetComponent<UnityEngine.UI.Button>().colors = colorBlockEnable;
            else
                btn.GetComponent<UnityEngine.UI.Button>().colors = colorBlockDisable;
        }
    }

    private void DisableButton(UnityEngine.UI.Button btn)
    {
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = Color.grey;

        btn.interactable = false;
        btn.colors = colorBlock;
    }

    private void EnableButton(UnityEngine.UI.Button btn)
    {
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = Color.white;

        btn.interactable = true;
        btn.colors = colorBlock;
    }

    public void ChangeScene()
    {

    }
}
