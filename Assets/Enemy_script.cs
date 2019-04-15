using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class Enemy_script : MonoBehaviour {

    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


        void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            //this.gameObject.SetActive(false);
            CharacterController2D.health -= 1;
        }
        Debug.Log("After Enemy Health: " + CharacterController2D.health);
    }
}
