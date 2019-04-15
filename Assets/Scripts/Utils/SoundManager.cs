using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoSingleton<SoundManager>
{
    public static List<FMOD.Studio.EventInstance> FmodEvent_empties;
    public static List<FMOD.Studio.EventInstance> FmodEvent_traps;

    override protected void Awake()
    {
        base.Awake();

        FmodEvent_empties = new List<FMOD.Studio.EventInstance>();
        FmodEvent_traps = new List<FMOD.Studio.EventInstance>();
    }


    static public void PlayEmptySound()
    {
        FmodEvent_empties.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Empty"));
        FmodEvent_empties[FmodEvent_empties.Count - 1].start();
    }

    static public void PlayTrapSound()
    {
        FmodEvent_traps.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Trap"));
        FmodEvent_traps[FmodEvent_traps.Count - 1].start();
    }

    static public void ClearEventSound()
    {
        foreach (var fEvent in FmodEvent_empties)
            fEvent.stop(0);
        foreach (var fEvent in FmodEvent_traps)
            fEvent.stop(0);

        DOVirtual.DelayedCall(0.5f, () => {
            foreach (var fEvent in FmodEvent_empties)
                fEvent.clearHandle();
            foreach (var fEvent in FmodEvent_traps)
                fEvent.clearHandle();

            FmodEvent_empties = new List<FMOD.Studio.EventInstance>();
            FmodEvent_traps = new List<FMOD.Studio.EventInstance>();
        });

    }

    private void OnDisable()
    {
        ClearEventSound();
    }
}
