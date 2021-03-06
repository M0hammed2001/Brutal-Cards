using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace BrutalCards{

    public class SceneController : MonoBehaviour {


        [SerializeField] private MemoryGame memoryGame;
        [SerializeField] private MemoryCard originalCard;
        [SerializeField] private Sprite[] images;

        [SerializeField]List<MemoryCard> aiCardsToPick = new List <MemoryCard>();
        
        
        int randomNumber;

        public Player localPlayer;
        public Player remotePlayer;

        public GameObject LobbyButton;
        public GameObject OptionsPopover;
        public GameObject RulesPopover;
        public GameObject PopoverBackground;
        public GameObject MultiRulePopover;
        public GameObject MemoryRulesPopover;
        public GameObject DeadlyFishRulesPopover;
        public AudioSource audioSource;
        public AudioClip pick, collect, wrong;

        



        [SerializeField]
        public Player currentTurnPlayer;

        [SerializeField]
        ProtectedData protectedData;
    
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

        }
         public void OnLobbyClicked()
        {
            Debug.Log("OnLobbyClicked");
            SceneManager.LoadScene("LobbyScene");
        }
        void OnGUI(){
            if (Input.GetKeyDown("escape")){
                Debug.Log("KeyCode down: escape");
                OnOptionsClicked();
            }   
        }
        public void OnRulesCancelClicked(){
            RulesPopover.SetActive(false);
            OptionsPopover.SetActive(true);
        }
        public void ShowRulesPopover()
        {
            RulesPopover.SetActive(true);
            OptionsPopover.SetActive(false);
        }
        public void OnRulesClicked()
        {
            Debug.Log("OnRulesClicked");
            ShowRulesPopover();
        }
        public void ShowOptionsPopover()
        {
            PopoverBackground.SetActive(true);
            OptionsPopover.SetActive(true);
        }
        public void OnCancelClicked(){
            PopoverBackground.SetActive(false);
            OptionsPopover.SetActive(false);

        }
        public void OnOptionsClicked()
        {
            Debug.Log("OnOptionsClicked");
            ShowOptionsPopover();
        }
        public void ShowMultiRulePopover()
        {
            MultiRulePopover.SetActive(true);
            OptionsPopover.SetActive(false);
        }
        public void ShowDeadlyFishRulesPopover(){
            DeadlyFishRulesPopover.SetActive(true);
            OptionsPopover.SetActive(false);
            MultiRulePopover.SetActive(false);
            
        }
        public void ShowMemoryRulesPopover(){
            MemoryRulesPopover.SetActive(true);
            OptionsPopover.SetActive(false);
            MultiRulePopover.SetActive(false);
            

        }
        void HideAllPopover()
        {
            PopoverBackground.SetActive(false);
            OptionsPopover.SetActive(false);
            RulesPopover.SetActive(false);
            
        }
        



        public void Start()
        {
            Vector3 startPos = originalCard.transform.position; //position set for the first card. the others have been ofset from this position

            SwitchTurn();
            HideAllPopover();
            int[] numbers =  { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11};
            ShuffleArray(numbers); 

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
                    int id = protectedData.memoryGameArray[index];
                    card.ChangeSprite(id, images[id]);

                    float posX = (Constants.offsetX * i) + startPos.x;
                    float posY = (Constants.offsetY * j) + startPos.y;
                    card.transform.position = new Vector3(posX, posY, startPos.z);
                }
            }
        }

        

        private int[] ShuffleArray(int[] number)
        {
            int[] newArray = number;
            for(int i = 0; i < newArray.Length; i++)
            {
                int tmp = newArray[i];
                int r = Random.Range(i, newArray.Length);
                newArray[i] = newArray[r];
                newArray[r] = tmp;
            }
            protectedData.memoryGameArray = newArray;
            return newArray;
        }

       

        private MemoryCard _firstRevealed;
        private MemoryCard _secondRevealed;

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
                audioSource.PlayOneShot(pick, 1f);
                _firstRevealed = card;
            }
            else
            {
                audioSource.PlayOneShot(pick, 1f);
                _secondRevealed = card;
                StartCoroutine(CheckMatch());
            }
        }

        public void AiCardpick()
        {
            int r = Random.Range(0, aiCardsToPick.Count);
            int t = Random.Range(0, aiCardsToPick.Count);
            while (r == t)
            {
                t = Random.Range(0, aiCardsToPick.Count);
            }
            CardRevealed(aiCardsToPick[r]);

            
        }

        public bool CheckingMatch()
        {
            if(_firstRevealed.id == _secondRevealed.id) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator CheckMatch()
        {
            if(_firstRevealed.id == _secondRevealed.id)
            {
                audioSource.PlayOneShot(collect, 1f);
                 if(currentTurnPlayer == localPlayer)
                {
                    player_score++;
                    playerScore.text = "Player Score: " + player_score;
                }
                else
                {
                    bot_score++;
                    botScore.text = "Bot Score: " + bot_score;
                }
            }
           else
            {
                audioSource.PlayOneShot(wrong, 1f);
                yield return new WaitForSeconds(0.5f);

                _firstRevealed.Unreveal();
                _secondRevealed.Unreveal();
                SwitchTurn();
          }

            _firstRevealed = null;
            _secondRevealed = null;

        }

        // public void Restart()
        // {
        //     SceneManager.LoadScene("Scene_001");
        // }

    }
}