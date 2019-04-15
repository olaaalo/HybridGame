using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public static FMOD.Studio.EventInstance FmodEvent_musicMainMenu;

    public static FMOD.Studio.EventInstance FmodEvent_buttonValidation;
    public static FMOD.Studio.EventInstance FmodEvent_buttonNavigation;
    public static FMOD.Studio.EventInstance FmodEvent_buttonSelection;
    public static FMOD.Studio.EventInstance FmodEvent_buttonReturn;

    public static FMOD.Studio.EventInstance FmodEvent_deckBox;
    public static FMOD.Studio.EventInstance FmodEvent_potOfGreed;
    public static FMOD.Studio.EventInstance FmodEvent_milleniumPuzzle;

    override protected void Awake()
    {
        base.Awake();

        FmodEvent_musicMainMenu = FMODUnity.RuntimeManager.CreateInstance("event:/Musics/Music_1");

        FmodEvent_buttonValidation = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Button_validation");
        FmodEvent_buttonNavigation = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Button_navigation");
        FmodEvent_buttonSelection = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Button_selection");
        FmodEvent_buttonReturn = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Button_return");

        FmodEvent_deckBox = FMODUnity.RuntimeManager.CreateInstance("event:/Others/DeckBox_onClick");
        FmodEvent_potOfGreed = FMODUnity.RuntimeManager.CreateInstance("event:/Others/PotOfGreed_onClick");
        FmodEvent_milleniumPuzzle = FMODUnity.RuntimeManager.CreateInstance("event:/Others/MilleniumPuzzle_onClick");
    }
}
