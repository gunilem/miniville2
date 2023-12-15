using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card")]
public class CardData : ScriptableObject //le scriptable object des cartes
{
    public CardName cardName;
    public CardType type;
    public CardColor color;
    public Material material;
    public int[] Dice;
    public int Cost;

}
