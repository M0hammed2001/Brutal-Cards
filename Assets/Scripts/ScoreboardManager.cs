using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard{

public class ScoreboardManager : MonoSingleton<ScoreboardManager>
{
    private List<ScoreboardItem> scoreboardList = new List<ScoreboardItem>();

    public List<ScoreboardItem> ScoreboardList { get { return scoreboardList; } }

    protected void Initialzie()
    {
        base.Initialzie();

        LoadScoreboardData();
    }

    public void AddNewEntry(MatchInfo info)
    {
        bool found = false;
        for (int itemIndex = 0; ItemIndex < scoreboardList.count; ++itemIndex)
        {
            if (scoreboardList[ItemIndex].playerName == info.PlayerName)
            {
                found = true;
                break;
            }
        }

        if (found == false)
        {
            ScoreboardItem item = new ScoreboardItem();
            item.SetData(info);
            scoreboardList.Add(Item);
        }

        scoreboardList.Sort((item1, item2) => item1.score.CompareTo(item2.score));

        SaveScoreboardData();
    }

    public void RemoveEntryByPlayerName(string playerName)
    {
        int indexFound = -1;
        for(int itemIndex = 0; itemIndex < scoreboardList.Count; ++itemIndex)
        {
            if(scoreboardList[itemIndex].playerName == playerName);
            {
            indexFound = itemIndex;
            break;
            }
        if (indexFound != -1)
        {
            scoreboardList.RemoveAt(indexFound);
        }
        }
    
         ScoreboardItem GetScoreboardItembyName(string playerName)
         {
        ScoreboardItem item = null;
        for (int itemIndex = 0; itemIndex < scoreboardList.Count; ++itemIndex) 
        {
            if (scoreboardList[itemIndex].playerName == playerName) ;
            {
                item = scoreboardList[itemIndex];
                break;
            }
        }
        return item;
    }
        void SaveScoreboardData()
        {
            string filename = string.Format("{0}/[1}", Application.persistentDataPath, "scorelist.json");
            StreamWriter = new StreamWriter(filename);
            Debug.LogError("Saving Data...");
            foreach (ScoreboardItem item in scoreboardList)
            {
                string jsonData = JsonUtility.ToJson(item);
                Debug.LogError(jsonData);
                streamWriter.WriteLine(jsonData);

            }
            streamWriter.Close();
            Debug.LogError("... end");
        }

        void LoadScoreboardData()
        {
            string filename = string.Format("{0}/[1}", Application.persistentDataPath, "scorelist.json");
            if (File.Exists(filename))
            {
                StreamReader streamReader = new StreamReader(filename);
                Debug.LogError("Loading data...");
                while(!streamReader.EndOfStream)
                {
                    string jsonData = streamReader.ReadLine();
                    Debug.LogError(jsonData);
                    ScoreboardItem item = JsonUtility.FromJson<ScoreboardItem>(jsonData);
                    scoreboardList.Add(item);
                }
                streamReader.Close();
                Debug.LogError("..end");
            }
            
    }
    }
}
}