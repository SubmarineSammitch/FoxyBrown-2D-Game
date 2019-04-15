using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryScript : MonoBehaviour
{
    public CharacterController2D CharControl;
    public int m_CherryValue;

    // Start is called before the first frame update
    void Start()
    {
        CharControl = FindObjectOfType<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Item collection
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            CharControl.AddGems(m_CherryValue);
            //other.gameObject.SetActive(false);
            this.gameObject.SetActive(false);

            CharacterController2D.health += 1;
        }
    }

}
