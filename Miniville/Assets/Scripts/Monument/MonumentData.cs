using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Monument")]
public class MonumentData : ScriptableObject
{
    public MonumentName monumentName;
    [HideInInspector]public CardType type = CardType.Building;
    public Material headMaterial;
    public Material tailMaterial;
    public int Cost;
}
