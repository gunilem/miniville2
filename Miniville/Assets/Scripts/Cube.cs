using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool throwDice;
    [SerializeField] float gravity = 8f;
    [SerializeField] float torqueForce = 8f;
    Rigidbody rb;

    float xprevious;
    float yprevious;
    float zprevious;

    int result;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xprevious = transform.position.x;
        yprevious = transform.position.y;
        zprevious = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (throwDice)
        {
            rb.AddForce(new Vector3(2f,3f,2f), ForceMode.Impulse);
            rb.AddTorque(Vector3.left * torqueForce);
            throwDice = false;
        }
        
    }

    private void FixedUpdate()
    {
        if (transform.position.x != xprevious && transform.position.y != yprevious && transform.position.z != zprevious)
        {
            Gravity();
        }
        else
        {
            GetDiceResult();
        }

        xprevious = transform.position.x;
        yprevious = transform.position.y;
        zprevious = transform.position.z;
    }

    void Gravity()
    { 
        rb.AddForce(Vector3.down * gravity * Time.deltaTime);
    }

    void GetDiceResult()
    {
        Vector3[] vecteursComparaison = new Vector3[]
        {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right,
                Vector3.up,
                Vector3.down
        };

        Vector3 forwardVector = transform.forward;

        forwardVector.x = Mathf.Round(forwardVector.x);
        forwardVector.y = Mathf.Round(forwardVector.y);
        forwardVector.z = Mathf.Round(forwardVector.z);

        Vector3 upVector = transform.up;

        upVector.x = Mathf.Round(upVector.x);
        upVector.y = Mathf.Round(upVector.y);
        upVector.z = Mathf.Round(upVector.z);

        Vector3 rightVector = transform.right;

        rightVector.x = Mathf.Round(rightVector.x);
        rightVector.y = Mathf.Round(rightVector.y);
        rightVector.z = Mathf.Round(rightVector.z);

        
        if (forwardVector.x == 0f && forwardVector.y == -1f && forwardVector.z == 0f)
        {
            DiceSetResult(2);
        }
        else if (forwardVector.x == 0f && forwardVector.y == 1f && forwardVector.z == 0f)
        {
            DiceSetResult(5);
        }
        else if (upVector.x == 0f && upVector.y == -1f && upVector.z == 0f)
        {
            DiceSetResult(3);
        }
        else if (upVector.x == 0f && upVector.y == 1f && upVector.z == 0f)
        {
            DiceSetResult(4);
        }
        else if (rightVector.x == 0f && rightVector.y == -1f && rightVector.z == 0f)
        {
            DiceSetResult(6);
        }
        else if (rightVector.x == 0f && rightVector.y == 1f && rightVector.z == 0f)
        {
            DiceSetResult(1);
        }
    }

    private void DiceSetResult(int res)
    {
        result = res;
        UnityEngine.Debug.Log(res);
    }
}
