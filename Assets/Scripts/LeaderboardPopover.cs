using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPopover : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<LeaderboardTableEntry> leaderboardTableEntryList;
    private List<Transform> leaderboardTableEntryTransformList;



    private void Awake()
    {
        entryContainer = transform.Find("leaderboardpopoverEntryContainer");
        entryTemplate = entryContainer.Find("leaderboardpopoverEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

       leaderboardTableEntryList = new List<LeaderboardTableEntry>();
        new LeaderboardTableEntry { brutalWins = 30, nickname = "AA" };
        new LeaderboardTableEntry { brutalWins = 40, nickname = "BB" };
        new LeaderboardTableEntry { brutalWins = 60, nickname = "CC" };
        new LeaderboardTableEntry { brutalWins = 15, nickname = "DD" };
        new LeaderboardTableEntry { brutalWins = 72, nickname = "ee" };
        new LeaderboardTableEntry { brutalWins = 82, nickname = "ff" };
        new LeaderboardTableEntry { brutalWins = 11, nickname = "gg" };
        new LeaderboardTableEntry { brutalWins = 29, nickname = "hh" };
        new LeaderboardTableEntry { brutalWins = 118, nickname = "ii"};
        
        foreach (LeaderboardTableEntry leaderboardTableEntry in leaderboardTableEntryList)
        {
            CreateLeaderboardTableEntryTransform(leaderboardtableEntry, entryContainer, leaderboardTableEntryTransformList);
        }
    }
    
    void CreateLeaderboardTableEntryTransform(LeaderboardTableEntry leaderboardtableEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 20f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
            {
                rankString = rank + "th"; 
                case 1: rankString = "1st";
                case 2: rankString = "2nd";
                case 3: rankString = "3rd"; 
                break;
            }
        }

        entryTransform.Find("rankingText").GetComponent<Text>().text = rankString;

        int brutalWins = leaderboardtableEntry.brutalWins;

        entryTransform.Find("nicknameText").GetComponent<Text>().text = brutalWins.ToString();

        string nickname = leaderboardtableEntry.nickname;

        entryTransform.Find("brutalWinsText").GetComponent<Text>().text = nickname;

        transformList.Add(entryTransform);
    }

    /*
     * Represents a single leaderboard entry
     * */
    private class LeaderboardTableEntry 
    {
        public int brutalWins;
        public string nickname;
    }




}
