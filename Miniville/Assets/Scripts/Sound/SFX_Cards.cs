using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class SFX_Cards : CardSelector
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
                    FMODUnity.RuntimeManager.PlayOneShot(cardToFront, GetComponent<Transform>().position);
                    Debug.Log("Play sound : cardToFront");
                    break;

                case "cardToBack":
                    FMODUnity.RuntimeManager.PlayOneShot(cardToBack, GetComponent<Transform>().position);
                    Debug.Log("Play sound : cardToBack");
                    break;

                case "cardFlip":
                    FMODUnity.RuntimeManager.PlayOneShot(cardFlip, GetComponent<Transform>().position);
                    Debug.Log("Play sound : cardFlip");
                    break;

                default:
                    Debug.Log("soundName n'a pas de nom valide");
                    break;
            }
        }
    }
}