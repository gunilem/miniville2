using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class SFX_Cards : MonoBehaviour
    {
        public EventReference cardToFront;
        public EventReference cardToBack;
        public EventReference cardFlip;

        public void PlaySound(string soundName, Transform selectedCard)
        {
            // ----- SOUNDNAME -----
            //   - cardToFront
            //   - cardToBack
            //   - cardFlip
            // ---------------------            

            switch (soundName)
            {
                case "cardToFront":
                    // Mettre le PlayOneShot
                    break;

                case "cardToBack":
                    // Mettre le PlayOneShot
                    break;

                case "cardFlip":
                    // Mettre le PlayOneShot
                    break;

                default:
                    Debug.Log("soundName n'a pas de nom valide");
                    break;
            }
        }
    }
}