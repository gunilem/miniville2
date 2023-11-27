using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public GameObject Coins1;
    public GameObject Coins5;
    public GameObject Coins10;

    [SerializeField] float ejectMaxSpeed = 500f;
    [SerializeField] float ejectMinSpeed = 200f;

    public List<Player> players = new List<Player>();

    private List<int> prePlayersMoney = new List<int>();
    public bool updatePlayer;

    private List<List<GameObject>> displayedCoinsPerPlayer = new List<List<GameObject>>();

    void Start()
    {
        for (int i = 0; i < players.Count; i++)
        {
            prePlayersMoney.Add(0);
            displayedCoinsPerPlayer.Add(new List<GameObject>());
        }

        updatePlayer = false;
        
    }

    public void UpdatePlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Coins > prePlayersMoney[i])
            {
                InstantiateCoins(players[i], players[i].Coins - prePlayersMoney[i]);
                prePlayersMoney[i] = players[i].Coins;
            }

            if (players[i].Coins < prePlayersMoney[i])
            {
                RemoveCoins(players[i], prePlayersMoney[i] - players[i].Coins);
                prePlayersMoney[i] = players[i].Coins;
            }
        }
    }

    private void InstantiateCoins(Player player, int nbCoins)
    {
        int CoinsToInstantiate = nbCoins;
        bool instantiated = false;
        int nbOfCoins1 = 0;

        while (CoinsToInstantiate >= 10)
        {
            displayedCoinsPerPlayer[players.IndexOf(player)].Add(Instantiate(Coins10, player.coinInstantiatePos.transform));
            CoinsToInstantiate -= 10;
        }

        while (CoinsToInstantiate >= 5)
        {
            instantiated = false;
            foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
            {
                if (Coins.name == "Coins_5(Clone)")
                {
                    RemoveCoins(player, 5);
                    InstantiateCoins(player, 10);
                    CoinsToInstantiate -= 5;
                    instantiated = true;
                    break;
                }
            }
            if (!instantiated)
            {
                displayedCoinsPerPlayer[players.IndexOf(player)].Add(Instantiate(Coins5, player.coinInstantiatePos.transform));
                CoinsToInstantiate -= 5;
            }
        }

        while (CoinsToInstantiate >= 1)
        {
            nbOfCoins1 = 0;
            instantiated = false;
            foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
            {
                if (Coins.name == "Coins_1(Clone)")
                    nbOfCoins1++;
            }

            if (nbOfCoins1 == 4)
            {
                RemoveCoins(player, 1);
                RemoveCoins(player, 1);
                RemoveCoins(player, 1);
                RemoveCoins(player, 1);

                InstantiateCoins(player, 5);
                CoinsToInstantiate -= 1;
            }
            else
            {
                displayedCoinsPerPlayer[players.IndexOf(player)].Add(Instantiate(Coins1, player.coinInstantiatePos.transform));
                CoinsToInstantiate -= 1;
            }
        }
    }

    private void RemoveCoins(Player player, int nbCoins)
    {
        int nbCoinsToRemove = nbCoins;
        List<GameObject> CoinsToRemove = new List<GameObject>();
        int CoinsToAdd = 0;

        // ENLEVE LES PIECES DE 10
        for (int i = nbCoinsToRemove; i >= 10; i -= 10)
        {
            foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
            {
                if (Coins.name == "Coins_10(Clone)")
                {
                    CoinsToRemove.Add(Coins);
                    displayedCoinsPerPlayer[players.IndexOf(player)].Remove(Coins);
                    nbCoinsToRemove -= 10;
                    break;
                }
            }
        }

        // ENLEVE LES PIECES DE 5
        for (int i = nbCoinsToRemove; i >= 5; i -= 5)
        {
            bool CoinsRemoved = false;

            foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
            {
                if (Coins.name == "Coins_5(Clone)")
                {
                    CoinsToRemove.Add(Coins);
                    displayedCoinsPerPlayer[players.IndexOf(player)].Remove(Coins);
                    nbCoinsToRemove -= 5;
                    CoinsRemoved = true;
                    break;
                }
            }
                
            if (!CoinsRemoved)
            {
                foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
                {
                    if (Coins.name == "Coins_10(Clone)")
                    {
                        CoinsToRemove.Add(Coins);
                        displayedCoinsPerPlayer[players.IndexOf(player)].Remove(Coins);
                        CoinsToAdd += 5;
                        nbCoinsToRemove -= 5;
                        CoinsRemoved = true;
                        break;
                    }
                }
            }
        }

        // ENLEVE LES PIECES DE 1
        for (int i = nbCoinsToRemove; i >= 1; i -= 1)
        {
            bool CoinsRemoved = false;

            foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
            {
                if (Coins.name == "Coins_1(Clone)")
                {
                    CoinsToRemove.Add(Coins);
                    displayedCoinsPerPlayer[players.IndexOf(player)].Remove(Coins);
                    nbCoinsToRemove -= 1;
                    CoinsRemoved = true;
                    break;
                }
            }

            if (!CoinsRemoved)
            {
                foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
                {
                    if (Coins.name == "Coins_5(Clone)")
                    {
                        CoinsToRemove.Add(Coins);
                        displayedCoinsPerPlayer[players.IndexOf(player)].Remove(Coins);
                        CoinsToAdd += 4;
                        nbCoinsToRemove -= 1;
                        CoinsRemoved = true;
                        break;
                    }
                }

                if (!CoinsRemoved)
                {
                    foreach (GameObject Coins in displayedCoinsPerPlayer[players.IndexOf(player)])
                    {
                        if (Coins.name == "Coins_10(Clone)")
                        {
                            CoinsToRemove.Add(Coins);
                            displayedCoinsPerPlayer[players.IndexOf(player)].Remove(Coins);
                            CoinsToAdd += 9;
                            nbCoinsToRemove -= 1;
                            CoinsRemoved = true;
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < CoinsToRemove.Count; i++)
        {
            GameObject Coins = CoinsToRemove[i];

            Coins.GetComponent<MeshCollider>().enabled = false;
            Coins.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10, 11), Random.Range(1, 11), Random.Range(-10, 11)).normalized * Random.Range(ejectMinSpeed, ejectMaxSpeed), ForceMode.Impulse);
            Coins.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(1, 11), Random.Range(1, 11), Random.Range(1, 11)).normalized * Random.Range(ejectMinSpeed, ejectMaxSpeed));

            StartCoroutine(MyCoroutine(Coins));
        }

        if (CoinsToAdd > 0)
            InstantiateCoins(player, CoinsToAdd);
    }

    IEnumerator MyCoroutine(GameObject Coins)
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(Coins);
    }
}
