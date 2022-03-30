using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.SceneManagement;

namespace BrutalCards
{
    public class MemoryMultiplayer : MemoryGame
    {


        NetCode netCode;

        ProtectedData protectedData;

        protected new void Awake()
        {
            Debug.Log("awake called");
            Debug.Log("1");
            base.Awake();
            
            remotePlayer.IsAI = false;

            netCode = FindObjectOfType<NetCode>();

            NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
            {
                if (successful)
                {
                    foreach(SWPlayer swPlayer in reply.players)
                    {
                        string playerName = swPlayer.GetCustomDataString();
                        string playerId = swPlayer.id;

                        if (playerId.Equals(NetworkClient.Instance.PlayerId))
                        {
                            localPlayer.PlayerId = playerId;
                            localPlayer.PlayerName = playerName;
                        }
                        else
                        {
                            remotePlayer.PlayerId = playerId;
                            remotePlayer.PlayerName = playerName;
                        }
                    }

                    sceneController = new SceneController(localPlayer, remotePlayer, NetworkClient.Lobby.RoomId);
                    netCode.EnableRoomPropertyAgent();
                }
                else
                {
                    Debug.Log("Failed to get players in room.");
                }

            });
            

        }
        protected new void Start()
        {
            if (NetworkClient.Instance.IsHost)
            {
                sceneController.localMemoryArray = protectedData.GetMemoryCards();
            }
            netCode.ModifyGameDataMemory(sceneController.EncryptedData());
            netCode.OnEncryptedDataChanged();
            Debug.Log("4");
            Debug.Log("Multiplayer Game Start");
            gameState = GameState.GameStarted;
            netCode.OnEncryptedDataChanged();
        }

        //****************** Game Flow *********************//

        public override void GameFlow()
        {
            Debug.LogError("Should never be here");
        }

        protected override void OnGameStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                Vector3 startPos = sceneController.originalCard.transform.position; //position set for the first card. the others have been ofset from this position
                sceneController.localMemoryArray = protectedData.gameMemoryArray;
                Debug.Log("wvbrhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
                for(int v = 0; v < sceneController.localMemoryArray.Count; v++)
                {
                    Debug.Log("hwdhbvijefr" + sceneController.localMemoryArray[v]);
                }
            
                int index = 0;
                for(int i = 0; i < Constants.gridCols; i++)
                {
                    for(int j = 0; j < Constants.gridRows; j++)
                    {
                        MemoryCard card;

                        if(i == 0 && j == 0)
                        {
                            card = sceneController.originalCard;
                        }
                        else
                        {
                            card = Instantiate(sceneController.originalCard) as MemoryCard;
                        }
                        

                        index = j * Constants.gridCols + i;
                        int id = sceneController.localMemoryArray[index];
                        card.ChangeSprite(id, sceneController.images[id]);

                        float posX = (Constants.offsetX * i) + startPos.x;
                        float posY = (Constants.offsetY * j) + startPos.y;
                        card.transform.position = new Vector3(posX, posY, startPos.z);
                    }
                }
                Debug.Log("ONGAMESTARTED NETWORK");
                gameState = GameState.TurnStarted;

                netCode.ModifyGameDataMemory(sceneController.EncryptedData());
            }
            Debug.Log("ONGAMESTARTED NETWORK 2");
            GameFlow();
        }
        
        protected override void OnTurnStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                Debug.Log("ONTURNSTARTED NETWORK");
                sceneController.SwitchTurn();
                gameState = GameState.TurnSelectingCards;

                sceneController.SetCurrentTurnPlayer(sceneController.currentTurnPlayer);
                SetGameState(gameState);

                netCode.ModifyGameData(sceneController.EncryptedData());
                netCode.NotifyOtherPlayersGameStateChanged();
            }
            Debug.Log("ONTURNSTARTED NETWORK 2");
        }

        protected override void OnTurnSelectingCards()
        {
            if (sceneController.currentTurnPlayer == localPlayer)
            {

            }
            else
            {

            }

            if (NetworkClient.Instance.IsHost)
            {
                Debug.Log("ONTURNSELECTINGCARDS NETWORK");
                gameState = GameState.CheckingPairs;
                SetGameState(gameState);

                netCode.ModifyGameData(sceneController.EncryptedData());
                netCode.NotifyOtherPlayersGameStateChanged();
            }
                Debug.Log("ONTURNSELECTINGCARDS NETWORK 2");
        }

        protected override void OnCheckingPairs()
        {
            if( sceneController.checkingMatch == true)
            {
                gameState = GameState.TurnSelectingCards;
            }
            else
            {
                sceneController.SwitchTurn();
                gameState = GameState.TurnSelectingCards;
            }

            netCode.ModifyGameData(sceneController.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }

//*********************************************************************

        public void OnGameDataReady(EncryptedData encryptedData)
        {
            if(encryptedData == null)
            {
                Debug.Log("New game");
                if (NetworkClient.Instance.IsHost==true)
                {
                    Debug.Log("ONGAMEDATAREADY NETWORK");
                    gameState = GameState.GameStarted;
                    SetGameState(gameState);

                    netCode.ModifyGameData(sceneController.EncryptedData());

                    netCode.NotifyOtherPlayersGameStateChanged();
                }
            }
            else
            {
                Debug.Log("ONGAMEDATAREADY NETWORK 2");               
                sceneController.ApplyEncrptedData(encryptedData);
                gameState = GetGameState();
                sceneController.currentTurnPlayer = sceneController.GetCurrentTurnPlayer();

                if(gameState > GameState.GameStarted)
                {
                    Debug.Log("Restore the game state");

                    base.GameFlow();
                }
            }
        }

        public void OnGameDataChanged(EncryptedData encryptedData)
        {
            sceneController.ApplyEncrptedData(encryptedData);
            gameState = GetGameState();
            sceneController.currentTurnPlayer = sceneController.GetCurrentTurnPlayer();
        }
    
        public void OnGameStateChanged()
        {
            base.GameFlow();
        }

        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

    }

}