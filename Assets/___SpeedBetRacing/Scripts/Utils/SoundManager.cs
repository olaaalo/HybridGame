using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using LibLabSystem;

namespace LibLabGames.SpeedBetRacing
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        public List<FMOD.Studio.EventInstance> FmodEvent_empties;
        public List<FMOD.Studio.EventInstance> FmodEvent_traps;
        public List<FMOD.Studio.EventInstance> FmodEvent_monies;

        void Awake()
        {
            instance = this;
			DontDestroyOnLoad(gameObject);

            FmodEvent_empties = new List<FMOD.Studio.EventInstance>();
            FmodEvent_traps = new List<FMOD.Studio.EventInstance>();
            FmodEvent_monies = new List<FMOD.Studio.EventInstance>();
        }

        public void PlayEmptySound()
        {
            FmodEvent_empties.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Empty"));
            FmodEvent_empties[FmodEvent_empties.Count - 1].start();
        }

        public void PlayTrapSound()
        {
            FmodEvent_traps.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Trap"));
            FmodEvent_traps[FmodEvent_traps.Count - 1].start();
        }

        public void PlayMoneySound()
        {
            FmodEvent_monies.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Money"));
            FmodEvent_monies[FmodEvent_monies.Count - 1].start();
        }

        public void StopEventSound()
        {
            foreach (var fEvent in FmodEvent_empties)
                fEvent.stop(0);
            foreach (var fEvent in FmodEvent_traps)
                fEvent.stop(0);
            foreach (var fEvent in FmodEvent_monies)
                fEvent.stop(0);

            DOVirtual.DelayedCall(0.5f, ClearEventSound);
        }

        public void ClearEventSound()
        {
            foreach (var fEvent in FmodEvent_empties)
                fEvent.clearHandle();
            foreach (var fEvent in FmodEvent_traps)
                fEvent.clearHandle();
            foreach (var fEvent in FmodEvent_monies)
                fEvent.clearHandle();

            FmodEvent_empties = new List<FMOD.Studio.EventInstance>();
            FmodEvent_traps = new List<FMOD.Studio.EventInstance>();
            FmodEvent_monies = new List<FMOD.Studio.EventInstance>();
        }

        void OnDisable()
        {
            ClearEventSound();
        }
    }
}