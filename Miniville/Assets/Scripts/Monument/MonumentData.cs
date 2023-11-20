using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Monument")]
public class MonumentData : ScriptableObject
{
    public MonumentName monumentName;
    [HideInInspector]public CardType type = CardType.Building;
    public Material material;
    public Material materialVerso;
    public int Cost;
}
