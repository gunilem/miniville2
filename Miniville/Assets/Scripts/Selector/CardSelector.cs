using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CardSelector : MonoBehaviour
{
    public Camera camera;
    public LayerMask mask;

    public Vector3 basePos;
    public Transform Selectedcard;

    public GameObject canva;

    public float speed = 2.0f;
    public Transform positionOfCard;

    bool isSelectingCard = false;
    bool isDeselectingCard = false;

    bool ifCardSelected = false;
    bool isRotated = false;

    public SFX_Cards sfx;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Selectedcard == null) SelectCard();
            if (ifCardSelected) FlipCard();
        }
        if (isSelectingCard)
        {
            if (Selectedcard.position != positionOfCard.position)
            {
                Selectedcard.position = Vector3.MoveTowards(Selectedcard.position, positionOfCard.position, speed * Time.deltaTime);
            }
            else
            {
                isSelectingCard = false;
                ifCardSelected = true;
                canva.SetActive(true);
            }
        }
        if (isDeselectingCard)
        {
            if (Selectedcard.position != basePos)
            {
                Selectedcard.position = Vector3.MoveTowards(Selectedcard.position, basePos, speed * Time.deltaTime);
            }
            else
            {
                isDeselectingCard = false;
                Selectedcard = null;
                basePos = Vector3.zero;
            }
        }
    }

    private void SelectCard()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            basePos = hit.transform.position;
            Selectedcard = hit.transform;
            isSelectingCard = true;

            //SFX
            sfx.PlaySound("cardToFront", Selectedcard);
        }
        
    }

    public void DeselectCard()
    {
        isDeselectingCard = true;
        ifCardSelected = false;
        if (isRotated)
        {
            isRotated = false;
            Selectedcard.Rotate(Vector3.right, 180);

            //SFX
            sfx.PlaySound("cardToBack", Selectedcard);
        }
        canva.SetActive(false);
    }

    public void FlipCard()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            Selectedcard.Rotate(Vector3.right, 180);
            isRotated = !isRotated;

            //SFX
            sfx.PlaySound("cardFlip", Selectedcard);
        }
    }
}
