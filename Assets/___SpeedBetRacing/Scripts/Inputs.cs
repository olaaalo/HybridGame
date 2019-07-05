using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using LibLabSystem;
using UnityEngine;
using UnityEngine.Events;

namespace LibLabGames.SpeedBetRacing
{
    public class Inputs : MonoBehaviour
    {
        public static Inputs instance;

        public static string port = "COM6";
        private static SerialPort sp;

        UnityEvent eventInput0 = new UnityEvent();
        UnityEvent eventInput1 = new UnityEvent();
        UnityEvent eventInput2 = new UnityEvent();
        UnityEvent eventInput3 = new UnityEvent();
        UnityEvent eventInput4 = new UnityEvent();

        void Start()
        {
            if (LLSystem.isInit == false)
                return;

            instance = this;
        }

        string[] portNames;
        bool find;
        public void ActiveSerialPort()
        {
            portNames = SerialPort.GetPortNames();

            foreach(var p in portNames)
            {
                if (p == port)
                    find = true;
            }

            if (!find)
                return;

            sp = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            sp.DtrEnable = false;
            sp.ReadTimeout = 10;
            sp.Open();

            eventInput0.AddListener(Input0);
            eventInput1.AddListener(Input1);
            eventInput2.AddListener(Input2);
            eventInput3.AddListener(Input3);
            eventInput4.AddListener(Input4);
        }

        void Update()
        {
            if (sp == null) return;

            if (sp.IsOpen)
            {
                string cmd = CheckForRecievedData();

                if (cmd != string.Empty)
                {
                    if (cmd.Contains("0"))
                        eventInput0.Invoke();
                    if (cmd.Contains("1"))
                        eventInput1.Invoke();
                    if (cmd.Contains("2"))
                        eventInput2.Invoke();
                    if (cmd.Contains("3"))
                        eventInput3.Invoke();
                    if (cmd.Contains("4"))
                        eventInput4.Invoke();
                }
            }
        }

        public string CheckForRecievedData()
        {
            try
            {
                string inData = sp.ReadLine();
                sp.BaseStream.Flush();
                sp.DiscardInBuffer();
                return inData;
            }
            catch { return string.Empty; }
        }

        private void Input0()
        {
            LLLog.Log("ARDUINO", "Input 0");
            GameManager.instance.BetOnVehicle(0, 1);
        }

        private void Input1()
        {
            LLLog.Log("ARDUINO", "Input 1");
            GameManager.instance.BetOnVehicle(1, 1);
        }

        private void Input2()
        {
            LLLog.Log("ARDUINO", "Input 2");
            GameManager.instance.BetOnVehicle(2, 1);
        }

        private void Input3()
        {
            LLLog.Log("ARDUINO", "Input 3");
            GameManager.instance.BetOnVehicle(3, 1);
        }

        private void Input4()
        {
            LLLog.Log("ARDUINO", "Input 4");
            GameManager.instance.BetOnVehicle(4, 1);
        }

        private void OnApplicationQuit()
        {
            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }
        }

        private void OnDisable()
        {
            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }
        }

        private void OnDestroy()
        {
            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }
        }
    }
}