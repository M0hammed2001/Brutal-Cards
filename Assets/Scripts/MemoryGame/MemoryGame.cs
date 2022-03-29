
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using Unity;

namespace BrutalCards
{
    public class MemoryGame : MonoBehaviour
    {
        SceneController sceneController;


        public Player localPlayer;
        public Player remotePlayer;
        public float x;
        
        public enum GameState
        {
            Idle,
            GameStarted,
            TurnStarted,
            TurnSelectingCards,
            CheckingPairs,
            GameFinished
        };

        [SerializeField]
        protected GameState gameState = GameState.Idle;

        protected void Awake()
        {

        }

        
        protected void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();

        }

    

        public virtual void GameFlow(){
            Debug.Log("running part 1");
            Debug.Log("Scene Controller SCORE: "+ sceneController.bot_score);
            if (gameState > GameState.GameStarted)
            {
                Debug.Log("running part 2");
                if (sceneController.bot_score >= 7)
                {
                    Debug.Log("running part 3");
                    gameState = GameState.GameFinished;
                }
                else if(sceneController.player_score >= 7)
                {
                    gameState = GameState.GameFinished;
                }
            }
            else
            {
                gameState = GameState.GameStarted;
            }
            Debug.Log("GameState: " + gameState);
            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
                        OnGameStarted();
                        break;
                        
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        OnGameStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnTurnStarted();
                        break;
                    }
                case GameState.TurnSelectingCards:
                    {
                        Debug.Log("TurnSelectingCards");
                        OnTurnSelectingCards();
                        break;
                    }
                case GameState.CheckingPairs:
                    {
                        Debug.Log("CheckingPairs");
                        OnCheckingPairs();
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        protected virtual void OnGameStarted()
        {

            gameState = GameState.TurnStarted;
            Debug.Log("onGameStarted: " + gameState);
            GameFlow();
            
        }

        protected virtual void OnTurnStarted()
        {
            Debug.Log("hello4");
            sceneController.SwitchTurn();
            gameState = GameState.TurnSelectingCards;
            GameFlow();
        }

        protected virtual void OnTurnSelectingCards()
        {
            if (sceneController.currentTurnPlayer == sceneController.localPlayer)
            {
                Debug.Log("2"); 
                gameState = GameState.CheckingPairs;
            }
            else
            {
                Debug.Log("3");   
                sceneController.AiCardpick();
                gameState = GameState.CheckingPairs;
            }
        }

        protected virtual void OnCheckingPairs()
        {
            if( sceneController.CheckingMatch() == true)
            {
                gameState = GameState.TurnSelectingCards;
            }
            else
            {

                sceneController.SwitchTurn();
                gameState = GameState.TurnSelectingCards;
            }
        }

        protected virtual void OnGameFinished()
        {
            if(sceneController.bot_score < sceneController.player_score)
            {
                
            }
            else if(sceneController.player_score < sceneController.bot_score)
            {

            }
            else
            {
                
            }
        }

        public MemoryGame.GameState GetGameState()
        {
            return gameState;
        }

        public void SetGameState(MemoryGame.GameState gameStated)
        {
            gameState = gameStated;
        }

        
    }
    
}
