using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class SD_Coins : MonoBehaviour
    {
        public EventReference eventReference;
        private FMOD.Studio.EventInstance eventInstance;

        // Start is called before the first frame update
        void Start()
        {
            eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);

        }

        // Update is called once per frame
        void Update()
        {
            eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Coin" && other != this.gameObject.GetComponent<Collider>())
            {
                eventInstance.setParameterByName("COIN_Collider", 0f);
                eventInstance.setParameterByName("COIN_Velocity", this.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                eventInstance.start();
            }
        }
    }
}