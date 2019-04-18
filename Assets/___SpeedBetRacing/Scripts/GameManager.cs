﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public Text[] playerBetTexts;
    public Text[] playerOneBetTexts;
    public Text[] playerTwoBetTexts;
    public Text[] playerThreeBetTexts;
    public Text[] allBetTexts;
    public Text betText;
    public int currentVehiculeBet;

    public Vehicule[] vehicules;
    public Color[] colors;

    public int[] bet_playerOne = new int[5];
    public int[] bet_playerTwo = new int[5];
    public int[] bet_playerThree = new int[5];

    public KeyCode[] input_playerOne;
    public KeyCode[] input_playerTwo;
    public KeyCode[] input_playerThree;

    public bool gameIsEnded;

    public void Start()
    {
        for (int i = 0; i < 5; ++i)
        {
            vehicules[i].ApplyColor(colors[i]);
            playerOneBetTexts[i].color = colors[i];
            playerTwoBetTexts[i].color = colors[i];
            playerThreeBetTexts[i].color = colors[i];
            allBetTexts[i].color = colors[i];
        }

        playerBetTexts[0].text = "Player 1 : " + betLimitOne;
        playerBetTexts[1].text = "Player 2 : " + betLimitTwo;
        playerBetTexts[2].text = "Player 3 : " + betLimitThree;
        
        UpdateVehiculesSpeed();
        StartCoroutine(coco());
    }

    IEnumerator coco()
    {
        while (gameIsEnded)
            yield return null;

        while (!gameIsEnded)
        {
            betText.transform.DOScale(1.5f, 0.5f).From();
            currentVehiculeBet = Random.Range(0,5);

            yield return new WaitForSeconds(4);

            UpdateVehiculesSpeed();

            currentVehiculeBet = -1;
            
            yield return new WaitForSeconds(2);
        }
    }

    private int betLimitOne = 15;
    private int betLimitTwo = 15;
    private int betLimitThree = 15;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            gameIsEnded = false;

        if (gameIsEnded || currentVehiculeBet == -1)
        {
            betText.color = Color.black;
            return;
        }

        // Check Input Player 1
        for (int i = 0; i < 5; ++i)
        {
            if (Input.GetKeyDown(input_playerOne[i]) && betLimitOne > 0)
            {
                // vehicules[i].speed -= 10;
                // vehicules[i].speed = Mathf.Clamp(vehicules[i].speed, 1, Mathf.Infinity);
                
                // vehicules[i].life--;
                // if (vehicules[i].life == 0){
                //     vehicules[i].life = 3;
                //     vehicules[i].speed = vehicules[i].baseSpeed;
                // }


                bet_playerOne[currentVehiculeBet]++;

                // vehicules[i].transform.DOKill();
                // vehicules[i].transform.localScale = Vector3.one;
                // vehicules[i].transform.DOScale(1.3f, 0.5f).From();

                playerOneBetTexts[i].text = bet_playerOne[i].ToString();
                playerOneBetTexts[i].transform.localScale -= Vector3.one * 0.07f;
                allBetTexts[currentVehiculeBet].text = (bet_playerOne[i] + bet_playerTwo[i] + bet_playerThree[i]).ToString();
                allBetTexts[currentVehiculeBet].transform.DOKill();
                allBetTexts[currentVehiculeBet].transform.localScale = Vector3.one * 1.5f;
                allBetTexts[currentVehiculeBet].transform.DOScale(2, 0.5f).From();
                
                
                betLimitOne--;

                playerBetTexts[0].text = "Player 1 : " + betLimitOne;

                //currentVehiculeBet = (currentVehiculeBet + 1) % 5;
            }
        }

        // Check Input Player 2
        for (int i = 0; i < 5; ++i)
        {
            if (Input.GetKeyDown(input_playerTwo[i]) && betLimitTwo > 0)
            {
                // vehicules[i].speed -= 10;
                // vehicules[i].speed = Mathf.Clamp(vehicules[i].speed, 1, Mathf.Infinity);

                
                // vehicules[i].life--;
                // if (vehicules[i].life == 0){
                //     vehicules[i].life = 3;
                //     vehicules[i].speed = vehicules[i].baseSpeed;
                // }

                
                bet_playerTwo[currentVehiculeBet]++;

                // vehicules[i].transform.DOKill();
                // vehicules[i].transform.localScale = Vector3.one;
                // vehicules[i].transform.DOScale(1.3f, 0.5f).From();

                playerTwoBetTexts[i].text = bet_playerTwo[i].ToString();
                playerTwoBetTexts[i].transform.localScale -= Vector3.one * 0.07f;
                allBetTexts[currentVehiculeBet].text = (bet_playerOne[i] + bet_playerTwo[i] + bet_playerThree[i]).ToString();
                allBetTexts[currentVehiculeBet].transform.DOKill();
                allBetTexts[currentVehiculeBet].transform.localScale = Vector3.one * 1.5f;
                allBetTexts[currentVehiculeBet].transform.DOScale(2, 0.5f).From();

                betLimitTwo--;

                playerBetTexts[1].text = "Player 2 : " + betLimitTwo;
                
                //currentVehiculeBet = (currentVehiculeBet + 1) % 5;
            }
        }

        // Check Input Player 3
        for (int i = 0; i < 5; ++i)
        {
            if (Input.GetKeyDown(input_playerThree[i]) && betLimitThree > 0)
            {
                // vehicules[i].speed -= 10;
                // vehicules[i].speed = Mathf.Clamp(vehicules[i].speed, 1, Mathf.Infinity);
                
                
                // vehicules[i].life--;
                // if (vehicules[i].life == 0){
                //     vehicules[i].life = 3;
                //     vehicules[i].speed = vehicules[i].baseSpeed;
                // }

                bet_playerThree[currentVehiculeBet]++;
                
                // vehicules[i].transform.DOKill();
                // vehicules[i].transform.localScale = Vector3.one;
                // vehicules[i].transform.DOScale(1.3f, 0.5f).From();

                playerThreeBetTexts[i].text = bet_playerThree[i].ToString();
                playerThreeBetTexts[i].transform.localScale -= Vector3.one * 0.07f;
                allBetTexts[currentVehiculeBet].text = (bet_playerOne[i] + bet_playerTwo[i] + bet_playerThree[i]).ToString();
                allBetTexts[currentVehiculeBet].transform.DOKill();
                allBetTexts[currentVehiculeBet].transform.localScale = Vector3.one * 1.5f;
                allBetTexts[currentVehiculeBet].transform.DOScale(2, 0.5f).From();

                betLimitThree--;

                playerBetTexts[2].text = "Player 3 : " + betLimitThree;

                //currentVehiculeBet = (currentVehiculeBet + 1) % 5;
            }
        }

        betText.color = colors[currentVehiculeBet];
    }

    public void UpdateVehiculesSpeed()
    {
        for (int i = 0; i < 5; ++i)
        {
            switch((bet_playerOne[i] + bet_playerTwo[i] + bet_playerThree[i]) % 4)
            {
                case 0:
                vehicules[i].speed = 20; // base
                break;

                case 1:
                vehicules[i].speed = 30; // base + 10
                break;

                case 2:
                vehicules[i].speed = 40; // base + 20
                break;

                case 3:
                vehicules[i].speed = 20; // base
                break;

                case 4:
                vehicules[i].speed = 50; // base  + 30
                break;
            }
        }
    }
}