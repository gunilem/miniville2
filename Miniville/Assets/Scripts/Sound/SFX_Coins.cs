using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class SFX_Coins : MonoBehaviour
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
            float speed = this.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if (speed > 5) { speed = 5; }

            if (other.tag == "Coin" && other != this.gameObject.GetComponent<Collider>())
                eventInstance.setParameterByName("COIN_Collider", 0f);

            if (other.tag == "Table" && other != this.gameObject.GetComponent<Collider>())
                eventInstance.setParameterByName("COIN_Collider", 1f);

            eventInstance.setParameterByName("COIN_Velocity", speed / 5);
            eventInstance.start();

            /*// DEBUG
            float debug;
            eventInstance.getParameterByName("COIN_Collider", out debug);
            Debug.Log("Velocity : " + (speed / 5) + "MaterialFMOD : " + debug);*/
        }
    }
}