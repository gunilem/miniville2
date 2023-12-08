using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_HandlerDestroyer : MonoBehaviour
{
    public void DestroyBGMHandler()
    {
        GameObject.Find("MusicHandler").GetComponent<BGM_Handler>().DestroyInstance();
    }
}
