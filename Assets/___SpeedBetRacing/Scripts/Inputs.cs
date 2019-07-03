using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Events;

namespace LibLabGames.SpeedBetRacing
{
    public class Inputs : MonoBehaviour
    {
        public static string port = "COM4";
        private static SerialPort sp;

        UnityEvent eventInput0 = new UnityEvent();
        UnityEvent eventInput1 = new UnityEvent();
        UnityEvent eventInput2 = new UnityEvent();
        UnityEvent eventInput3 = new UnityEvent();
        UnityEvent eventInput4 = new UnityEvent();

        void Start()
        {
            sp = new SerialPort(port, 9600);
            sp.Open();
            sp.ReadTimeout = 10;

            eventInput0.AddListener(Input0);
            eventInput1.AddListener(Input1);
            eventInput2.AddListener(Input2);
            eventInput3.AddListener(Input3);
            eventInput4.AddListener(Input4);
        }

        void Update()
        {
            if (sp.IsOpen)
            {
                string k = sp.ReadLine();
                if (k != null)
                {
                    switch (k)
                    {
                    case "passage0":
                        eventInput0.Invoke();
                        break;
                    case "passage1":
                        eventInput1.Invoke();
                        break;
                    case "passage2":
                        eventInput2.Invoke();
                        break;
                    case "passage3":
                        eventInput3.Invoke();
                        break;
                    case "passage4":
                        eventInput4.Invoke();
                        break;
                    }
                }
            }
        }

        private void Input0()
        {
            //ICI TU FAIS CE QUE TU VEUX MON CHER CLEMENT ! 
            //APPEL TES FONCTIONS, INCREMENTE TES VARIABLES, DECLANCHE TES EVENTS, DEMERDE TOI ....

            //MAIS MAINTENANT STOP AVEC CE PUTAIN DE ARDUINO DE MEEEEEEEEERDE !!!!!!!!!!!!!!!!!!

            Debug.Log("Input0");
        }

        private void Input1()
        {
            Debug.Log("Input1");
        }

        private void Input2()
        {
            Debug.Log("Input2");
        }

        private void Input3()
        {
            Debug.Log("Input3");
        }

        private void Input4()
        {
            Debug.Log("Input4");
        }
    }
}