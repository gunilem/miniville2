using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public GameObject coin1;
    public GameObject coin5;
    public GameObject coin10;

    public int playerMoney;
    private int prePlayerMoney = 0;
    public bool updateMoney;

    private List<GameObject> displayedCoin = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        updateMoney = false;
    }

    void Update()
    {
        if (playerMoney < prePlayerMoney && updateMoney)
        {
            RemoveCoins(prePlayerMoney - playerMoney);
            prePlayerMoney = playerMoney;
        }
        else if (playerMoney > prePlayerMoney && updateMoney)
        {
            InstantiateCoins(playerMoney - prePlayerMoney);
            prePlayerMoney = playerMoney;
        }

        updateMoney = false;
    }

    private void InstantiateCoins(int nbCoins)
    {
        int coinToInstantiate = nbCoins;

        while (coinToInstantiate >= 10)
        {
            displayedCoin.Add(Instantiate(coin10, gameObject.transform));
            coinToInstantiate -= 10;
        }

        while (coinToInstantiate >= 5)
        {
            displayedCoin.Add(Instantiate(coin5, gameObject.transform));
            coinToInstantiate -= 5;
        }

        while (coinToInstantiate >= 1)
        {
            displayedCoin.Add(Instantiate(coin1, gameObject.transform));
            coinToInstantiate -= 1;
        }
    }

    private void RemoveCoins(int nbCoins)
    {
        int nbCoinToRemove = nbCoins;
        List<GameObject> coinsToRemove = new List<GameObject>();
        int coinsToAdd = 0;

        if (playerMoney >= nbCoinToRemove)
        {
            for (int i = nbCoinToRemove; i >= 10; i -= 10)
            {
                foreach (GameObject coin in displayedCoin)
                {
                    if (coin.name == "Coins_10(Clone)")
                    {
                        coinsToRemove.Add(coin);
                        displayedCoin.Remove(coin);
                        nbCoinToRemove -= 10;
                        break;
                    }
                }
            }

            for (int i = nbCoinToRemove; i >= 5; i -= 5)
            {
                bool coinRemoved = false;

                foreach (GameObject coin in displayedCoin)
                {
                    if (coin.name == "Coins_5(Clone)")
                    {
                        coinsToRemove.Add(coin);
                        displayedCoin.Remove(coin);
                        nbCoinToRemove -= 5;
                        coinRemoved = true;
                        break;
                    }
                }
                
                if (!coinRemoved)
                {
                    foreach (GameObject coin in displayedCoin)
                    {
                        if (coin.name == "Coins_10(Clone)")
                        {
                            coinsToRemove.Add(coin);
                            displayedCoin.Remove(coin);
                            coinsToAdd += 5;
                            nbCoinToRemove -= 5;
                            coinRemoved = true;
                            break;
                        }
                    }
                }
            }

            for (int i = nbCoinToRemove; i >= 1; i -= 1)
            {
                bool coinRemoved = false;

                foreach (GameObject coin in displayedCoin)
                {
                    if (coin.name == "Coins_1(Clone)")
                    {
                        coinsToRemove.Add(coin);
                        displayedCoin.Remove(coin);
                        nbCoinToRemove -= 1;
                        coinRemoved = true;
                        break;
                    }
                }

                if (!coinRemoved)
                {
                    foreach (GameObject coin in displayedCoin)
                    {
                        if (coin.name == "Coins_5(Clone)")
                        {
                            coinsToRemove.Add(coin);
                            displayedCoin.Remove(coin);
                            coinsToAdd += 4;
                            nbCoinToRemove -= 1;
                            coinRemoved = true;
                            break;
                        }
                    }

                    if (!coinRemoved)
                    {
                        foreach (GameObject coin in displayedCoin)
                        {
                            if (coin.name == "Coins_10(Clone)")
                            {
                                coinsToRemove.Add(coin);
                                displayedCoin.Remove(coin);
                                coinsToAdd += 9;
                                nbCoinToRemove -= 1;
                                coinRemoved = true;
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < coinsToRemove.Count; i++)
            {
                GameObject coin = coinsToRemove[i];
                Destroy(coin);
            }

            if (coinsToAdd > 0)
                InstantiateCoins(coinsToAdd);
        }
    }
}
