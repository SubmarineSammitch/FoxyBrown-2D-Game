using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private PlayerMovement playerMovement;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    //Gems
    public int m_Gems;
    public Text gemText;

    //Cherrys
    public GameObject healthImage_Low, healthImage_LowMid, healthImage_MidFull, healthImage_Full;
    public static int health;

    //Ladder
    public bool onLadder;
    public float climbSpeed;
    private float climbVelocity;
    private float gravityStore;
    public BoolEvent ClimbEvent;


    private void Awake()
    {
        startingHealth();

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        gravityStore = m_Rigidbody2D.gravityScale;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        if (ClimbEvent == null)
            ClimbEvent = new BoolEvent();
    }

    void Update()
    {
        Debug.Log("Updated Health: " + health);

        if (health >= 4)
        {
            health = 4;
            healthMeter(health);
        }
        else
            healthMeter(health);
    }

    private void FixedUpdate()
    {
        
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround) && !onLadder)
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }


        if (onLadder)
        {
            
            m_Rigidbody2D.gravityScale = 0f;

            climbVelocity = climbSpeed * Input.GetAxisRaw("Vertical");

            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, climbVelocity);
            ClimbEvent.Invoke(true);
            playerMovement.OnClimbing(true);

        }
        if (!onLadder)
        {
            m_Rigidbody2D.gravityScale = gravityStore;
            ClimbEvent.Invoke(false);
            playerMovement.OnClimbing(false);
        }


    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void AddGems(int numberOfGems)
    {
        m_Gems += numberOfGems;
        gemText = GameObject.Find("gemText").GetComponent<Text>();
        gemText.text = "Gems: " + m_Gems;
    }

    public void takeHealth(int healthtaken)
    {
        health -= healthtaken;
    }
    public void gainHealth(int healthGained)
    {
        health += healthGained;
    }

    public void startingHealth()
    {
        health = 4;

        healthImage_Low = GameObject.Find("healthImage_Low");
        healthImage_LowMid = GameObject.Find("healthImage_LowMid");
        healthImage_MidFull = GameObject.Find("healthImage_MidFull");
        healthImage_Full = GameObject.Find("healthImage_Full");

        healthImage_Low.gameObject.SetActive(true);
        healthImage_LowMid.gameObject.SetActive(true);
        healthImage_MidFull.gameObject.SetActive(true);
        healthImage_Full.gameObject.SetActive(true);
        
    }

    public void healthMeter(int health)
    {

        if (health >= 0 && health <= 4)
        {
            switch (health)
            {

                case 4:
                    healthImage_Low.gameObject.SetActive(true);
                    healthImage_LowMid.gameObject.SetActive(true);
                    healthImage_MidFull.gameObject.SetActive(true);
                    healthImage_Full.gameObject.SetActive(true);
                    break;
                case 3:
                    healthImage_Low.gameObject.SetActive(true);
                    healthImage_LowMid.gameObject.SetActive(true);
                    healthImage_MidFull.gameObject.SetActive(true);
                    healthImage_Full.gameObject.SetActive(false);
                    break;
                case 2:
                    healthImage_Low.gameObject.SetActive(true);
                    healthImage_LowMid.gameObject.SetActive(true);
                    healthImage_MidFull.gameObject.SetActive(false);
                    healthImage_Full.gameObject.SetActive(false);
                    break;
                case 1:
                    healthImage_Low.gameObject.SetActive(true);
                    healthImage_LowMid.gameObject.SetActive(false);
                    healthImage_MidFull.gameObject.SetActive(false);
                    healthImage_Full.gameObject.SetActive(false);
                    break;
                case 0:
                    healthImage_Low.gameObject.SetActive(false);
                    healthImage_LowMid.gameObject.SetActive(false);
                    healthImage_MidFull.gameObject.SetActive(false);
                    healthImage_Full.gameObject.SetActive(false);
                    break;

            }
        }
        else if (health > 4)
        {
            health = 4;
        }
        else
        {
            healthImage_Low.gameObject.SetActive(false);
            healthImage_LowMid.gameObject.SetActive(false);
            healthImage_MidFull.gameObject.SetActive(false);
            healthImage_Full.gameObject.SetActive(false);
            Debug.Log("Game Over");
        }
    }
}