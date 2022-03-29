using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BrutalCards{

public class LeaderboardPopoverUI : MonoBehaviour
{
       [SerializeField] private GameObject LeaderboardPopover;
        
       [SerializeField] private RectTransform scoreboardPanel;

       [SerializeField] private GameObject scoreboardItemPrefab;
       
    void Start()
    {
       if (ScoreboardManager.Exists)
      {
        if (MatchInfoSetUp.Exists)
        {
          MatchInfo info = MatchInfoSetUp.Instance.CurrentInfo;
          if (info != null)
          {
            ScoreboardManager.Instance.AddNewEntry(info);
          }
        }
      }
     
      PopulateLeaderboardPopover();
    } 

public void PopulateLeaderboardPopover()
  {
    if (scoreboardItemPrefab == null)
    {
      return;
    }
   
    if (ScoreboardManager.Exists)
    {
      int index = 0;
      foreach(ScoreboardItem item in ScoreboardManager.Instance.Scoreboardlist)
      {
        Gameobject go = Instantiate(scoreboardItemPrefab);
        ScoreboardItemUi itemUI = go.GetComponent<ScoreboardItemUI>();
        itemUI.SetData(index, item);
        if (scoreboardPanel)
        {
          go.Transform.SetParent(scoreboardPanel);
        }
        index++;
      }
    }
}  
}   
}

 


