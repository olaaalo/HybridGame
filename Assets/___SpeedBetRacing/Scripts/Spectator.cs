using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.SpeedBetRacing
{
    public class Spectator : MonoBehaviour
    {
        public Animator animator;
        public Renderer surfaceRenderer;

        public int vehiculeID;

        private void Start()
        {
            animator.speed = Random.Range(0.7f, 1.3f);

            vehiculeID = Random.Range(0, GameManager.instance.gameValue.vehiclesInfos.Count);

            surfaceRenderer.material.color = GameManager.instance.gameValue.vehiclesInfos[vehiculeID].color + Color.Lerp(Color.white * 0.8f, Color.black * 0.8f, Random.value) / Random.Range(3f, 5f);
        }
    }
}