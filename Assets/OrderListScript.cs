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

        int order1, order2, order3, order4;
        bool ready;
        // Start is called before the first frame update
        void Start()
        {
            orderText = gameObject.GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (ready)
            {
                orderText.text = "Nigiri: " + order1 + " Onigiri: " + order2 + " Maki: " + order3 + " Sashimi: " + order4;
            }
            else
            {
                orderText.text = "";
            }
        }

        [PunRPC]
        public void ReceiveOrder(int num)
        {
            rnd = new System.Random();
            order1 = (int)(num * 1f * rnd.Next(15, 35) / 100f);
            num -= order1;
            order2 = (int)(num * 1f * rnd.Next(25, 40) / 100f);
            num -= order2;
            order3 = (int)(num * 1f * rnd.Next(35, 65) / 100f);
            order4 = num - order3;
            ready = true;
        }
    }
}
