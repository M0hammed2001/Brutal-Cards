using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ScoreBoardItem
{
    [SerializeField] public string playerName = string.Empty;
    [SerializeField] public int score = 0;

    public void SetData(MatchInfo info)
    {
        playerName = info.PlayerName;
        score = info.Score;

    }
}
