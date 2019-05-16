using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    public CharacterController2D CharControl;
    public int m_GemValue;

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
            CharControl.AddGems(m_GemValue);
            //other.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }

}
