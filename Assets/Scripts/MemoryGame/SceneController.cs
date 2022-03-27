using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace BrutalCards{

    public class SceneController : MonoBehaviour {

        public const int gridRows = 3;
        public const int gridCols = 8;
        public const float offsetX = 2.3f;
        public const float offsetY = 3.3f;
        public float x;
        public AudioSource audioSource;
        public AudioClip pick, collect, wrong;

        public GameObject OptionsPopover;
        public GameObject PopoverBackground;

        [SerializeField] private MemoryCard originalCard;
        [SerializeField] private Sprite[] images;
        
        Player localPlayer;
        Player remotePlayer;

        [SerializeField]
        protected Player currentTurnPlayer;

        [SerializeField]
        ProtectedData protectedData;
    
        public SceneController(Player local, Player remote, string roomId = "1234567890123455"){
            localPlayer = local;
            remotePlayer = remote;
            protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId, roomId);
        }
        

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
            Debug.Log("base awake");
            localPlayer = new Player();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";

            remotePlayer = new Player();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.IsAI = true;

        }
         public void ShowOptionsPopover()
        {
            PopoverBackground.SetActive(true);
            OptionsPopover.SetActive(true);
        }
        void OnGUI(){
            if (Input.GetKeyDown("escape")){
                Debug.Log("KeyCode down: escape");
                OnOptionsClicked();
            }   
        }

        public void OnOptionsClicked()
        {
            Debug.Log("OnOptionsClicked");
            ShowOptionsPopover();
        }
         public void OnLobbyButtonClicked()
        {
            // FindObjectOfType<AudioManager>().Play("Creeky Door");
            Debug.Log("OnLobbyButtonClicked");
            SceneManager.LoadScene("LobbyScene");
        }
        public void OnCancelClicked(){
            PopoverBackground.SetActive(false);
            OptionsPopover.SetActive(false);

        }

        
        private void Start()
        {
            Vector3 startPos = originalCard.transform.position; //The position of the first card. All other cards are offset from here.

            int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11};
            numbers = ShuffleArray(numbers); //This is a function we will create in a minute!

            for(int i = 0; i < gridCols; i++)
            {
                for(int j = 0; j < gridRows; j++)
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

                    int index = j * gridCols + i;
                    int id = numbers[index];
                    card.ChangeSprite(id, images[id]);

                    float posX = (offsetX * i) + startPos.x;
                    float posY = (offsetY * j) + startPos.y;
                    card.transform.position = new Vector3(posX, posY, startPos.z);
                }
            }
            gameState = GameState.GameStarted;

        }

        public virtual void GameFlow(){
            if (gameState > GameState.GameStarted)
            {
                x = 1;
                if (x != 1)
                {
                    gameState = GameState.GameFinished;
                }
            }

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
                        OnturnStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnturnStarted();
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

        protected void OnGameStarted()
        {

            gameState = GameState.TurnStarted;
        }

        protected void OnturnStarted()
        {
            SwitchTurn();
            gameState = GameState.TurnSelectingCards;
        }

        protected void OnTurnSelectingCards()
        {
            if (currentTurnPlayer == localPlayer)
            {

            }
            if (currentTurnPlayer.IsAI)
            {

            }
        }

        protected void OnCheckingPairs()
        {
            //CheckMatch()
        }

        protected void OnGameFinished()
        {

        }
        //-------------------------------------------------------------------------------------------------------------------------------------------

        private int[] ShuffleArray(int[] numbers)
        {
            int[] newArray = numbers.Clone() as int[];
            for(int i = 0; i < newArray.Length; i++)
            {
                int tmp = newArray[i];
                int r = Random.Range(i, newArray.Length);
                newArray[i] = newArray[r];
                newArray[r] = tmp;
            }
            return newArray;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------

        public void SwitchTurn(){
            if (currentTurnPlayer == null)
            {
                currentTurnPlayer = localPlayer;
                return;
            }

            if (currentTurnPlayer == localPlayer)
            {
                currentTurnPlayer = remotePlayer;
            }
            else
            {
                currentTurnPlayer = localPlayer;
            }
        }

        private MemoryCard _firstRevealed;
        private MemoryCard _secondRevealed;

        private int bot_score = 0;
        private int player_score = 0;
        [SerializeField] private TextMesh botScore;
        [SerializeField] private TextMesh playerScore;

        public bool canReveal
        {
            get { return _secondRevealed == null; }
        }

        public void CardRevealed(MemoryCard card)
        {
            if(_firstRevealed == null)
            {
                
                _firstRevealed = card;
                audioSource.PlayOneShot(pick, 1f);
            }
            else
            {
                _secondRevealed = card;
                StartCoroutine(CheckMatch());
                audioSource.PlayOneShot(pick, 1f);
            }
        }

        private IEnumerator CheckMatch()
        {
            if(_firstRevealed.id == _secondRevealed.id)
            {
                audioSource.PlayOneShot(collect, 1f);
                if (currentTurnPlayer == localPlayer)
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