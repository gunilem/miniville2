using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCollider : MonoBehaviour
{
    [SerializeField] Collider mc;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Dice"))
        {
            mc.enabled = true;
        }
    }
}
