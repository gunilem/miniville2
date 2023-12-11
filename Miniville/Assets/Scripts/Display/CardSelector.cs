using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using static UnityEditor.PlayerSettings;
using UnityEditor;

public class CardSelector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Card's layer mask")] public LayerMask mask;
    [Tooltip("speed of the card to travel from a position to the other")] public float speed = 2.0f;
    [Tooltip("Sound Effects Script")] public SFX_Cards sfx;
    [Tooltip("Transform to know the position of the card in front of the camera")] public Transform positionOfCard;


    #region informations of the current selected card
    //informations of the current selected card//
    [HideInInspector] public Transform Selectedcard; // the selected card

    [HideInInspector] public Transform previousParent; // privous parent of the selected card
    [HideInInspector] public Vector3 basePos; // position before the selection
    #endregion


    #region states
    //states//
    bool isCardSelected = false;

    bool selectingCard = false; // is selecting the card
    bool deselectingCard = false; // is deselecting the card

    bool isRotated = false; // if the card was rotated

    bool wantsToChangeRound = false; // to know if we have to change the round 
    #endregion


    delegate void Del(); // definition of a delegate attribut void and without parameters

    RaycastHit hit; // raycast hit results


    private void Update()
    {
        if (Game.instance.isPurchasing || Game.instance.PreThrowingDiceState)// if either in the Purchasing state or the state waiting the player to throw his dices
        {
            if (!Game.instance.players[Game.instance.currentPlayerIndex].playerIsAI) // if player is not an AI
            {
                if (Input.GetMouseButtonDown(0)) // if the player is clicking
                {

                    if (!isCardSelected) SelectCard(); // select a card if there is no card selected
                    if (isCardSelected && !selectingCard && !deselectingCard) DeselectCard(); // deselect the card if there is a card selected and not in selecting or deselecting state
                }
            }
            else if (!isCardSelected) // if player is an AI and there is no card selected
            {
                if ((int)UnityEngine.Random.Range(0, 2) == 0) // random between 0 and 1
                {
                    SelectCardAIForPurchase(); // if 0 select a card
                }
                else
                {
                    ToNextState(); // if 1 skip the round;
                }
            }
            if (selectingCard) // if selecting card state
            {
                MoveCardToCam(Selectedcard, new Del[] { FinishSelectingState, TransitionSelectingStateToPurchaseState }); // move selectedcard to the camera and do referenced action 
            }
            if (deselectingCard)// if deselecting card state
            {
                MoveCardToPos(Selectedcard, basePos, new Del[] { FinishDeselectingState });// move selectedcard to the player pos and do referenced action 
            }
        }
        else if (Game.instance.isTrading) // if is in trading state
        {
            if (!stolenCardDeselecting)  // if not is in the deselecting state of the stolen card
            {
                if (Game.instance.players[Game.instance.currentPlayerIndex].playerIsAI) // if current player is a AI
                {
                    if (!selectingCard && !givenCardSelected) // if is not in selecting state and the given card is not selected
                        { SelectionTradingStateAI(); } // select card
                }
                else 
                {
                    if (Input.GetMouseButtonDown(0)) // if the player is clicking
                    {
                        if (!selectingCard) SelectionTradingState(); // if is not in selecting state select a card
                        if (isCardSelected && !selectingCard && !deselectingCard) DeselectCard(); //if a card is selected and if is neither in selecting nor deselecting state deselect a card 
                    }
                }
                if (selectingCard && stolenCardSelected) // if is in selecting state and stolen card is selected
                {
                    MoveCardToCam(stolenCard, new Del[] { FinishSelectingState }); // move stolenCard to the camera and do referenced action 
                }
                else if (selectingCard && givenCardSelected)// if is in selecting state and given card is selected
                {
                    MoveCardToCam(givenCard, new Del[] { FinishSelectingState }); // move givenCard to the camera and do referenced action 
                }
                else if (stolenCardSelected && givenCardSelected)// if neither the stolen and the given card is selected
                {
                    MoveCardToPos(stolenCard, stolenCardPos, new Del[] { DeselectStolenCard }); // move stolenCard to the stolenCardPos and do referenced action 
                }
                else if (givenCardSelected && !stolenCardSelected)// if the given card is selected but not the stolen one
                {
                    MoveCardToPos(givenCard, givenCardPos, new Del[] { DeselectGivenCard });// move givenCard to the givenCardPos and do referenced action
                }
            }
            else
            {
                MoveCardToPos(stolenCard, stolenCardPos, new Del[] { ResetStolenCard }); //  move stolenCard to the basePos and do referenced action
            }
        }
        else if (Game.instance.isStealingMoney) // if is in stealing money state
        {
            if (Game.instance.players[Game.instance.currentPlayerIndex].playerIsAI) // if current player is an AI
            {
                int random = UnityEngine.Random.Range(0, Game.instance.numberOfPlayers);
                if (Game.instance.currentPlayerIndex == random)
                {
                    if (random == Game.instance.numberOfPlayers - 1)
                    {
                        random--;
                    }
                    else random++;
                }
                SelectPlayerToStealCoins(random);
            }
        }
    }

    void FinishSelectingState()
    {
        selectingCard = false;
    }

    void TransitionSelectingStateToPurchaseState()
    {
        Player curentPlayer = Game.instance.players[Game.instance.currentPlayerIndex]; // get the player script of the current player
        CardDisplayData data = Selectedcard.GetComponent<CardDisplayData>(); // get the data of the selected card

        if (Game.instance.isPurchasing) // if in the purchasing state
        {
            if (data.player == null) curentPlayer.purchaseCardButton.interactable = true; // if the owner player of the selected card is null (the board) active the purchase button of the current player 
            else
            {
                if (data.cardName == CardName.None // if the card is a monument and 
                    && !data.player.PileMonuments[data.monumentName] // monument is not purchased yet
                    && data.player == curentPlayer) // if the owner player of the card is the current player
                { curentPlayer.purchaseCardButton.interactable = true; } // active the purchase button of the current player 
            }
        }

        if (curentPlayer.playerIsAI) // if current player is and AI
        {
            Purchase(); // purchase;
        }
    }

    void FinishDeselectingState()
    {
        deselectingCard = false;
        isCardSelected = false;


        Player curentPlayer = Game.instance.players[Game.instance.currentPlayerIndex]; // get the player script of the current player
        CardDisplayData data = Selectedcard.GetComponent<CardDisplayData>(); // get the data of the selected card

        if (data.cardName != CardName.None)// if the card is not a monument
        {
            if ((data.player != null && data.player.NbCard(data.cardName) > 1) // if the owner player of the card is defined and player has more than 1 card
                || (data.player == null && Game.instance.NbCard(data.cardName) > 1))// or if the owner player of the card is not defined (the board) and there is more than 1 card
            {
                Destroy(Selectedcard.gameObject); // destroy the card (this prevent to have more than 1 card stacked)
            }
        }
        if (data.player != null) data.player.ReloadCard(); // if the owner player of the card is defined reload deck display

        if (Game.instance.isPurchasing) // is purchasing state
            curentPlayer.nextRoundButton.interactable = true; // let the player choose to skip the round

        if (Game.instance.PreThrowingDiceState)// is pre throwing dice state
        {
            curentPlayer.roll1DiceButton.interactable = true; // letting the player choose to roll the dice
            curentPlayer.roll2DiceButton.interactable = true; // letting the player choose to roll the dice
        }

        //reset variable//
        Selectedcard = null;
        basePos = Vector3.zero;
        //

        if (wantsToChangeRound) // if wants to change round
        {
            wantsToChangeRound = false; 
            ToNextState(); // change round
        }
    }

    void DeselectStolenCard() 
    {
        CardDisplayData data = stolenCard.GetComponent<CardDisplayData>();// get the data of the stolen card
        if (data.player != null && data.player.NbCard(data.cardName) > 1)// if the owner player of the card is defined and player has more than 1 card
        {
            Destroy(stolenCard.gameObject); // destroy the card (this prevent to have more than 1 card stacked)
        }

        //reset variable//
        stolenCard = null;
        stolenCardPos = Vector3.zero;
        selectingCard = true;
        stolenCardSelected = false;
        //
    }

    void DeselectGivenCard()
    {
        CardDisplayData data = givenCard.GetComponent<CardDisplayData>();// get the data of the given card
        if (data.player != null && data.player.NbCard(data.cardName) > 1)// if the owner player of the card is defined and player has more than 1 card
        {
            Destroy(givenCard.gameObject); // destroy the card (this prevent to have more than 1 card stacked)
        }

        //reset variable//
        givenCardPos = Vector3.zero;
        givenCard = null;
        givenCardSelected = false;
        //

        // set the game variable to switch the state//
        Game.instance.isTrading = false;
        Game.instance.playerMadeChoice = true;
        //
    }

    void ResetStolenCard()
    {
        stolenCardDeselecting = false;
        stolenCard = null;
        stolenCardPlayer = null;
    }


    private void SelectCard()
    {
        if (throwCast(mask)) //detect raycast on the mask
        {
            Player currentPlayer = Game.instance.players[Game.instance.currentPlayerIndex];// get the player script of the current player
            basePos = hit.transform.position; // set base pos to the current position of the hit card
            CardDisplayData data = hit.transform.GetComponent<CardDisplayData>(); // get the data of the hit card

            if (data.cardName != CardName.None)// if the card is not a monument
            {
                if ((data.player != null && data.player.NbCard(data.cardName) > 1) // if the owner player of the card is defined and player has more than 1 card
                    || (data.player == null && Game.instance.NbCard(data.cardName) > 1))// or if the owner player of the card is not defined (the board) and there is more than 1 card
                {
                    GameObject card = InstantiateNewCardObject(hit.transform.gameObject, data); // instantiate a new card object like the hit one

                    Selectedcard = card.transform; // set the selected card to new one just created;
                }
                else
                    Selectedcard = hit.transform; // set the selected card to hit one
            }
            else
                Selectedcard = hit.transform;// set the selected card to hit one

            currentPlayer.nextRoundButton.interactable = false;// letting the player choose to skip the round

            // set the variable//
            isCardSelected = true;
            selectingCard = true;
            previousParent = Selectedcard.parent;
            Selectedcard.SetParent(positionOfCard, true);
            //

            if (Game.instance.PreThrowingDiceState) // if we are in the pre throwing dice state
            {
                currentPlayer.roll1DiceButton.interactable = false; // not letting the player choose to rool the dice
                currentPlayer.roll2DiceButton.interactable = false; // not letting the player choose to rool the dice
            }

            //SFX
            sfx.PlaySound("cardToFront", Selectedcard);
        }
        
    }

    public void SelectCardAIForPurchase()
    {
        GameObject[] gos;
        if ((int)UnityEngine.Random.Range(0, 2) == 0)
        {
            gos = Game.instance.cardObjects.Values.ToArray();
        }
        else
        {
            gos = Game.instance.players[Game.instance.currentPlayerIndex].MonumentsObjects.Values.ToArray();
        }
        int randomGo = UnityEngine.Random.Range(0, gos.Length);
        GameObject cardObject = gos[randomGo];

        // same as SelectCard() but for cardObject object 

        Player currentPlayer = Game.instance.players[Game.instance.currentPlayerIndex];
        basePos = cardObject.transform.position;
        CardDisplayData data = cardObject.transform.GetComponent<CardDisplayData>();

        if (data.cardName != CardName.None)
        {
            if ((data.player != null && data.player.NbCard(data.cardName) > 1) || (data.player == null && Game.instance.NbCard(data.cardName) > 1))
            {
                GameObject card = InstantiateNewCardObject(cardObject, data);

                Selectedcard = card.transform;
            }
            else
                Selectedcard = cardObject.transform;
        }
        else
            Selectedcard = cardObject.transform;

        currentPlayer.nextRoundButton.interactable = false;
        isCardSelected = true;
        selectingCard = true;

        previousParent = Selectedcard.parent;
        Selectedcard.SetParent(positionOfCard, true);

        if (Game.instance.PreThrowingDiceState)
        {
            Debug.Log("klj654874v");
            currentPlayer.roll1DiceButton.interactable = false;
            currentPlayer.roll2DiceButton.interactable = false;
        }

        //SFX
        sfx.PlaySound("cardToFront", Selectedcard);
    }

    public void DeselectCard()
    {
        if (throwCast(LayerMask.NameToLayer("Everything"))) // throw a cast on everything
        {
            if (hit.transform == Selectedcard) // if the hit is the selected card
            {
                FlipCard(); // flip the card
            }
            else
            {
                Deselect(); // deselect the card
            }
        }
    }

    void Deselect()
    {
        deselectingCard = true; // set the state
        if (isRotated) // if card was rotated
        {
            FlipCard(); // flip the card
        }
        Game.instance.players[Game.instance.currentPlayerIndex].purchaseCardButton.interactable = false; // not letting the player choose to purchase the card

        Selectedcard.SetParent(previousParent); // set the parent of the selected card to its previous parent
        //SFX
        sfx.PlaySound("cardToBack", Selectedcard);
    }

    public void FlipCard()
    {
        if (Selectedcard.GetComponent<CardDisplayData>().cardName != CardName.None) // if the card is not a monument
        {
            Selectedcard.Rotate(Vector3.forward, 180); // rotate 180° the card around z axis 
            isRotated = !isRotated; // set the isrotated variable to its opposite

            //SFX
            sfx.PlaySound("cardFlip", Selectedcard);
        }
    }

    bool throwCast(LayerMask mask)
    {
        // use to cast a ray cast on the UI
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>(); // result of the UI raycast
        EventSystem.current.RaycastAll(eventData, results); // ray cast
        //
        if (results.Count == 0) // if ray hit nothing
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); // get the ray from the camera to the mouse position

            return Physics.Raycast(ray, out hit, Mathf.Infinity, mask); // return the result of the raycast 
        }
        return false;
    }

    public void Purchase()
    {
        CardDisplayData data = Selectedcard.GetComponent<CardDisplayData>(); // get the data of the selected card
        if (data.cardName != CardName.None) // if card is not a monument
        {

            if (Game.instance.PlayerBuild(data.cardName)) // try to purchase the card
            {
                // if card purchased
                // refresh the variable of the card's data //
                data.player = Game.instance.players[Game.instance.currentPlayerIndex];
                data.size = data.player.cardSizeMultiplier;
                //

                Selectedcard.SetParent(data.player.cardContent, true); // set the new parent of the selected card to the card content of the buyer

                // set the new base Position
                basePos = data.player.cardContent.TransformPoint(Game.instance.players[Game.instance.currentPlayerIndex].AddToDict(Selectedcard.gameObject));
                
                // we wants to change the round because a player can purchase one card by round
                wantsToChangeRound = true;

                //change the previous parent to the new player card content
                previousParent = Game.instance.players[Game.instance.currentPlayerIndex].cardContent;
            }
        }
        else
        {
            if (Game.instance.PlayerBuildMonument(data.monumentName))// try to purchase the monument
            {
                // if monument purchased
                Selectedcard.Rotate(Vector3.forward, 180); // flip the card

                // we wants to change the round because a player can purchase one card by round
                wantsToChangeRound = true;

                //SFX
                sfx.PlaySound("cardFlip", Selectedcard);
            }
        }
        Deselect(); // deselect selected card
    }

    public void ToNextState()
    {
        if (isCardSelected) { wantsToChangeRound = true; Deselect(); }
        else Game.instance.ToNextState = true;
    }


    Transform stolenCard;
    Vector3 stolenCardPos;
    Player stolenCardPlayer;
    bool stolenCardSelected;

    Transform givenCard;
    Vector3 givenCardPos;
    Player givenCardPlayer;
    bool givenCardSelected;


    bool stolenCardDeselecting = false;


    public void SelectionTradingState()
    {
        if (throwCast(mask)) // throw a cast
        {
            // if the layer mask has been hit
            CardDisplayData data = hit.transform.GetComponent<CardDisplayData>(); // get the data of the selected card
            if (data.player != null // if player defined
                    && data.cardName != CardName.None // and if the card is not a monument
                    && data.cardType != CardType.Building  // and if the card is not a building
                    && data.player != Game.instance.players[Game.instance.currentPlayerIndex]) // and if the owner player is not the current player
            {

                if (!stolenCardSelected) // if stolen card is not selected
                {

                    // set the state variables//
                    stolenCardSelected = true;
                    selectingCard = true;
                    //

                    // set the UI Display //
                    Game.instance.tradingCardUI1.SetActive(false); // UI for the first part of the trade
                    Game.instance.tradingCardUI2.SetActive(true); // UI for the second part of the trade
                    //

                    

                    if (data.player.NbCard(data.cardName) > 1) // if player has more than one card
                    {

                        GameObject card = InstantiateNewCardObject(hit.transform.gameObject, data); // create a new card


                        stolenCard = card.transform; // set the stolen card to the create one
                        stolenCardPlayer = card.GetComponent<CardDisplayData>().player; // set the stolen player to the owner of the stolen card
                    }
                    else
                    {
                        stolenCard = hit.transform;// set the stolen card to the hit one
                        stolenCardPlayer = data.player;// set the stolen player to the owner of the stolen card

                    }

                    stolenCardPos = stolenCard.position; // set the stolenCardPos to its actuel location

                }
                else
                {
                    // set the state variables//
                    givenCardSelected = true;
                    selectingCard = true;
                    //

                    if (data.player.NbCard(data.cardName) > 1)// if player has more than one card
                    {

                        GameObject card = InstantiateNewCardObject(hit.transform.gameObject, data);



                        givenCard = card.transform;// set the given card to the create one
                        givenCardPlayer = card.GetComponent<CardDisplayData>().player;// set the given player to the owner of the given card
                    }
                    else
                    {
                        givenCard = hit.transform;// set the given card to the hit one
                        givenCardPlayer = data.player;// set the given player to the owner of the given card
                    }

                    SetChangeTradingState();

                }
            }
        }
        else
        {
            if (stolenCardSelected) // if a card selected
                DeselectTradingCard(); // deselect the stolen card
        }
    }

    public void SelectionTradingStateAI()
    {
        if (!stolenCardSelected)// if stolen card is not selected
        {
            int random = UnityEngine.Random.Range(0, Game.instance.numberOfPlayers);
            if (Game.instance.currentPlayerIndex == random)
            {
                if (random == Game.instance.numberOfPlayers - 1)
                {
                    random--;
                }
                else random++;
            }

            GameObject[] gos = Game.instance.players[random].cardObjects.Values.ToArray();
            int randomGo = UnityEngine.Random.Range(0, gos.Length);
            GameObject cardObject = gos[randomGo];

            CardDisplayData data = cardObject.transform.GetComponent<CardDisplayData>();

            if (data.player != null // if player defined
                && data.cardName != CardName.None // and if the card is not a monument
                && data.cardType != CardType.Building)// and if the card is not a building
            {
                // set the state variables//
                stolenCardSelected = true;
                selectingCard = true;
                //

                // set the UI Display //
                Game.instance.tradingCardUI1.SetActive(false); // UI for the first part of the trade
                Game.instance.tradingCardUI2.SetActive(true); // UI for the second part of the trade
                //

                if (data.player.NbCard(data.cardName) > 1)// if player has more than one card
                {

                    GameObject card = InstantiateNewCardObject(cardObject, data);// create a new card


                    stolenCard = card.transform;// set the stolen card to the create one
                    stolenCardPlayer = card.GetComponent<CardDisplayData>().player;// set the stolen player to the owner of the stolen card

                }
                else
                {
                    stolenCard = cardObject.transform;// set the stolen card to the hit one
                    stolenCardPlayer = data.player;// set the stolen player to the owner of the stolen card
                }
            }
        }
        else
        {
            GameObject[] gos = Game.instance.players[Game.instance.currentPlayerIndex].cardObjects.Values.ToArray();
            int randomGo = UnityEngine.Random.Range(0, gos.Length);
            GameObject cardObject = gos[randomGo];

            CardDisplayData data = cardObject.transform.GetComponent<CardDisplayData>();

            if (data.player != null // if player defined
                && data.cardName != CardName.None // and if the card is not a monument
                && data.cardType != CardType.Building)// and if the card is not a building
            {

                // set the state variables//
                givenCardSelected = true;
                selectingCard = true;
                //

                if (data.player.NbCard(data.cardName) > 1)// if player has more than one card
                {

                    GameObject card = InstantiateNewCardObject(cardObject, data);

                    givenCard = card.transform;// set the given card to the create one
                    givenCardPlayer = card.GetComponent<CardDisplayData>().player;// set the given player to the owner of the given card
                }
                else
                {
                    givenCard = cardObject.transform; // set the given card to the hit one
                    givenCardPlayer = data.player; // set the given player to the owner of the given card
                }

                SetChangeTradingState();

            }
        }

    }

    public void SetChangeTradingState()
    {
        // set the new parent of the two cards//
        stolenCard.SetParent(givenCardPlayer.cardContent, true);
        givenCard.SetParent(stolenCardPlayer.cardContent, true);
        //

        // remove the stolen card from the stolen player//
        stolenCardPlayer.PileCards[stolenCard.GetComponent<CardDisplayData>().cardName]--; //remove the stolen card of the stolen player
        if (stolenCardPlayer.NbCard(stolenCard.GetComponent<CardDisplayData>().cardName) == 0) // if there is no card of this type in the stolen player
            stolenCardPlayer.cardObjects.Remove(stolenCard.GetComponent<CardDisplayData>().cardName); // remove the card object
                                                                                                      //

        // remove the given card of the gievn player//
        givenCardPlayer.PileCards[givenCard.GetComponent<CardDisplayData>().cardName]--; //remove the given card of the given player
        if (givenCardPlayer.NbCard(givenCard.GetComponent<CardDisplayData>().cardName) == 0) // if there is no card of this type in the given player
            givenCardPlayer.cardObjects.Remove(givenCard.GetComponent<CardDisplayData>().cardName); // remove the card object
                                                                                                    //


        stolenCardPlayer.GiveCard(givenCard.GetComponent<CardDisplayData>().cardName); // give the given card to the stolen player
        givenCardPlayer.GiveCard(stolenCard.GetComponent<CardDisplayData>().cardName); // give the stolen card to the given player

        // change the pos of the card from the new ones//
        givenCardPos = stolenCardPlayer.cardContent.TransformPoint(stolenCardPlayer.AddToDict(givenCard.gameObject));
        stolenCardPos = givenCardPlayer.cardContent.TransformPoint(givenCardPlayer.AddToDict(stolenCard.gameObject));
        //


        // change the player owner of the cards
        stolenCard.GetComponent<CardDisplayData>().player = givenCardPlayer;
        givenCard.GetComponent<CardDisplayData>().player = stolenCardPlayer;
        //


        Game.instance.playerToSteal = stolenCardPlayer; // change the game value to know the stolen player


        Game.instance.tradingCardUI2.SetActive(false);// UI for the second part of the trade
    }

    void DeselectTradingCard()
    {
        // set the state variables//
        stolenCardDeselecting = true;
        stolenCardSelected = false;
        //
    }

    public void SelectPlayerToStealCoins(int index)
    {
        if (Game.instance.isStealingMoney) // if game is in stealing money state
        {
            Game.instance.playerToSteal = Game.instance.players[index]; // set the player to steal to the referenced player

            // set the state variables//
            Game.instance.isStealingMoney = false;
            Game.instance.playerMadeChoice = true;
            //
        }
    }



    void MoveCardToCam(Transform card, Del[] nextAction)
    {

        if (card.position != positionOfCard.position) // while targeted pos not reach (positionOfCard's position)
        {
            // set the card position to a vector to the targeted location with a delta of speed * Time.deltaTime
            card.position = Vector3.MoveTowards(card.position, positionOfCard.position, speed * Time.deltaTime);

            // set the card local scale to a vector to the targeted scale with a delta of (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude
            // (division usefull to reach the scale desired depending on the distance
            card.localScale = Vector3.MoveTowards(card.localScale, Vector3.one, (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude);
        }
        else
        {
            // do all the newt action referenced
            foreach (Del del in nextAction)
            {
                del();
            }
            //
        }
    }

    void MoveCardToPos(Transform card, Vector3 pos, Del[] nextAction)
    {
        if (card.position != pos)// while targeted pos not reach (pos's position)
        {
            // set the card position to a vector to the targeted location with a delta of speed * Time.deltaTime
            card.position = Vector3.MoveTowards(card.position, pos, speed * Time.deltaTime);

            // set the card local scale to a vector to the targeted scale with a delta of (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude
            // (division usefull to reach the scale desired depending on the distance
            card.localScale = Vector3.MoveTowards(card.localScale, card.GetComponent<CardDisplayData>().size * Vector3.one, (speed * Time.deltaTime) / (basePos - positionOfCard.position).magnitude);
        }
        else
        {
            foreach (Del del in nextAction)
            {
                del();
            }
        }
    }

    public GameObject InstantiateNewCardObject(GameObject obj, CardDisplayData data)
    {
        GameObject card = Instantiate(obj, obj.transform.parent); // instantiate a clone of obj with the same parent
        card.transform.position = obj.transform.position; // set the obj position to the clone

        // change all variable of the data//
        CardDisplayData data2 = card.GetComponent<CardDisplayData>();
        data2.cardName = data.cardName;
        data2.player = data.player;
        data2.size = data.size;
        data2.cardType = data.cardType;
        data2.x = data.x;
        data2.y = data.y;
        //

        return card; // return the created card
    }
}
