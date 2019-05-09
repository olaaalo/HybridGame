using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_CommentatorObject", menuName = "SpeedBetRacing/CommentatorObject")]
public class CommentatorObject : ScriptableObject
{
    [Serializable]
    public struct CommentatorQuotes
    {
        public List<AudioClip> startGame;
        public List<AudioClip> startRace;

        public List<AudioClip> firstPlaceDreamShark;
        public List<AudioClip> firstRankDreamShark;
        public List<AudioClip> secondRankDreamShark;
        public List<AudioClip> thirdRankDreamShark;
        public List<AudioClip> fourthRankDreamShark;
        public List<AudioClip> fifthRankDreamShark;
        public List<AudioClip> explosionDreamShark;
        
        public List<AudioClip> firstPlaceTropicalBear;
        public List<AudioClip> firstRankTropicalBear;
        public List<AudioClip> secondRankTropicalBear;
        public List<AudioClip> thirdRankTropicalBear;
        public List<AudioClip> fourthRankTropicalBear;
        public List<AudioClip> fifthRankTropicalBear;
        public List<AudioClip> explosionTropicalBear;
        
        public List<AudioClip> firstPlaceOrangeEarth;
        public List<AudioClip> firstRankOrangeEarth;
        public List<AudioClip> secondRankOrangeEarth;
        public List<AudioClip> thirdRankOrangeEarth;
        public List<AudioClip> fourthRankOrangeEarth;
        public List<AudioClip> fifthRankOrangeEarth;
        public List<AudioClip> explosionOrangeEarth;

        public List<AudioClip> firstPlaceSerenity;
        public List<AudioClip> firstRankSerenity;
        public List<AudioClip> secondRankSerenity;
        public List<AudioClip> thirdRankSerenity;
        public List<AudioClip> fourthRankSerenity;
        public List<AudioClip> fifthRankSerenity;
        public List<AudioClip> explosionSerenity;
        
        public List<AudioClip> firstPlaceBurningLine;
        public List<AudioClip> firstRankBurningLine;
        public List<AudioClip> secondRankBurningLine;
        public List<AudioClip> thirdRankBurningLine;
        public List<AudioClip> fourthRankBurningLine;
        public List<AudioClip> fifthRankBurningLine;
        public List<AudioClip> explosionBurningLine;
        
        public List<AudioClip> randomQuote;
    }

    public List<CommentatorQuotes> commentatorQuotes;
}
