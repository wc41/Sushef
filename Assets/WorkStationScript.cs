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

        void Start()
        {
            fish = new List<int>();
            rice = new List<int>();
            seaweed = new List<int>();
            allIngredients = new List<int>();
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
                    int toRemove = fish[0];
                    fish.RemoveAt(0);
                    allIngredients.Remove(toRemove);
                }
            } else if (fish.Count >= 1)
            {
                if (rice.Count >= 1)
                {
                    if (seaweed.Count >= 1)
                    {
                        int toRemove = fish[0];
                        fish.RemoveAt(0);
                        allIngredients.Remove(toRemove);
                        toRemove = rice[0];
                        rice.RemoveAt(0);
                        allIngredients.Remove(toRemove);
                        toRemove = seaweed[0];
                        seaweed.RemoveAt(0);
                        allIngredients.Remove(toRemove);
                    } else
                    {
                        int toRemove = fish[0];
                        fish.RemoveAt(0);
                        allIngredients.Remove(toRemove);
                        toRemove = rice[0];
                        rice.RemoveAt(0);
                        allIngredients.Remove(toRemove);
                    }
                }
            } else
            {
                if (rice.Count >= 2 && seaweed.Count >= 1)
                {
                    int toRemove = seaweed[0];
                    fish.RemoveAt(0);
                    allIngredients.Remove(toRemove);
                    toRemove = rice[0];
                    rice.RemoveAt(0);
                    allIngredients.Remove(toRemove);
                    toRemove = rice[0];
                    rice.RemoveAt(0);
                    allIngredients.Remove(toRemove);
                }
            }

            // creating a sushi piece:
            // Remove Objects from List
            // Remove Objects from Array (Displayed on board)
            // Create sushi object

        }

        public void AddIngredient(string name, int ID)
        {
            // presumably called when ingredient object collides with workstation area
            // presumably PhotonView ID is also sent 

            if (allIngredients.Count == 6)
            {
                Debug.Log("Reached Limit");
            } else
            {
                // switch case for names
                allIngredients.Add(ID);
            }
        }

    }
}
