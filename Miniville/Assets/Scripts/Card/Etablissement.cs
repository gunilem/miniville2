using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Etablissement : MonoBehaviour
{
    [SerializeField] CardName cardName;
    [SerializeField] MeshRenderer meshRenderer;
    void Start()
    {
        Material[] materialsArray = meshRenderer.materials;
        materialsArray[2] = AllCards.CardsData[cardName].material;
        meshRenderer.materials = materialsArray;
    }
    // Update is called once per frame
    void Update()
    {

    }
}