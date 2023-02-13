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
            g.GetPhotonView().RPC("UpdateOrder", RpcTarget.Others, 30);
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

                Debug.Log("$$$ made sashimi");
                g.GetPhotonView().RPC("Create", RpcTarget.Others, 3);
                rearrange();
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
                        Debug.Log("$$$ made maki");
                        g.GetPhotonView().RPC("Create", RpcTarget.Others, 2);
                        
                        rearrange();
                        // sushi = PhotonNetwork.Instantiate("maki", gameObject.translation, Quaternion.identity);

                    }
                    else
                    {
                        useFish();
                        useRice();
                        Debug.Log("$$$ made nigiri");
                        g.GetPhotonView().RPC("Create", RpcTarget.Others, 0);
                        rearrange();
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
                    Debug.Log("$$$ made onigiri");
                    g.GetPhotonView().RPC("Create", RpcTarget.Others, 1);
                    rearrange();

                }
                // sushi = PhotonNetwork.Instantiate("onigiri", gameObject.translation, Quaternion.identity);

            }
        }

        [PunRPC]
        public void TrashIngredientOmg(int id)
        {
            Debug.Log("$$$ trashing ingredient");
            GameObject toTrash = PhotonView.Find(id).gameObject;
            PhotonNetwork.Destroy(toTrash);
        }


        [PunRPC]
        public void RemoveIngredient(int ID)
        {
            foreach (int i in allIngredients)
            {
                if (i == ID)
                {
                    allIngredients.Remove(i);
                    Debug.Log("$$$ ingredient removed from workstation");

                }
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

    }
}
