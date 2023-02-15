using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace MyFirstARGame
{
    public class OrderListScript : MonoBehaviourPun
    {
        System.Random rnd;
        Text orderText;

        bool lost = false;

        public int[] order = { 0, 0, 0, 0 };
        bool ready;
        int score;
        int initOrder;
        // Start is called before the first frame update
        void Start()
        {
            orderText = gameObject.GetComponentInChildren<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (ready)
            {
                if (order[0] + order[1] + order[2] + order[3] == 0)
                {
                    orderText.text = "";
                    gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    score += initOrder * 5;
                    ready = false;
                } else if (lost)
                {
                    ready = false;
                }
                orderText.text = "Nigiri: " + order[0] + " Onigiri: " + order[1] + " Maki: " + order[2] + " Sashimi: " + order[3] + "\nScore: " + score;
            }
            else if (order[0] + order[1] + order[2] + order[3] == 0)
            {
                orderText.text = "You win!!!";
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
            } else if (lost)
            {
                orderText.text = "You lost!!!";
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
            } else 
            {
                orderText.text = "";
            }
        }

        public void Lost()
        {
            lost = true;
        }
        public void ReceiveOrder(int num)
        {
            initOrder = num;
            rnd = new System.Random();
            order[0] = (int)(num * 1f * rnd.Next(15, 35) / 100f);
            num -= order[0];
            order[1] = (int)(num * 1f * rnd.Next(25, 40) / 100f);
            num -= order[1];
            order[2] = (int)(num * 1f * rnd.Next(35, 65) / 100f);
            order[3] = num - order[2];
            lost = false;
            ready = true;

            Debug.Log("$$$order received, making...");
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        public void Create(int type)
        {
            if (order[type] > 0)
            {
                order[type] -= 1;
            }
            if (type == 0)
            {
                score += 15;
            } else if (type == 3)
            {
                score += 30;
            } else
            {
                score += 20;
            }
        }
    }
}
