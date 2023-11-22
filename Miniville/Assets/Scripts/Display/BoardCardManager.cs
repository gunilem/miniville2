using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardCardManager : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;

    public Dictionary<CardName, GameObject> cardObjects = new Dictionary<CardName, GameObject>();

    public float xOffSet = 1f;
    public float yOffSet = 1.75f;

    public float cardSizeMultiplier = 0.75f;

    int x = 0;
    int z = 0;

    public int cardPerRow = 5;

    public bool reload = false;

    public void ReloadCard()
    {
        List<CardName> cards = Game.instance.PileCards.Keys.ToList<CardName>();

        for (int i = 0; i < cards.Count; i++)
        {
            if (Game.instance.PileCards[cards[i]] != 0)
            {
                if (!cardObjects.ContainsKey(cards[i]))
                {
                    GameObject card = Instantiate(cardPrefab, transform);
                    card.transform.localScale *= cardSizeMultiplier;
                    card.transform.position += transform.right * (x % cardPerRow) * xOffSet * cardSizeMultiplier + transform.forward * ((int)(z / cardPerRow)) * yOffSet * cardSizeMultiplier;
                    card.transform.rotation = Quaternion.LookRotation(-transform.forward);

                    card.GetComponent<CardDisplayData>().CardName = cards[i];

                    ChangeMaterial(AllCards.CardsData[cards[i]].material, card);

                    cardObjects[cards[i]] = card;
                    x++;
                    z++;
                }


            }
            else if (cardObjects.ContainsKey(cards[i]))
            {
                Destroy(cardObjects[cards[i]]);
                cardObjects.Remove(cards[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (reload)
        {
            reload = false;
            ReloadCard();
        }
    }

    void ChangeMaterial(Material material, GameObject go)
    {
        Material[] materialsArray = go.GetComponent<MeshRenderer>().materials;
        materialsArray[2] = material;
        go.GetComponent<MeshRenderer>().materials = materialsArray;
    }
}
