using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commentator : MonoBehaviour
{
    public CommentatorObject commentatorObject;
    public AudioSource audioSource;

    public float timeToRandomQuote;
    private WaitForSeconds waitRandomQuote;

    private void Start()
    {
        waitRandomQuote = new WaitForSeconds(timeToRandomQuote);
    }

    public void PlayQuote(List<AudioClip> clips)
    {
        if (audioSource.isPlaying || clips.Count == 0) return;

        audioSource.clip = clips[Random.Range(0, clips.Count)];
        audioSource.Play();

        if (countdown != null)
            StopCoroutine(countdown);

        countdown = cocoCountdown();

        StartCoroutine(countdown);
    }

    public void ExplosionVehicle(string machineName)
    {
        if (!GameManager.instance.gameHasStarted) return;

        audioSource.Stop();

        switch (machineName)
        {
        case "Dream Shark":
            PlayQuote(commentatorObject.commentatorQuotes[0].explosionDreamShark);
            break;
        case "Burning Line":
            PlayQuote(commentatorObject.commentatorQuotes[0].explosionBurningLine);
            break;
        case "Serenity":
            PlayQuote(commentatorObject.commentatorQuotes[0].explosionSerenity);
            break;
        case "Orange Earth":
            PlayQuote(commentatorObject.commentatorQuotes[0].explosionOrangeEarth);
            break;
        case "Tropical Bear":
            PlayQuote(commentatorObject.commentatorQuotes[0].explosionTropicalBear);
            break;
        }
    }

    public void FirstPlaceVehicle(string machineName)
    {
        switch (machineName)
        {
        case "Dream Shark":
            PlayQuote(commentatorObject.commentatorQuotes[0].firstPlaceDreamShark);
            break;
        case "Burning Line":
            PlayQuote(commentatorObject.commentatorQuotes[0].firstPlaceBurningLine);
            break;
        case "Serenity":
            PlayQuote(commentatorObject.commentatorQuotes[0].firstPlaceSerenity);
            break;
        case "Orange Earth":
            PlayQuote(commentatorObject.commentatorQuotes[0].firstPlaceOrangeEarth);
            break;
        case "Tropical Bear":
            PlayQuote(commentatorObject.commentatorQuotes[0].firstPlaceTropicalBear);
            break;
        }
    }

    public void FirstRankVehicle(int[] ranks)
    {
        audioSource.Stop();

        for (int i = 0; i < ranks.Length; ++i)
        {
            if (ranks[i] == 1)
            {
                switch (i)
                {
                case 0:
                    PlayQuote(commentatorObject.commentatorQuotes[0].firstRankDreamShark);
                    break;
                case 1:
                    PlayQuote(commentatorObject.commentatorQuotes[0].firstRankBurningLine);
                    break;
                case 2:
                    PlayQuote(commentatorObject.commentatorQuotes[0].firstRankSerenity);
                    break;
                case 3:
                    PlayQuote(commentatorObject.commentatorQuotes[0].firstRankTropicalBear);
                    break;
                case 4:
                    PlayQuote(commentatorObject.commentatorQuotes[0].firstRankOrangeEarth);
                    break;
                }
            }
        }
    }

    public void RanksVehicle(int[] ranks)
    {
        audioSource.Stop();

        StartCoroutine(QuoteRank(ranks));
    }

    private IEnumerator QuoteRank(int[] ranks)
    {
        for (int countRank = 1; countRank < 6; ++countRank)
        {
            for (int i = 0; i < ranks.Length; ++i)
            {
                if (ranks[i] == countRank)
                {
                    switch (countRank)
                    {
                        // 1er position
                    case 1:
                        switch (i)
                        {
                        case 0:
                            PlayQuote(commentatorObject.commentatorQuotes[0].firstRankDreamShark);
                            break;
                        case 1:
                            PlayQuote(commentatorObject.commentatorQuotes[0].firstRankBurningLine);
                            break;
                        case 2:
                            PlayQuote(commentatorObject.commentatorQuotes[0].firstRankSerenity);
                            break;
                        case 3:
                            PlayQuote(commentatorObject.commentatorQuotes[0].firstRankTropicalBear);
                            break;
                        case 4:
                            PlayQuote(commentatorObject.commentatorQuotes[0].firstRankOrangeEarth);
                            break;
                        }
                        break;
                        // 2eme position
                    case 2:
                        switch (i)
                        {
                        case 0:
                            PlayQuote(commentatorObject.commentatorQuotes[0].secondRankDreamShark);
                            break;
                        case 1:
                            PlayQuote(commentatorObject.commentatorQuotes[0].secondRankBurningLine);
                            break;
                        case 2:
                            PlayQuote(commentatorObject.commentatorQuotes[0].secondRankSerenity);
                            break;
                        case 3:
                            PlayQuote(commentatorObject.commentatorQuotes[0].secondRankTropicalBear);
                            break;
                        case 4:
                            PlayQuote(commentatorObject.commentatorQuotes[0].secondRankOrangeEarth);
                            break;
                        }
                        break;
                        // 3eme position
                    case 3:
                        switch (i)
                        {
                        case 0:
                            PlayQuote(commentatorObject.commentatorQuotes[0].thirdRankDreamShark);
                            break;
                        case 1:
                            PlayQuote(commentatorObject.commentatorQuotes[0].thirdRankBurningLine);
                            break;
                        case 2:
                            PlayQuote(commentatorObject.commentatorQuotes[0].thirdRankSerenity);
                            break;
                        case 3:
                            PlayQuote(commentatorObject.commentatorQuotes[0].thirdRankTropicalBear);
                            break;
                        case 4:
                            PlayQuote(commentatorObject.commentatorQuotes[0].thirdRankOrangeEarth);
                            break;
                        }
                        break;
                        // 4eme position
                    case 4:
                        switch (i)
                        {
                        case 0:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fourthRankDreamShark);
                            break;
                        case 1:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fourthRankBurningLine);
                            break;
                        case 2:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fourthRankSerenity);
                            break;
                        case 3:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fourthRankTropicalBear);
                            break;
                        case 4:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fourthRankOrangeEarth);
                            break;
                        }
                        break;
                        // 5eme position
                    case 5:
                        switch (i)
                        {
                        case 0:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fifthRankDreamShark);
                            break;
                        case 1:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fifthRankBurningLine);
                            break;
                        case 2:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fifthRankSerenity);
                            break;
                        case 3:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fifthRankTropicalBear);
                            break;
                        case 4:
                            PlayQuote(commentatorObject.commentatorQuotes[0].fifthRankOrangeEarth);
                            break;
                        }
                        break;
                    }
                }
            }

            while (audioSource.isPlaying)
                yield return null;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator countdown;
    private IEnumerator cocoCountdown()
    {
        while (audioSource.isPlaying)
            yield return null;

        yield return waitRandomQuote;

        PlayQuote(commentatorObject.commentatorQuotes[0].randomQuote);
    }
}