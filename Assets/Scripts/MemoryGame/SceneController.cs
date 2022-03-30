using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using SWNetwork;
using System.Linq;



namespace BrutalCards
{

    public class SceneController : MonoBehaviour {

        EncryptedData encryptedData;
        NetCode netCode;
        [SerializeField]
        public List<byte> localMemoryArray = new List<byte>();


        [SerializeField] private MemoryCard originalCard;
        [SerializeField] private Sprite[] images;

        [SerializeField] List<MemoryCard> aiCardsToPick = new List <MemoryCard>();
        
        

        MemoryGame memoryGame;
        MemoryMultiplayer randomizer;
        
        int randomNumber;

        public Player localPlayer;
        public Player remotePlayer;


        [SerializeField]
        public Player currentTurnPlayer;

        [SerializeField]
        ProtectedData protectedData;
        MemoryCard memoryCards;
    
        public SceneController(Player local, Player remote, string roomId = "1234567890123455"){
            localPlayer = local;
            remotePlayer = remote;
            protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId, roomId);
        }
        


        protected void Awake()
        {
            Debug.Log("base awake");
            localPlayer = new Player();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";

            remotePlayer = new Player();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.IsAI = true;
            SwitchTurn();
            if (NetworkClient.Instance.IsHost){
                byte[] numbers = { 0, 0, 8, 1, 2, 7, 3, 3, 4, 4, 5, 5, 6, 6, 7, 2, 1, 8, 9, 9, 10, 11, 10, 11};
                for (int i =0; i < numbers.Length; i++ ){
                    byte tmp = numbers[i];
                    int r = UnityEngine.Random.Range(i, numbers.Length);
                    numbers[i]= numbers[r];
                    numbers[r] = tmp;
                }
                protectedData.gameMemoryArray.AddRange(numbers);
            }
            localMemoryArray = protectedData.gameMemoryArray;
        }


        public void Start()
        {
            Vector3 startPos = originalCard.transform.position; //position set for the first card. the others have been ofset from this position

            
            
            
            ShuffleArray(); 

            for(int i = 0; i < Constants.gridCols; i++)
            {
                for(int j = 0; j < Constants.gridRows; j++)
                {
                    MemoryCard card;

                    if(i == 0 && j == 0)
                    {
                        card = originalCard;
                    }
                    else
                    {
                        card = Instantiate(originalCard) as MemoryCard;
                    }
                    aiCardsToPick.Add(card);
                    
                    int index = j * Constants.gridCols + i;

                    int id = localMemoryArray[index];
                    card.ChangeSprite(id, images[id]);

                    float posX = (Constants.offsetX * i) + startPos.x;
                    float posY = (Constants.offsetY * j) + startPos.y;
                    card.transform.position = new Vector3(posX, posY, startPos.z);
                }
            }
            
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------

        private List<byte> ShuffleArray()
        {
            List<byte> newArray = new List <byte>();
            newArray = protectedData.GetMemoryCards();
            for(int i = 0; i < newArray.Count; i++)
            {
                byte tmp = newArray[i];
                int r = UnityEngine.Random.Range(i, newArray.Count);
                newArray[i] = newArray[r];
                Debug.Log(newArray[i]);
                newArray[r] = tmp;
            }
            protectedData.gameMemoryArray = newArray;
            return newArray;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------

        private MemoryCard _firstRevealed;
        private MemoryCard _secondRevealed;
        public bool checkingMatch;

        public int bot_score = 0;
        public int player_score = 0;
        [SerializeField] public TextMesh botScore;
        [SerializeField] public TextMesh playerScore;
        [SerializeField] public TextMesh playersTurn;
        
        public void SwitchTurn(){
            if (currentTurnPlayer == null)
            {
                currentTurnPlayer = localPlayer;
                playersTurn.text = localPlayer + "'s Turn" ;
                return;

            }

            if (currentTurnPlayer == localPlayer)
            {
                currentTurnPlayer = remotePlayer;
                playersTurn.text = remotePlayer.PlayerName + "'s Turn" ;
            }
            else
            {
                currentTurnPlayer = localPlayer;
                playersTurn.text = localPlayer.PlayerName + "'s Turn" ;
            }
        }

        public bool canReveal
        {
            get { return _secondRevealed == null; }
        }

        public void CardRevealed(MemoryCard card)
        {
            if(_firstRevealed == null)
            {
                _firstRevealed = card;
            }
            else
            {
                _secondRevealed = card;
                StartCoroutine(CheckMatch());
            }
        }

        public void AiCardpick()
        {
            int r = UnityEngine.Random.Range(0, aiCardsToPick.Count);
            int t = UnityEngine.Random.Range(0, aiCardsToPick.Count);
            while (r == t)
            {
                t = UnityEngine.Random.Range(0, aiCardsToPick.Count);
            }
            memoryCards.AiClicking(aiCardsToPick[r]);

            
        }


        public IEnumerator CheckMatch()
        {
            if(_firstRevealed.id == _secondRevealed.id)
            {
                checkingMatch = true;
                if(currentTurnPlayer == localPlayer)
                {
                    player_score++;
                    playerScore.text = ("Player Score: " + player_score);
                    
                }
                else
                {
                    bot_score++;
                    botScore.text = ("Bot Score: " + bot_score);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                checkingMatch = false;
                _firstRevealed.Unreveal();
                _secondRevealed.Unreveal();
                SwitchTurn();
                
            }

            _firstRevealed = null;
            _secondRevealed = null;

        }
        
        public EncryptedData EncryptedData()
        {
            Byte[] data = protectedData.ToArray();

            EncryptedData encryptedData = new EncryptedData();
            encryptedData.data = data;

            return encryptedData;
        }

        public void SetCurrentTurnPlayer(Player player){
            protectedData.SetCurrentTurnPlayerId(player.PlayerId);
        }

        public Player GetCurrentTurnPlayer(){
            string playerId = protectedData.GetCurrentTurnPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else
            {
                return remotePlayer;
            }
        }

        public void ApplyEncrptedData(EncryptedData encryptedData){
            if(encryptedData == null)
            {
                return;
            }

            protectedData.ApplyByteArray(encryptedData.data);
        }

    
    }
}