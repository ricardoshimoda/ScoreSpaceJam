﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class ScoringManager : Singleton<ScoringManager>
{
    public int test = 0;
    public UserData currentPlayer;
    private static int state;
    // Start is called before the first frame update
    public string gameTitle = "Getaway Taxi";
    public string gameScene = "Game";
    public string urlBase = "https://rdifbvxni2.execute-api.us-east-1.amazonaws.com/default/";
    private UserPlay latestPlay;
    public bool UpdateScore(UserPlay playResults){
        if (currentPlayer==null)
            return false;

        Debug.Log("Whoa");
        var playScore = playResults.score + playResults.bonus;
        if(currentPlayer.top_score < playScore)
            currentPlayer.top_score = playScore;
        if(currentPlayer.top_rounds < playResults.rounds)
            currentPlayer.top_rounds = playResults.rounds;
        if(currentPlayer.top_time < playResults.time)
            currentPlayer.top_time = playResults.time;
        currentPlayer.total_matches++;
        // Now we call the lambda function
        latestPlay = playResults;
        latestPlay.player_id_match = currentPlayer.user_id + "-" + currentPlayer.total_matches;
        latestPlay.session_tk = currentPlayer.session_tk;
        Debug.Log(" Before Coroutine vWhoa");
        StartCoroutine("PostScore");
        return true;
    }

    private IEnumerator PostScore(){

        Debug.Log("Making Request Object");
        var requestBody = new ScoreUpdateRequest(){
            Username = currentPlayer.user_id,
            SessionToken = currentPlayer.session_tk,
            top_score = currentPlayer.top_score,
            top_rounds = currentPlayer.top_rounds ,
            top_time = currentPlayer.top_time,
            total_matches = currentPlayer.total_matches,
            user_id_match = latestPlay.player_id_match,
            score = latestPlay.score,
            rounds = latestPlay.rounds,
            time = latestPlay.time,
            bonus = latestPlay.bonus,
        };
        var postData = JsonUtility.ToJson(requestBody);
        Debug.Log("Calling, calling Request Object");
        Debug.Log(urlBase + "BTUsrMgm");
        Debug.Log(postData);
        using (UnityWebRequest www = UnityWebRequest.Put(urlBase + "BTUsrMgm", postData))
        {
            www.method = UnityWebRequest.kHttpVerbPUT;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
 
            yield return www.SendWebRequest();
 
            if (www.isNetworkError)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Yay! " + www.downloadHandler.text);
            }
        }
    }




}
