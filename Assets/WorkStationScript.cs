using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

        void Start()
        {
            fish = new List<int>();
            rice = new List<int>();
            seaweed = new List<int>();
            allIngredients = new List<int>();
            g = GameObject.FindGameObjectWithTag("GameManager");
        }

        // Update is called once per frame
        void Update()
        {
            // Recipes:
            // Nigiri: F + R
            // Onigiri: S + R + R
            // Maki: S + R + F
            // Sashimi: F + F + F
            if (fish.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    useFish();
                }

                // sushi = PhotonNetwork.Instantiate("sashimi", gameObject.translation, Quaternion.identity);

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
                        // sushi = PhotonNetwork.Instantiate("maki", gameObject.translation, Quaternion.identity);

                    }
                    else
                    {
                        useFish();
                        useRice();
                        // sushi = PhotonNetwork.Instantiate("nigiri", gameObject.translation, Quaternion.identity);

                    }
                }
            } else
            {
                if (rice.Count >= 2 && seaweed.Count >= 1)
                {
                    useSeaweed();
                    useRice();
                    useRice();
                }

                // sushi = PhotonNetwork.Instantiate("onigiri", gameObject.translation, Quaternion.identity);

            }
        }

        [PunRPC]
        public void AddIngredient(int ID)
        {
            // presumably called when ingredient object collides with workstation area
            // presumably PhotonView ID is also sent 

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
                pos = new Vector3(0.1f * allIngredients.IndexOf(ID) + -0.1f, 0.15f, gameObject.transform.position.z);
                i.transform.position = pos;

                if (i.name.Contains("fish"))
                {
                    fish.Add(ID);
                } else if (i.name.Contains("seaweed"))
                {
                    seaweed.Add(ID);
                } else if (i.name.Contains("rice"))
                {
                    rice.Add(ID);
                }
            }
        }

        private void useFish()
        {
            int toRemove = fish[0];
            fish.RemoveAt(0);
            allIngredients.Remove(toRemove);
            g.GetPhotonView().RPC("Trash", RpcTarget.Others, toRemove);
        }

        private void useSeaweed()
        {
            int toRemove = seaweed[0];
            fish.RemoveAt(0);
            allIngredients.Remove(toRemove);
            g.GetPhotonView().RPC("Trash", RpcTarget.Others, toRemove);
        }

        private void useRice()
        {
            int toRemove = rice[0];
            rice.RemoveAt(0);
            allIngredients.Remove(toRemove);
            g.GetPhotonView().RPC("Trash", RpcTarget.Others, toRemove);
        }

    }
}
