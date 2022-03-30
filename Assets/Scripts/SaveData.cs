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

// Input Entry
[System.Serializable]

    public class Data
    {

    public string playerName;
    public int highscore = 0;
    // not a list, needes to be stored in json in form of an array -- brutalwins
    // to use references from other classes need to make the class to use static or 
    // have to add to the scene or instanntiate it.
    //from the readfromjson method, , (contents) does not need to be converted back to list. remoove .toList 
    // use jsonhelper when saving list not jsonutility
    }