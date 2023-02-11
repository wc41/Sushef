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
        public GameObject[] tables;
        public GameObject wall1;
        public GameObject wall2;
        private float time;
        private int tableId;
        public bool roundStarted;
        void Start()
        {
            roundStarted = false;
            time = 0;
            tableId = 0;
            tables = new GameObject[10];
        }

        // Update is called once per frame
        void Update()
        {
            if (roundStarted)
            {
                time += Time.deltaTime;
                for (int i = 0; i < 10; i++)
                {
                    if (tables[i] != null)
                    {
                        tables[i].transform.Translate(-0.3f * Time.deltaTime, 0, 0);
                    }
                }

                if (time > 0.4f)
                {
                    time = 0f;
                    if (tables[tableId] != null)
                    {
                        PhotonNetwork.Destroy(tables[tableId]);
                    }

                    tables[tableId] = PhotonNetwork.Instantiate("Belt", new Vector3(.6f, 0.02f, 0f), Quaternion.identity);
                    tableId++;
                    if (tableId > 9)
                    {
                        tableId = 0;
                    }
                }
            }

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
                    beginRound();
                }
            }
        }

        public void beginRound()
        {
            time = 0;
            roundStarted = true;
            tables[0] = PhotonNetwork.Instantiate("Belt", new Vector3(.6f, 0.02f, 0f), Quaternion.identity);
            tableId++;
            wall1 = PhotonNetwork.Instantiate("Barrier", new Vector3(-.5f, 0.02f, 0f), Quaternion.identity);
            wall2 = PhotonNetwork.Instantiate("Barrier", new Vector3(.5f, 0.02f, 0f), Quaternion.identity);
        }
    }
}
