using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class CardSelector : MonoBehaviour
{
    //public Camera camera;
    public LayerMask mask;

    public Vector3 basePos;
    public Transform Selectedcard;

    public float speed = 2.0f;
    public Transform positionOfCard;

    //bool isSelectingCard = false;
    //bool isDeselectingCard = false;

    //bool ifCardSelected = false;
    //bool isRotated = false;

    //bool isPurchasingCard = false;

    public SFX_Cards sfx;

    //states//
    bool isCardSelected = false;
    bool selectingCard = false;

    bool deselectingCard = false;

    bool isRotated = false;

    bool wansToChangeRound = false;


    RaycastHit hit;

    private void Update()
    {
        if (Game.instance.isPurchasing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isCardSelected) SelectCard();
                if (isCardSelected && !selectingCard && !deselectingCard) DeselectCard();
            }
            if (selectingCard)
            {
                if (Selectedcard.position != positionOfCard.position)
                {
                    Selectedcard.position = Vector3.MoveTowards(Selectedcard.position, positionOfCard.position, speed * Time.deltaTime);
                    Selectedcard.localScale = Vector3.MoveTowards(Selectedcard.localScale, Vector3.one, (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude);
                }
                else
                {
                    selectingCard = false;
                    CardDisplayData data = Selectedcard.GetComponent<CardDisplayData>();
                    if (Game.instance.isPurchasing)
                    {
                        if (data.player == null) Game.instance.players[Game.instance.currentPlayerIndex].purchaseCardButton.interactable = true;
                        else
                        {
                            if (data.cardName == CardName.None
                                && !data.player.PileMonuments[data.monumentName]
                                && data.player.transform == Game.instance.players[Game.instance.currentPlayerIndex].transform)
                            { Game.instance.players[Game.instance.currentPlayerIndex].purchaseCardButton.interactable = true; }
                        }
                    }
                }
            }
            if (deselectingCard)
            {
                if (Selectedcard.position != basePos)
                {
                    Selectedcard.position = Vector3.MoveTowards(Selectedcard.position, basePos, speed * Time.deltaTime);
                    Selectedcard.localScale = Vector3.MoveTowards(Selectedcard.localScale, Selectedcard.GetComponent<CardDisplayData>().size * Vector3.one, (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude);
                }
                else
                {
                    CardDisplayData data = Selectedcard.GetComponent<CardDisplayData>();

                    deselectingCard = false;
                    isCardSelected = false;

                    if (data.cardName != CardName.None)
                    {
                        if ((data.player != null && data.player.NbCard(data.cardName) > 1)
                            || (data.player == null && Game.instance.NbCard(data.cardName) > 1))
                        {
                            Destroy(Selectedcard.gameObject);
                        }
                    }
                    if (data.player != null) data.player.ReloadCard();

                    Game.instance.players[Game.instance.currentPlayerIndex].nextRoundButton.interactable = true;

                    Selectedcard = null;
                    basePos = Vector3.zero;
                    if (wansToChangeRound)
                    {
                        wansToChangeRound = false;
                        ToNextState();
                    }
                }
            }
        }
    }

    private void SelectCard()
    {
        if (throwCast(mask))
        {
            basePos = hit.transform.position;
            CardDisplayData data = hit.transform.GetComponent<CardDisplayData>();

            if (data.cardName != CardName.None)
            {
                if ((data.player != null && data.player.NbCard(data.cardName) > 1) || (data.player == null && Game.instance.NbCard(data.cardName) > 1))
                {
                    GameObject card = Instantiate(hit.transform.gameObject, hit.transform.parent);
                    card.transform.position = basePos;
                    card.GetComponent<CardDisplayData>().cardName = data.cardName;
                    card.GetComponent<CardDisplayData>().player = data.player;
                    card.GetComponent<CardDisplayData>().size = data.size;
                    Selectedcard = card.transform;
                }
                else
                    Selectedcard = hit.transform;
            }
            else
                Selectedcard = hit.transform;

            Game.instance.players[Game.instance.currentPlayerIndex].nextRoundButton.interactable = false;
            isCardSelected = true;
            selectingCard = true;

            //SFX
            sfx.PlaySound("cardToFront", Selectedcard);
        }
        
    }

    public void DeselectCard()
    {
        if (throwCast(LayerMask.NameToLayer("Everything")))
        {
            if (hit.transform == Selectedcard)
            {
                FlipCard();
            }
            else
            {
                Deselect();
            }
        }
    }

    void Deselect()
    {
        deselectingCard = true;
        if (isRotated)
        {
            Selectedcard.Rotate(Vector3.forward, 180);
            isRotated = false;
        }
        Game.instance.players[Game.instance.currentPlayerIndex].purchaseCardButton.interactable = false;

        //SFX
        sfx.PlaySound("cardToBack", Selectedcard);
    }

    public void FlipCard()
    {
        if (Selectedcard.GetComponent<CardDisplayData>().cardName != CardName.None)
        {
            Selectedcard.Rotate(Vector3.forward, 180);
            isRotated = !isRotated;

            //SFX
            sfx.PlaySound("cardFlip", Selectedcard);
        }
    }

    bool throwCast(LayerMask mask)
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        if (results.Count == 0)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            return Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
        }
        return false;
    }

    public void Purchase()
    {
        CardDisplayData data = Selectedcard.GetComponent<CardDisplayData>();
        if (data.cardName != CardName.None)
        {

            if (Game.instance.PlayerBuild(data.cardName))
            {

                data.player = Game.instance.players[Game.instance.currentPlayerIndex];
                data.size = data.player.cardSizeMultiplier;
                Selectedcard.SetParent(data.player.cardContent, true);
                basePos = data.player.cardContent.position + Game.instance.players[Game.instance.currentPlayerIndex].AddToDict(Selectedcard.gameObject);
                wansToChangeRound = true;
            }
        }
        else
        {
            if (Game.instance.PlayerBuildMonument(data.monumentName))
            {
                Selectedcard.Rotate(Vector3.forward, 180);
                //SFX
                sfx.PlaySound("cardFlip", Selectedcard);
                wansToChangeRound = true;
            }
        }
        Deselect();
    }

    public void ToNextState()
    {
        if (isCardSelected) { wansToChangeRound = true; Deselect(); }
        else Game.instance.ToNextState = true;
    }
}
