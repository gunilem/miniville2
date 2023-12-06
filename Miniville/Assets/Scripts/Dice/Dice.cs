using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] float gravity = 8f;
    [SerializeField] float torqueForce = 8f;
    [SerializeField] float throwForce = 2f;
    public Vector3 DicePosAtBegin;

    Rigidbody rb;

    public int result = -1;

    public bool StopDice = true;


    [SerializeField, Tooltip("to prevent a broken dice")] float cooldown = 0.1f;
    float timer;

    public void Start()
    {
        DicePosAtBegin = transform.position;

        rb = GetComponent<Rigidbody>();

        timer = cooldown;
    }

    public void Update()
    {
        if (StopDice) { return; }
        if (IsDiceHasStopped())
        {
            if (timer <= 0)
            {
                CalculateResult();
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        else
        {
            timer = cooldown;
        }
    }


    public void CalculateResult()
    {
        if (transform.right == Vector3.up)
        {
            result = 1;
        }
        else if (-transform.forward == Vector3.up)
        {
            result = 2;
        }
        else if (-transform.up == Vector3.up)
        {
            result = 3;
        }
        else if (transform.up == Vector3.up)
        {
            result = 4;
        }
        else if (transform.forward == Vector3.up)
        {
            result = 5;
        }
        else if (-transform.right == Vector3.up)
        {
            result = 6;
        }
        else
        {
            ThrowDice();
            return;
        }
        StopDice = true;
    }


    public void ResetDice(Vector3 pos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }

    public void ThrowDice()
    {
        timer = cooldown;
        result = -1;
        StopDice = false;

        int _xDec = (Random.Range(0, 2) * 2) - 1;
        int _yDec = (Random.Range(0, 2) * 2) - 1;
        int _zDec = (Random.Range(0, 2) * 2) - 1;

        Vector3 distanceToCenter = new Vector3(0, 7, 0) - transform.position;

        rb.AddForce(distanceToCenter * throwForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.left * torqueForce);

    }

    public bool IsDiceHasStopped()
    {
        return rb.velocity.magnitude < 0.0001;
    }
}
