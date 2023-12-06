using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.EventSystems;

public class CardSelector : MonoBehaviour
{
    //public Camera camera;
    public LayerMask mask;

    public Vector3 basePos;
    public Transform Selectedcard;

    public float speed = 2.0f;
    public Transform positionOfCard;

    public Transform previousParent;

    public SFX_Cards sfx;

    //states//
    bool isCardSelected = false;
    bool selectingCard = false;

    bool deselectingCard = false;

    bool isRotated = false;

    bool wansToChangeRound = false;

    bool firstCardDeselecting = false;


    RaycastHit hit;

    private void Update()
    {
        if (Game.instance.isPurchasing || Game.instance.PreThrowingDiceState)
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

                    if (Game.instance.isPurchasing)
                        Game.instance.players[Game.instance.currentPlayerIndex].nextRoundButton.interactable = true;

                    if (Game.instance.PreThrowingDiceState)
                    {
                        Game.instance.players[Game.instance.currentPlayerIndex].roll1DiceButton.interactable = true;
                        Game.instance.players[Game.instance.currentPlayerIndex].roll2DiceButton.interactable = true;
                    }


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
        else if (Game.instance.isTrading)
        {
            if (!firstCardDeselecting)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!selectingCard) SelectionTradingState();
                    if (isCardSelected && !selectingCard && !deselectingCard) DeselectCard();
                }
                if (selectingCard && firstCardSelected)
                {
                    if (firstCard.position != positionOfCard.position)
                    {
                        MoveCardToCam(firstCard);
                    }
                    else
                    {
                        selectingCard = false;
                    }
                }
                else if (selectingCard && secondCardSelected)
                {
                    if (secondCard.position != positionOfCard.position)
                    {
                        MoveCardToCam(secondCard);
                    }
                    else
                    {
                        selectingCard = false;
                    }
                }
                else if (firstCardSelected && secondCardSelected)
                {
                    if (firstCard.position != firstCardPos)
                    {
                        MoveCardToPos(firstCard, firstCardPos);
                    }
                    else
                    {
                        CardDisplayData data = firstCard.GetComponent<CardDisplayData>();
                        if (data.player != null && data.player.NbCard(data.cardName) > 1)
                        {
                            Destroy(firstCard.gameObject);
                        }

                        firstCard = null;
                        firstCardPos = Vector3.zero;
                        selectingCard = true;
                        firstCardSelected = false;
                    }
                }
                else if (secondCardSelected && !firstCardSelected)
                {
                    if (secondCard.position != secondCardPos)
                    {
                        MoveCardToPos(secondCard, secondCardPos);
                    }
                    else
                    {
                        CardDisplayData data = secondCard.GetComponent<CardDisplayData>();
                        if (data.player != null && data.player.NbCard(data.cardName) > 1)
                        {
                            Destroy(secondCard.gameObject);
                        }

                        secondCardPos = Vector3.zero;
                        secondCard = null;
                        secondCardSelected = false;
                        Game.instance.playerMadeChoice = true;
                    }
                }
            }
            else
            {
                if (firstCard.position != basePos)
                {
                    MoveCardToPos(firstCard, basePos);
                }
                else
                {
                    firstCardDeselecting = false;
                    firstCard = null;
                    firstCardPlayer = null;
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
                    CardDisplayData data2 = card.GetComponent<CardDisplayData>();
                    data2.cardName = data.cardName;
                    data2.player = data.player;
                    data2.size = data.size;
                    data2.cardType = data.cardType;
                    data2.x = data.x;
                    data2.y = data.y;

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

            previousParent = Selectedcard.parent;
            Selectedcard.SetParent(positionOfCard, true);

            if (Game.instance.PreThrowingDiceState)
            {
                Debug.Log("klj654874v");
                Game.instance.players[Game.instance.currentPlayerIndex].roll1DiceButton.interactable = false;
                Game.instance.players[Game.instance.currentPlayerIndex].roll2DiceButton.interactable = false;
            }

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

        previousParent = Game.instance.players[Game.instance.currentPlayerIndex].cardContent;
        Selectedcard.SetParent(previousParent);
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
                basePos = data.player.cardContent.TransformPoint(Game.instance.players[Game.instance.currentPlayerIndex].AddToDict(Selectedcard.gameObject));
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

    Transform firstCard;
    Vector3 firstCardPos;
    //Vector3 firstCardBasePos;
    Player firstCardPlayer;

    Transform secondCard;
    Vector3 secondCardPos;
    //Vector3 secondCardBasePos;
    Player secondCardPlayer;

    bool firstCardSelected;
    bool secondCardSelected;


    public void SelectionTradingState()
    {
        if (throwCast(mask))
        {
            CardDisplayData data = hit.transform.GetComponent<CardDisplayData>();
            if (!firstCardSelected)
            {
                if (data.player != null && data.cardName != CardName.None && data.cardType != CardType.Building && data.player != Game.instance.players[Game.instance.currentPlayerIndex])
                {
                    if (data.player.NbCard(data.cardName) > 1)
                    {
                        GameObject card = Instantiate(hit.transform.gameObject, hit.transform.parent);
                        card.transform.position = hit.transform.position;
                        CardDisplayData data2 = card.GetComponent<CardDisplayData>();
                        data2.cardName = data.cardName;
                        data2.player = data.player;
                        data2.size = data.size;
                        data2.cardType = data.cardType;
                        data2.x = data.x;
                        data2.y = data.y;


                        firstCardSelected = true;
                        selectingCard = true;


                        firstCard = card.transform;
                        //firstCardBasePos = hit.transform.localPosition;
                        firstCardPlayer = data2.player;


                        Game.instance.tradingCardUI1.SetActive(false);
                        Game.instance.tradingCardUI2.SetActive(true);
                    }
                    else
                    {


                        firstCardSelected = true;
                        selectingCard = true;


                        firstCard = hit.transform;
                        //firstCardBasePos = hit.transform.localPosition;
                        firstCardPlayer = data.player;


                        Game.instance.tradingCardUI1.SetActive(false);
                        Game.instance.tradingCardUI2.SetActive(true);
                    }
                }
            }
            else
            {
                if (data.player != null && data.cardName != CardName.None && data.cardType != CardType.Building && data.player == Game.instance.players[Game.instance.currentPlayerIndex])
                {
                    if (data.player.NbCard(data.cardName) > 1)
                    {
                        GameObject card = Instantiate(hit.transform.gameObject, hit.transform.parent);
                        card.transform.position = hit.transform.position;
                        CardDisplayData data2 = card.GetComponent<CardDisplayData>();
                        data2.cardName = data.cardName;
                        data2.player = data.player;
                        data2.size = data.size;
                        data2.cardType = data.cardType;
                        data2.x = data.x;
                        data2.y = data.y;
                        secondCardSelected = true;
                        selectingCard = true;


                        secondCard = card.transform;
                        //secondCardBasePos = hit.transform.localPosition;
                        secondCardPlayer = data2.player;
                    }
                    else
                    {
                        secondCardSelected = true;
                        selectingCard = true;


                        secondCard = hit.transform;
                        //secondCardBasePos = hit.transform.localPosition;
                        secondCardPlayer = data.player;
                    }

                    firstCard.SetParent(secondCardPlayer.cardContent, true);
                    secondCard.SetParent(firstCardPlayer.cardContent, true);

                    
                    firstCardPlayer.PileCards[firstCard.GetComponent<CardDisplayData>().cardName]--;
                    if (firstCardPlayer.NbCard(firstCard.GetComponent<CardDisplayData>().cardName) == 0)
                        firstCardPlayer.cardObjects.Remove(firstCard.GetComponent<CardDisplayData>().cardName);

                    firstCardPlayer.GiveCard(secondCard.GetComponent<CardDisplayData>().cardName);

                    
                    secondCardPlayer.PileCards[secondCard.GetComponent<CardDisplayData>().cardName]--;
                    if (secondCardPlayer.NbCard(secondCard.GetComponent<CardDisplayData>().cardName) == 0)
                        secondCardPlayer.cardObjects.Remove(secondCard.GetComponent<CardDisplayData>().cardName);

                    secondCardPlayer.GiveCard(firstCard.GetComponent<CardDisplayData>().cardName);

                    secondCardPos = firstCardPlayer.cardContent.TransformPoint(firstCardPlayer.AddToDict(secondCard.gameObject));

                    firstCardPos = secondCardPlayer.cardContent.TransformPoint(secondCardPlayer.AddToDict(firstCard.gameObject));


                    //firstCardPlayer.ReloadCard();



                    firstCard.GetComponent<CardDisplayData>().player = secondCardPlayer;
                    secondCard.GetComponent<CardDisplayData>().player = firstCardPlayer;

                    Game.instance.playerToSteal = firstCardPlayer;
                    Game.instance.tradingCardUI2.SetActive(false);

                }
            }

        }
        else
        {
            if (firstCardSelected)
                DeselectTradingCard();
        }
    }

    void DeselectTradingCard()
    {
        basePos = firstCardPlayer.cardContent.TransformPoint(new Vector3(firstCard.GetComponent<CardDisplayData>().x, 0, firstCard.GetComponent<CardDisplayData>().y));

        firstCardDeselecting = true;
        firstCardSelected = false;
    }

    public void SelectPlayerToStealCoins(int index)
    {
        if (Game.instance.isStealingMoney)
        {
            Game.instance.playerToSteal = Game.instance.players[index];
            Game.instance.playerMadeChoice = true;
        }
    }



    public void MoveCardToCam(Transform card)
    {
        card.position = Vector3.MoveTowards(card.position, positionOfCard.position, speed * Time.deltaTime);
        card.localScale = Vector3.MoveTowards(card.localScale, Vector3.one, (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude);
    }

    public void MoveCardToPos(Transform card, Vector3 pos)
    {
        card.position = Vector3.MoveTowards(card.position, pos, speed * Time.deltaTime);
        card.localScale = Vector3.MoveTowards(card.localScale, card.GetComponent<CardDisplayData>().size * Vector3.one, (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude);

    }
}
