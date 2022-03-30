using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField] private LeaderboardData leaderboarddata = new LeaderboardData();

    public void SaveIntoJson()
    {
        string brutalwins = JsonUtility.ToJson(leaderboarddata);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/Brutalwins.json", brutalwins);
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<Data> data = new List<Data>();
}

[System.Serializable]
public class Data
{
    public string playerName;
    public int highscore;
}