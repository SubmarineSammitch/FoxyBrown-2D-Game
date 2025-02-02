using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
    public Animator animator;


	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;
    private bool climbing = false;
	
	// Update is called once per frame
	void Update ()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch") && !climbing)
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
        if (Input.GetButtonDown("Crouch") && climbing == true)
        {
            Debug.Log("PlayerMovement: If in Update");
            crouch = false;
            animator.SetBool("IsClimbing", true);
        }
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;

    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool IsCrouching)
    {
        animator.SetBool("IsCrouching", IsCrouching);
    }
    public void OnClimbing(bool IsClimbing)
    {
        climbing = IsClimbing;
        animator.SetBool("IsClimbing", IsClimbing);
    }

}
