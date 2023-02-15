using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

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

        private int player1;
        private int player2;

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
                    int num = rnd.Next(31);
                    if (num < 10)
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("ifish", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
                    } else if (num >= 10 && num < 20)
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("irice", new Vector3(.6f, 0.22f, 0f), Quaternion.identity);
                    } else if (num >= 20 && num < 30)
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("iweed", new Vector3(.6f, 0.215f, 0f), Quaternion.identity);
                    } else
                    {
                        ingredients[tableId] = PhotonNetwork.Instantiate("iwasabi", new Vector3(.6f, 0.215f, 0f), Quaternion.identity);
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
        public void AddIngredientGlobal(int id, int workstation, int playerID)
        {
            if (workstation == 1)
            {
                workstation1.GetPhotonView().RPC("AddIngredient", RpcTarget.Others, id, playerID);
            } if (workstation == 2)
            {
                workstation2.GetPhotonView().RPC("AddIngredient", RpcTarget.Others, id, playerID);
            }
        }

        [PunRPC]
        public void TrashWasabiGlobal(int id, int workstation, int playerID)
        {
            if (workstation == 1)
            {
                workstation1.GetPhotonView().RPC("TrashWasabi", RpcTarget.Others, id, playerID);
            }
            if (workstation == 2)
            {
                workstation2.GetPhotonView().RPC("TrashWasabi", RpcTarget.Others, id, playerID);
            }
        }

        [PunRPC]
        public void WinState (int playerID)
        {
            if (playerID == player1)
            {
                workstation2.GetPhotonView().RPC("Lose", RpcTarget.Others, playerID);
            }
            if (playerID == player2)
            {
                workstation1.GetPhotonView().RPC("Lose", RpcTarget.Others, playerID);
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
        public void HoldToCreate(int ID, int playerID)
        {
            if (ID == 1)
            {
                workstation1.GetPhotonView().RPC("Cook", RpcTarget.Others, playerID);
            }
            if (ID == 2)
            {
                workstation2.GetPhotonView().RPC("Cook", RpcTarget.Others, playerID);
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
        public void ReadyPlayer1(int ID, int playerID)
        {
            Debug.Log("ready player 1");
            workstation1 = PhotonView.Find(ID).gameObject;
            player1 = playerID;
        }

        [PunRPC]
        public void ReadyPlayer2(int ID, int playerID)
        {
            Debug.Log("ready player 2");
            workstation2 = PhotonView.Find(ID).gameObject;
            player2 = playerID;
        }

        [PunRPC]
        public void SetHost()
        {
            isHost = true;
        }

        [PunRPC]
        public void Trash(int id, int workstation, int playerID)
        {
            Debug.Log("globalscript trash called");
            if (workstation == 1)
            {
                workstation1.GetPhotonView().RPC("TrashIngredientOmg", RpcTarget.Others, id, playerID);
            }
            if (workstation == 2)
            {
                workstation2.GetPhotonView().RPC("TrashIngredientOmg", RpcTarget.Others, id, playerID);
            }
        }

        [PunRPC]
        public void Sushi(int id, int workstation, int playerID)
        {
            Debug.Log("globalscript sushi called");
            if (workstation == 1)
            {
                workstation1.GetPhotonView().RPC("MadeSushi", RpcTarget.Others, id, playerID);
            }
            if (workstation == 2)
            {
                workstation2.GetPhotonView().RPC("MadeSushi", RpcTarget.Others, id, playerID);
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
            wall1 = PhotonNetwork.Instantiate("Barrier", new Vector3(-.54f, 0.02f, 0f), Quaternion.identity);
            wall2 = PhotonNetwork.Instantiate("Barrier", new Vector3(.54f, 0.02f, 0f), Quaternion.identity);
        }
    }
}
