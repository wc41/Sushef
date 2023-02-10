using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MyFirstARGame
{
    public class GlobalScript : MonoBehaviourPun
    {
        // Start is called before the first frame update

        public int players;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (players == 2)
            {
            }
        }
            
        [PunRPC]
        public void PlayerJoin()
        {
            // In our PC scene, we have an ImageTarget object that we can update with the observerd real word size and then disable us.
            // It might still be desirable to keep this GameObject around, especially when troubleshooting image tracking related issues.
            // But you could also remove it entirely and just send the tracked data to the PC instead of instantiating a GameObject.
            Debug.Log("player: " + players);
        }
    }
}
