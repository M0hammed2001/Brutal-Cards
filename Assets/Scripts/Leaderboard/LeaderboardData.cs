using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LeaderboardData
{
    InputField NicknameInputField;
    string filename;
    
    public List<Data> data = new List<Data>();

    private void Start()
    {
        data = FileHandler.ReadListFromJSON<Data>(filename);
    }

    public void AddNameToList()
    {
        data.Add(new Data(NicknameInputField.text));
        FileHandler.SaveToJSON<Data>(data, filename);
    }
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