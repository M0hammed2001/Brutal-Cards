using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BrutalCards{
        
    public class MemoryCard : MonoBehaviour {

        [SerializeField]  private SceneController controller;
        [SerializeField] private GameObject Card_Back;


        public void OnMouseDown()
        {
            if(Card_Back.activeSelf && controller.canReveal)
            {
                if(controller.currentTurnPlayer == controller.localPlayer)
                {
                    Card_Back.SetActive(false);
                    controller.CardRevealed(this);
                }  
                
                if(controller.currentTurnPlayer == controller.remotePlayer)
                {
                    controller.AiCardpick();
                    Debug.Log("here");
                }       

            } 
            
        }

        public void AiClicking(MemoryCard card)
        {
            Card_Back.SetActive(false);
            controller.CardRevealed(card);
            Debug.Log("here");
        }

        private int _id;
        public int id
        {
            get { return _id; }
        }

        public void ChangeSprite(int id, Sprite image)
        {
            _id = id;
            GetComponent<SpriteRenderer>().sprite = image; //This gets the sprite renderer component and changes the property of it's sprite!
        }

        public void Unreveal()
        {
            Card_Back.SetActive(true);
        }


    }
}