using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace MyFirstARGame
{
    public class WorkStationScript : MonoBehaviourPun
    {
        // Start is called before the first frame update
        
        enum Ingredient
        {
            Rice,
            Seaweed,
            Fish
        }

        List<int> fish;
        List<int> rice;
        List<int> seaweed;
        List<int> allIngredients;

        GameObject sushi;
        GameObject g;

        GameObject order;
        GameObject continuePanel;
        bool orderPlaced;

        void Awake()
        {
            fish = new List<int>();
            rice = new List<int>();
            seaweed = new List<int>();
            allIngredients = new List<int>();
        }

        // Update is called once per frame
        void Update()
        {
            if (orderPlaced)
            {
                if (order.GetComponent<OrderListScript>().order.Sum() == 0)
                {
                    g = GameObject.FindGameObjectWithTag("GameManager");
                    g.GetPhotonView().RPC("Win", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
                    orderPlaced = false;
                }
            }
        }

        [PunRPC] 
        public void Cook(int playerID)
        {
            if (playerID != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log("Expecting player ID:" + PhotonNetwork.LocalPlayer.ActorNumber + ", is actually: " + playerID);
                return;
            }
            if (fish.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    useFish();
                }

                Debug.Log("$$$ made sashimi");
                rearrange();
                sushi = PhotonNetwork.Instantiate("ssashimi", new Vector3(0f, 0.19f, gameObject.transform.GetChild(0).transform.position.z), Quaternion.identity);
            }
            else if (fish.Count >= 1)
            {
                if (rice.Count >= 1)
                {
                    if (seaweed.Count >= 1)
                    {
                        useFish();
                        useRice();
                        useSeaweed();
                        Debug.Log("$$$ made maki");
                        rearrange();
                        sushi = PhotonNetwork.Instantiate("smaki", new Vector3(0f, 0.19f, gameObject.transform.GetChild(0).transform.position.z), Quaternion.identity);
                    }
                    else
                    {
                        useFish();
                        useRice();
                        Debug.Log("$$$ made nigiri");
                        rearrange();
                        sushi = PhotonNetwork.Instantiate("snigiri", new Vector3(0f, 0.19f, gameObject.transform.GetChild(0).transform.position.z), Quaternion.identity);
                    }
                }
            }
            else
            {
                if (rice.Count >= 2 && seaweed.Count >= 1)
                {
                    useSeaweed();
                    useRice();
                    useRice();
                    Debug.Log("$$$ made onigiri");
                    rearrange();
                    sushi = PhotonNetwork.Instantiate("sonigiri", new Vector3(0f, 0.19f, gameObject.transform.GetChild(0).transform.position.z), Quaternion.identity);
                }
            }
        }

        [PunRPC]
        public void PlaceOrder(int i)
        {
            order = GameObject.FindGameObjectWithTag("OrderUI");
            order.GetComponent<OrderListScript>().ReceiveOrder(15 + 5 * i);
            orderPlaced = true;
        }

        [PunRPC]
        public void TrashIngredientOmg(int id, int playerID)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != playerID) return;
            Debug.Log("$$$ trashing ingredient");
            RemoveIngredient(id);
            GameObject toTrash = PhotonView.Find(id).gameObject;
            PhotonNetwork.Destroy(toTrash);
            rearrange();
        }


        [PunRPC]
        public void MadeSushi(int id, int playerID)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != playerID) return;
            string name = sushi.name;
            if (name.Contains("onigiri"))
            {
                order.GetComponent<OrderListScript>().Create(1);
            } else if (name.Contains("nigiri"))
            {
                order.GetComponent<OrderListScript>().Create(0);
            } else if (name.Contains("sashimi"))
            {
                order.GetComponent<OrderListScript>().Create(3);
            } else if (name.Contains("maki"))
            {
                order.GetComponent<OrderListScript>().Create(2);
            }
            PhotonNetwork.Destroy(sushi);
        }

        [PunRPC]
        public void RemoveIngredient(int ID)
        {
            foreach (int i in allIngredients)
            {
                if (i == ID)
                {
                    allIngredients.Remove(i);
                    if (fish.Contains(i))
                    {
                        fish.Remove(i);
                    } else if (seaweed.Contains(i))
                    {
                        seaweed.Remove(i);
                    } else if (rice.Contains(i)){
                        rice.Remove(i);
                    }
                    Debug.Log("$$$ ingredient removed from workstation");
                }
            }
            rearrange();
        }


        [PunRPC]
        public void AddIngredient(int ID, int playerID)
        {
            // presumably called when ingredient object collides with workstation area
            // presumably PhotonView ID is also sent 

            if (PhotonNetwork.LocalPlayer.ActorNumber != playerID) return;
            Debug.Log("$$$ add ingredient called");
            GameObject i = PhotonView.Find(ID).gameObject;

            if (allIngredients.Count == 3)
            {
                Debug.Log("$$$ Reached Limit");
            } else
            {
                // switch case for names
                allIngredients.Add(ID);
                Vector3 pos = i.transform.position;
                pos = new Vector3(0.1f * allIngredients.IndexOf(ID) + -0.1f, 0.19f, gameObject.transform.position.z);
                i.transform.position = pos;

                if (i.name.Contains("fish"))
                {
                    fish.Add(ID);
                } else if (i.name.Contains("weed"))
                {
                    seaweed.Add(ID);
                } else if (i.name.Contains("rice"))
                {
                    rice.Add(ID);
                }
            }
            Debug.Log("$$$ Current Ingredients: " + string.Join(", ", allIngredients));
        }

        private void useFish()
        {
            int toRemove = fish[0];
            fish.RemoveAt(0);
            allIngredients.Remove(toRemove);
            GameObject toTrash = PhotonView.Find(toRemove).gameObject;
            PhotonNetwork.Destroy(toTrash);
        }

        private void useSeaweed()
        {
            int toRemove = seaweed[0];
            seaweed.RemoveAt(0);
            allIngredients.Remove(toRemove);
            GameObject toTrash = PhotonView.Find(toRemove).gameObject;
            PhotonNetwork.Destroy(toTrash);
        }

        private void useRice()
        {
            int toRemove = rice[0];
            rice.RemoveAt(0);
            allIngredients.Remove(toRemove);
            GameObject toTrash = PhotonView.Find(toRemove).gameObject;
            PhotonNetwork.Destroy(toTrash);
        }

        private void rearrange()
        {
            for (int i = 0; i < allIngredients.Count; i++)
            {
                GameObject obj = PhotonView.Find(allIngredients[i]).gameObject;
                Vector3 pos = obj.transform.position;
                pos = new Vector3(0.1f * i + -0.1f, 0.19f, gameObject.transform.position.z);
                obj.transform.position = pos;

            }
        }

        [PunRPC]
        public void WaitForRestart()
        {
            orderPlaced = false;
            allIngredients.Clear();
            fish.Clear();
            seaweed.Clear();
            rice.Clear();
            continuePanel = GameObject.FindGameObjectWithTag("ContinueUI");
            continuePanel.SetActive(true);
        }
    }
}
