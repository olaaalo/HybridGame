using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoSingleton<SoundManager>
{
    public static List<FMOD.Studio.EventInstance> FmodEvent_empties;
    public static List<FMOD.Studio.EventInstance> FmodEvent_traps;
    public static List<FMOD.Studio.EventInstance> FmodEvent_monies;

    override protected void Awake()
    {
        base.Awake();

        FmodEvent_empties = new List<FMOD.Studio.EventInstance>();
        FmodEvent_traps = new List<FMOD.Studio.EventInstance>();
        FmodEvent_monies = new List<FMOD.Studio.EventInstance>();
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

    static public void PlayMoneySound()
    {
        FmodEvent_monies.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Money"));
        FmodEvent_monies[FmodEvent_monies.Count - 1].start();
    }

    static public void StopEventSound()
    {
        foreach (var fEvent in FmodEvent_empties)
            fEvent.stop(0);
        foreach (var fEvent in FmodEvent_traps)
            fEvent.stop(0);
        foreach (var fEvent in FmodEvent_monies)
            fEvent.stop(0);

        DOVirtual.DelayedCall(0.5f, ClearEventSound);
    }

    static public void ClearEventSound()
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

    private void OnDisable()
    {
        ClearEventSound();
    }
}
