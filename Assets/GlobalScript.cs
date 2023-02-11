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
        public GameObject table;
        public GameObject wall1;
        public GameObject wall2;
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
        }
            
        [PunRPC]
        public void PlayerJoin()
        {
            // In our PC scene, we have an ImageTarget object that we can update with the observerd real word size and then disable us.
            // It might still be desirable to keep this GameObject around, especially when troubleshooting image tracking related issues.
            // But you could also remove it entirely and just send the tracked data to the PC instead of instantiating a GameObject.
            if (players <= 2)
            {
                players++;
                Debug.Log("player: " + players);
                if (players == 1)
                {
                    startRound();
                }
            }
        }

        public void startRound()
        {
            table = PhotonNetwork.Instantiate("Belt", new Vector3(0f, 0.02f, 0f), Quaternion.identity);
            wall1 = PhotonNetwork.Instantiate("Barrier", new Vector3(-.5f, 0.02f, 0f), Quaternion.identity);
            wall2 = PhotonNetwork.Instantiate("Barrier", new Vector3(.5f, 0.02f, 0f), Quaternion.identity);

        }
    }
}
