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

    public int spawnCoin;
    public bool spawn;

    public int destroyCoin;
    public bool destroy;

    private List<GameObject> displayedCoin = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InstantiateCoins(spawnCoin);
        destroy = false;
    }

    void Update()
    {
        if (destroyCoin != 0 && destroy)
        {
            RemoveCoins(destroyCoin);
            destroy = false;
        }

        if (spawnCoin != 0 && spawn)
        {
            InstantiateCoins(spawnCoin);
            spawn = false;
        }
    }

    private void InstantiateCoins(int nbCoins)
    {
        int coinToInstantiate = nbCoins;

        while (coinToInstantiate >= 10)
        {
            displayedCoin.Add(coin10);
            coinToInstantiate -= 10;
        }

        while (coinToInstantiate >= 5)
        {
            displayedCoin.Add(coin5);
            coinToInstantiate -= 5;
        }

        while (coinToInstantiate >= 1)
        {
            displayedCoin.Add(coin1);
            coinToInstantiate -= 1;
        }

        foreach (GameObject coin in displayedCoin)
            CoinsManager.Instantiate(coin, gameObject.transform);
    }

    private void RemoveCoins(int nbCoins)
    {
        int nbCoinToRemove = nbCoins;
        List<GameObject> coinsToRemove = new List<GameObject>();
        int coinsToAdd = 0;

        Debug.Log(nbCoinToRemove);

        if (playerMoney >= nbCoinToRemove)
        {
            for (int i = nbCoinToRemove; i >= 10; i -= 10)
            {
                Debug.Log("Coin 10 degage ptn");
                GameObject coinToRemove = displayedCoin.FirstOrDefault(objet => objet.name == "Coins_10(Clone)");

                if (coinToRemove != null)
                {
                    Debug.Log("Nik mamie");
                    coinsToRemove.Add(coinToRemove);
                    displayedCoin.Remove(coinToRemove);
                    nbCoinToRemove -= 10;
                }
            }

            for (int i = nbCoinToRemove; i >= 5; i -= 5)
            {
                Debug.Log("Coin 5 degage ptn");
                GameObject coinToRemove = displayedCoin.FirstOrDefault(objet => objet.name == "Coins_5(Clone)");

                if (coinToRemove != null)
                {
                    Debug.Log("Nik mamie");
                    coinsToRemove.Add(coinToRemove);
                    displayedCoin.Remove(coinToRemove);
                    nbCoinToRemove -= 5;
                }
                else
                {
                    GameObject coinToRemove2 = displayedCoin.FirstOrDefault(objet => objet.name == "Coins_10(Clone)");
                    if (coinToRemove2 != null)
                    {
                        coinsToRemove.Add(coinToRemove2);
                        displayedCoin.Remove(coinToRemove2);
                        coinsToAdd += 5;
                        nbCoinToRemove -= 5;
                    }
                }
            }

            for (int i = nbCoinToRemove; i >= 1; i -= 1)
            {
                Debug.Log("Coin 1 degage ptn");
                GameObject coinToRemove = displayedCoin.FirstOrDefault(objet => objet.name == "Coins_1(Clone)");

                if (coinToRemove != null)
                {
                    Debug.Log("Nik mamie");
                    coinsToRemove.Add(coinToRemove);
                    displayedCoin.Remove(coinToRemove);
                    nbCoinToRemove -= 1;
                }
                else
                {
                    GameObject coinToRemove2 = displayedCoin.FirstOrDefault(objet => objet.name == "Coins_5(Clone)");
                    if (coinToRemove2 != null)
                    {
                        coinsToRemove.Add(coinToRemove2);
                        displayedCoin.Remove(coinToRemove2);
                        coinsToAdd += 4;
                        nbCoinToRemove -= 1;
                    }
                    else
                    {
                        GameObject coinToRemove3 = displayedCoin.FirstOrDefault(objet => objet.name == "Coins_10(Clone)");
                        if (coinToRemove3 != null)
                        {
                            coinsToRemove.Add(coinToRemove3);
                            displayedCoin.Remove(coinToRemove3);
                            coinsToAdd += 9;
                            nbCoinToRemove -= 1;
                        }
                    }
                }
            }

            Debug.Log("ptn");

            foreach (GameObject coin in coinsToRemove)
            {
                Destroy(coin);
                Debug.Log(coin.ToString());
            }

            if (coinsToAdd > 0)
                InstantiateCoins(coinsToAdd);
        }
        else
            Debug.Log("Pas assez de tune salope");
    }
}
