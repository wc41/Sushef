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

        // Game objects
        public GameObject[] tables;
        public GameObject[] ingredients;

        public GameObject wall1;
        public GameObject wall2;
        public GameObject workstation1;
        public GameObject workstation2;
        public GameObject emptyPlaneTracker;

        private float time;
        private int tableId;
        public bool roundStarted;
        public bool isHost;

        private bool ready1;
        private bool ready2;

        void Awake()
        {
            roundStarted = false;
            time = 0;
            tableId = 0;
            tables = new GameObject[10];
            ingredients = new GameObject[10];

            ready1 = false;
            ready2 = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (roundStarted && isHost)
            {
                time += Time.deltaTime;
                for (int i = 0; i < 10; i++)
                {
                    if (tables[i] != null)
                    {
                        tables[i].transform.Translate(-0.3f * Time.deltaTime, 0, 0);
                    }
                    if (ingredients[i] != null)
                    {
                        ingredients[i].transform.Translate(-0.3f * Time.deltaTime, 0, 0);
                    }
                }

                if (time > 0.4f)
                {
                    time = 0f;
                    if (tables[tableId] != null)
                    {
                        PhotonNetwork.Destroy(tables[tableId]);
                    }
                    if (ingredients[tableId] != null) {
                        PhotonNetwork.Destroy(ingredients[tableId]);
                    }

                    tables[tableId] = PhotonNetwork.Instantiate("Belt", new Vector3(.6f, 0.2f, 0f), Quaternion.identity);

                    System.Random rnd = new System.Random();
                    int num = rnd.Next(3);
                    if (num == 0)
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("ifish", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
                    } else if (num == 1)
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("irice", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
                    }
                    else if (num == 2)
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("iweed", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
                    }
                    tableId++;
                    if (tableId > 9)
                    {
                        tableId = 0;
                    }
                }
            } else if (isHost && !roundStarted && (ready1 || ready2))
            {
                beginRound();
            }
        }

        [PunRPC]
        public void AddIngredientGlobal(int id, int workstation)
        {
            if (workstation == 1)
            {
                workstation1.GetPhotonView().RPC("AddIngredient", RpcTarget.Others, id);
            } if (workstation == 2)
            {
                workstation2.GetPhotonView().RPC("AddIngredient", RpcTarget.Others, id);
            }
        }

        [PunRPC]
        public void TakeIngredientAway(int id)
        {
            Debug.Log("$$$ picking up ingredient");
            for (int i = 0; i < 10; i++)
            {
                if (ingredients[i] != null)
                {
                    if (ingredients[i].GetComponent<PhotonView>().ViewID == id)
                    {
                        ingredients[i] = null;
                        return;
                    }
                }
            }

            Debug.Log("$$$ ingredient not found from conveyor belt");

            if (workstation1 != null)
            {
                workstation1.GetPhotonView().RPC("RemoveIngredient", RpcTarget.Others, id);
            }
            if (workstation2 != null)
            {
                workstation2.GetPhotonView().RPC("RemoveIngredient", RpcTarget.Others, id);
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
                if (!roundStarted && players == 1 && isHost)
                {
                    beginRound();
                }
            }
        }

        [PunRPC]
        public void ReadyPlayer1(int ID)
        {
            Debug.Log("ready player 1");
            workstation1 = PhotonNetwork.Instantiate("Workstation", new Vector3(0f, 0f, 0.2f), Quaternion.identity);
            workstation1.GetComponent<PhotonView>().TransferOwnership(ID);
        }

        [PunRPC]
        public void ReadyPlayer2(int ID)
        {
            Debug.Log("ready player 2");
            workstation2 = PhotonNetwork.Instantiate("Workstation", new Vector3(0f, 0f, -0.2f), Quaternion.Euler(0, 180, 0));
            workstation2.GetComponent<PhotonView>().TransferOwnership(ID);
        }

        [PunRPC]
        public void SetHost()
        {
            isHost = true;
        }

        [PunRPC]
        public void Trash(int id, int workstation)
        {
            Debug.Log("globalscript trash called");
            if (workstation == 1)
            {
                workstation1.GetPhotonView().RPC("TrashIngredientOmg", RpcTarget.Others, id);
            }
            if (workstation == 2)
            {
                workstation2.GetPhotonView().RPC("TrashIngredientOmg", RpcTarget.Others, id);
            }
        }

        public bool IsHost()
        {
            return isHost;
        }
 
        public void beginRound()
        {
            time = 0;
            roundStarted = true;
            tables[0] = PhotonNetwork.Instantiate("Belt", new Vector3(.6f, 0.2f, 0f), Quaternion.identity);

            emptyPlaneTracker = PhotonNetwork.Instantiate("planeTrack", new Vector3(0f, 0.19f, 0f), Quaternion.identity);
            if (emptyPlaneTracker == null)
            {
                Debug.Log("$$$ failed to instantiate plane tracker");
            }

            System.Random rnd = new System.Random();
            int num = rnd.Next(3);
            if (num == 0)
            {
                ingredients[0] = PhotonNetwork.Instantiate("ifish", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
            }
            else if (num == 1)
            {
                ingredients[0] = PhotonNetwork.Instantiate("irice", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
            }
            else if (num == 2)
            {
                ingredients[0] = PhotonNetwork.Instantiate("iweed", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
            }
            
            tableId++;
            wall1 = PhotonNetwork.Instantiate("Barrier", new Vector3(-.5f, 0.02f, 0f), Quaternion.identity);
            wall2 = PhotonNetwork.Instantiate("Barrier", new Vector3(.5f, 0.02f, 0f), Quaternion.identity);
        }
    }
}
