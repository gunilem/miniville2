using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonumentScript : MonoBehaviour
{
    public bool Activated;
    MonumentData monumentData;

    public MonumentScript(MonumentData data)
    {
        monumentData = data;
        Activated = false;
    }
}
