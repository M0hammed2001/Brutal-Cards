using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SWNetwork;

namespace BrutalCards
{
    public class Lobby : MonoBehaviour
    {
        public enum LobbyState
        {
            Default,
            JoinedRoom,
        }
        public LobbyState State = LobbyState.Default;
        public bool Debugging = false;
        public bool playedOnce = false;

        public GameObject PopoverBackground;
        public GameObject EnterNicknamePopover;
        public GameObject OptionsPopover;
        public GameObject LeaderboardPopover;
        public GameObject GamesPopover;
        public GameObject RulesPopover;
        public GameObject WaitForOpponentPopover;
        public GameObject StartRoomButton;
        public GameObject MemoryFromHellButton;
        public GameObject DeadlyFishButton;
        public InputField NicknameInputField;

        public GameObject Player1Portrait;
        public GameObject Player2Portrait;

        [SerializeField]
        public Data data;



        string nickname;

        private void Start()
        {
            // disable all online UI elements
            HideAllPopover();
            if (playedOnce == false){
                FindObjectOfType<AudioManager>().Play("Music");
                playedOnce = true;
            }
            NetworkClient.Lobby.OnLobbyConnectedEvent += OnLobbyConnected;
            NetworkClient.Lobby.OnNewPlayerJoinRoomEvent += OnNewPlayerJoinRoomEvent;
            NetworkClient.Lobby.OnRoomReadyEvent += OnRoomReadyEvent;
        }

        private void OnDestroy()
        {
            if (NetworkClient.Lobby != null)
            {
                NetworkClient.Lobby.OnLobbyConnectedEvent -= OnLobbyConnected;
                NetworkClient.Lobby.OnNewPlayerJoinRoomEvent -= OnNewPlayerJoinRoomEvent;
            }
        }

        void ShowEnterNicknamePopover()
        {
            PopoverBackground.SetActive(true);
            EnterNicknamePopover.SetActive(true);
        }

        public void ShowLeaderboardPopover()
        {
            PopoverBackground.SetActive(true);
            LeaderboardPopover.SetActive(true);
        }

        public void ShowOptionsPopover()
        {
            PopoverBackground.SetActive(true);
            OptionsPopover.SetActive(true);
        }
        public void ShowGamesPopover()
        {
            PopoverBackground.SetActive(true);
            GamesPopover.SetActive(true);
        }
        public void ShowRulesPopover()
        {
            RulesPopover.SetActive(true);
            OptionsPopover.SetActive(false);
        }

        void ShowJoinedRoomPopover()
        {
            EnterNicknamePopover.SetActive(false);
            WaitForOpponentPopover.SetActive(true);
            StartRoomButton.SetActive(false);
            Player1Portrait.SetActive(false);
            Player2Portrait.SetActive(false);
        }

        void ShowReadyToStartUI()
        {
            StartRoomButton.SetActive(true);
            Player1Portrait.SetActive(true);
            Player2Portrait.SetActive(true);
        }

        void HideAllPopover()
        {
            PopoverBackground.SetActive(false);
            EnterNicknamePopover.SetActive(false);
            WaitForOpponentPopover.SetActive(false);
            OptionsPopover.SetActive(false);
            GamesPopover.SetActive(false);
            LeaderboardPopover.SetActive(false);
            RulesPopover.SetActive(false);
            StartRoomButton.SetActive(false);
            Player1Portrait.SetActive(false);
            Player2Portrait.SetActive(false);
        }
        


        //****************** Matchmaking *********************//
        void Checkin()
        {
            NetworkClient.Instance.CheckIn(nickname, (bool successful, string error) =>
            {
                if (!successful)
                {
                    Debug.LogError(error);
                }
            });
        }

        void RegisterToTheLobbyServer()
        {
            NetworkClient.Lobby.Register(nickname, (successful, reply, error) => {
                if (successful)
                {
                    Debug.Log("Lobby registered " + reply);
                    if (string.IsNullOrEmpty(reply.roomId))
                    {
                        JoinOrCreateRoom();
                    }
                    else if (reply.started)
                    {
                        State = LobbyState.JoinedRoom;
                        ConnectToRoom();
                    }
                    else
                    {
                        State = LobbyState.JoinedRoom;
                        ShowJoinedRoomPopover();
                        GetPlayersInTheRoom();
                    }
                }
                else
                {
                    Debug.Log("Lobby failed to register " + reply);
                }
            });
        }

        void JoinOrCreateRoom()
        {
            NetworkClient.Lobby.JoinOrCreateRoom(false, 2, 60, (successful, reply, error) => {
                if (successful)
                {
                    Debug.Log("Joined or created room " + reply);
                    State = LobbyState.JoinedRoom;
                    ShowJoinedRoomPopover();
                    GetPlayersInTheRoom();
                }
                else
                {
                    Debug.Log("Failed to join or create room " + error);
                }
            });
        }

        void GetPlayersInTheRoom()
        {
            NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) => {
                if (successful)
                {
                    Debug.Log("Got players " + reply);
                    if(reply.players.Count == 1)
                    {
                        Player1Portrait.SetActive(true);
                    }
                    else
                    {
                        Player1Portrait.SetActive(true);
                        Player2Portrait.SetActive(true);

                        if (NetworkClient.Lobby.IsOwner)
                        {
                            ShowReadyToStartUI();
                        }
                    }
                }
                else
                {
                    Debug.Log("Failed to get players " + error);
                }
            });
        }

        void LeaveRoom()
        {
            NetworkClient.Lobby.LeaveRoom((successful, error) => {
                if (successful)
                {
                    Debug.Log("Left room");
                    State = LobbyState.Default;
                }
                else
                {
                    Debug.Log("Failed to leave room " + error);
                }
            });
        }

        void StartRoom()
        {
            NetworkClient.Lobby.StartRoom((successful, error) => {
                if (successful)
                {
                    Debug.Log("Started room.");
                }
                else
                {
                    Debug.Log("Failed to start room " + error);
                }
            });
        }

        void ConnectToRoom()
        {
            // connect to the game server of the room.
            NetworkClient.Instance.ConnectToRoom((connected) =>
            {
                if (connected)
                {
                    SceneManager.LoadScene("DeadlyFishMultiplayer");
                }
                else
                {
                    Debug.Log("Failed to connect to the game server.");
                }
            });
        }

        //****************** Lobby events *********************//
        void OnLobbyConnected()
        {
            RegisterToTheLobbyServer();
        }

        void OnNewPlayerJoinRoomEvent(SWJoinRoomEventData eventData)
        {
            if (NetworkClient.Lobby.IsOwner)
            {
                ShowReadyToStartUI();
            }
        }

        void OnRoomReadyEvent(SWRoomReadyEventData eventData)
        {
            ConnectToRoom();
        }

        //****************** UI event handlers *********************//
        /// <summary>
        /// Practice button was clicked.
        /// </summary>
        /// 

        public void OnQuitClicked()
        {
            Debug.Log("OnQuitClicked");
            Application.Quit();
        }
        public void OnPracticeClicked()
        {
            FindObjectOfType<AudioManager>().Play("Creeky Door");
            Debug.Log("OnPracticeClicked");
            ShowGamesPopover();
        }

        public void OnDeadlyFishClicked()
        {
            FindObjectOfType<AudioManager>().Play("Creeky Door");
            Debug.Log("OnDeadlyFishClicked");
            SceneManager.LoadScene("DeadlyFish");
        }

        public void OnMemoryFromHellClicked()
        {
            FindObjectOfType<AudioManager>().Play("Creeky Door");
            Debug.Log("OnMemoryFromHellClicked");
            SceneManager.LoadScene("MemoryFromHell");
        }

        /// <summary>
        /// Online button was clicked.
        /// </summary>
        public void OnOnlineClicked()
        {
            Debug.Log("OnOnlineClicked");
            ShowEnterNicknamePopover();
        }
        
        public void OnOptionsClicked()
        {
            Debug.Log("OnOptionsClicked");
            ShowOptionsPopover();
        }
        public void OnGamesClicked()
        {
            Debug.Log("OnOptionsClicked");
            ShowGamesPopover();
        }
        public void OnLeaderboardClicked()
        {
            Debug.Log("OnLeaderboardClicked");
            ShowLeaderboardPopover();
        }
        
        public void OnRulesClicked()
        {
            Debug.Log("OnRulesClicked");
            ShowRulesPopover();
        }

        /// <summary>
        /// Cancel button in the popover was clicked.
        /// </summary>
        public void OnCancelClicked()
        {
            Debug.Log("OnCancelClicked");

            if (State == LobbyState.JoinedRoom)
            {
                // TODO: leave room.
                LeaveRoom();
            }

            HideAllPopover();
        }

        public void OnRulesCancelClicked(){
            RulesPopover.SetActive(false);
            OptionsPopover.SetActive(true);
        }

        /// <summary>
        /// Start button in the WaitForOpponentPopover was clicked.
        /// </summary>
        public void OnStartRoomClicked()
        {
            Debug.Log("OnStartRoomClicked");
            // players are ready to player now.
            if (Debugging)
            {
                SceneManager.LoadScene("DeadlyFishMultiplayer");
            }
            else
            {
                // Start room
                StartRoom();
            }
        }

        /// <summary>
        /// Ok button in the EnterNicknamePopover was clicked.
        /// </summary>
        public void OnConfirmNicknameClicked()
        {
            nickname = NicknameInputField.text;
            // To have as input into brutalWins list tally using nickname as key to value
            nickname = data.playerName;
            Debug.Log($"OnConfirmNicknameClicked: {nickname}");

            if (Debugging)
            {
                ShowJoinedRoomPopover();
                ShowReadyToStartUI();
            }
            else
            {
                //Use nickname as player custom id to check into SocketWeaver.
                Checkin();
            }
        }
    }
}