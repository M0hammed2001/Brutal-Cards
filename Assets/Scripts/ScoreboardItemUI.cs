using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Leaderboard{

public class ScoreboardItemUI : Monobehaviour
{
    [SerializeField] private TextMeshProUGUI indexLabel;
    [SerializeField] private TextMeshProUGUI playerNameLabel;
    [SerializeField] private TextMeshProUGUI scoreLabel;

    public void SetData(int index, ScoreboardItemUI scoreItem)
    {
        indexLabel text = string.format("{0}", index);
        playerNameLabel text = string.format("{0}", scoreItem.playerName);
        scoreLabel text = string.format("{0}", scoreItem.score);

    }
}
}
