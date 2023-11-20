using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class SFX_Dice : MonoBehaviour
    {
        public EventReference eventReference;
        private FMOD.Studio.EventInstance eventInstance;

        void Start()
        {
            eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        }

        void Update()
        {
            eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        }

        void OnCollisionEnter(Collision collision)
        {
            float speed = this.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if (speed > 2) { speed = 2; }

            eventInstance.setParameterByName("DICE_Velocity", speed / 2);
            eventInstance.start();
        }
    }
}