using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{

    private CharacterController2D thePlayer;
    private PlayerMovement animationClimb;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = FindObjectOfType<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.name == "Player")
        {
            thePlayer.onLadder = true;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {

        if (other.name == "Player")
        {
            thePlayer.onLadder = false;
        }

    }
}
