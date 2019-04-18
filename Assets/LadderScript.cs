using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    public float Speed = 5f;
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

        if (other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.S))
        {
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -Speed);
            Debug.Log("Going down the ladder");
        }
        else if (other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.W))
        {
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, Speed);
            Debug.Log("Going up the ladder");
        }
        else
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

    }
}
