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
        // Start is called before the first frame update
        void Start()
        {
            orderText = gameObject.GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (ready && order[0] + order[1] + order[2] + order[3] == 0)
            {
                orderText.text = "You win!!!";
            } else if (lost)
            {
                orderText.text = "You lost!!!";
            }
            else if (ready)
            {
                orderText.text = "Nigiri: " + order[0] + " Onigiri: " + order[1] + " Maki: " + order[2] + " Sashimi: " + order[3];
            }
            else
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
            rnd = new System.Random();
            order[0] = (int)(num * 1f * rnd.Next(15, 35) / 100f);
            num -= order[0];
            order[1] = (int)(num * 1f * rnd.Next(25, 40) / 100f);
            num -= order[1];
            order[2] = (int)(num * 1f * rnd.Next(35, 65) / 100f);
            order[3] = num - order[2];
            ready = true;

            Debug.Log("$$$order received, making...");
        }

        public void Create(int type)
        {
            if (order[type] > 0)
            {
                order[type] -= 1;
            }
        }
    }
}
