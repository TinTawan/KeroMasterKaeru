using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioManager am; 
    //reference to cinemachine camera gameobjects
    [SerializeField] private GameObject thirdPCam;
    [SerializeField] private GameObject firstPCam;
    LevelLoader loadLevel;

    //check if in third person or not
    [HideInInspector] public bool thirdCamActive;

    //call character controller 
    public CharacterController controller;
    //reference camera 
    public Transform cam;
    [Header("Player Movement")]
    //define value for speed 
    public float speed = 6f;
    //define value for speed of rotation
    public float turnSmoothTime = 0.1f;
    //variable that stores rotation velocity
    float turnSmoothVelocity;


    [SerializeField] private KeyCode switchPers = KeyCode.Mouse1;

    [Header("Player Gravity and Jump")]
    //player gravity
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private float gravity = -19.62f;
    private bool grounded;
    private Vector3 playerVel;
    private float jumpHeight = 1.0f;
    private float originalSpeed;

    [Header("Charge Jump/Normal Jump")]
    [SerializeField] private float chargeSpeed;
    [SerializeField] public float chargePower;
    [SerializeField] public float MaxchargePower;
    [SerializeField] private bool isCharging;
    [SerializeField] private float slowDown;
    private bool canDoubleJump = false;
    private bool doubleJumpPower;
    [SerializeField] private float jumpSpeed = 7f;
    [SerializeField] private float jumpSpeedMultiplier = 1.2f;
    private bool justJumped;
    [SerializeField] AudioSource chargeJumpSound;



    [Header("Sprint")]
    //slider value from 0-1 for sprint value multiplier
    [SerializeField] [Range(0f, 1f)] float sprintMult;

    [Header("Water and Health")]
    [SerializeField] private float reduceHealthTime = 5f;
    [SerializeField] private float addHealthTime = 1f;
    [SerializeField] private GameObject splashEffect;

    private float healthTimer = 0f;
    public bool inWater;

    float timeSinceStep, timeBetweenSteps = .5f;

    private Animator anim;

    [Header("Particles")]
    [SerializeField] private GameObject jumpEffect;
    [SerializeField] private GameObject landEffect;



    private void Start()
    {
        thirdCamActive = true;
        thirdPCam.SetActive(true);
        firstPCam.SetActive(false);

        originalSpeed = speed;

        anim = GetComponentInChildren<Animator>();
        loadLevel = GameObject.FindObjectOfType<LevelLoader>();
        chargeJumpSound.enabled = false;
        
    }

    private void Update()
    {
        Cheats();
        //set grounded bool for when the ground check is touching the ground
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayers);

        // Checks if the player is on the ground and its y is still decreasing
        if (grounded && playerVel.y < 0)
        {
            //set the y velocity less than 0 to keep the player on the ground when moving around
            playerVel.y = -0.75f;
            anim.SetBool("jumping", false);

            if (justJumped)
            {
                justJumped = false;
                Instantiate(landEffect, groundCheck.transform.position, landEffect.transform.rotation);

            }
        }

        // Switching between first and 3rd person view
        SwitchCam();

        if (thirdCamActive)
        {
            ThirdPersonMove();
        }
        else
        {
            FirstPersonMove();
        }

        //call ChargeJump and pass in grounded bool
        ChargeJump(grounded);



        //increment players y velocity with gravity every frame
        //aka gravity logic
        playerVel = MoveDownSlope(playerVel);
        playerVel.y += gravity * Time.deltaTime;
        
        //move the character controller with the velocity * time, changing it every frame
        controller.Move(playerVel * Time.deltaTime);


        WaterHealthSection();

        //print("Has power up:" + doubleJumpPower);

        //play ribbit sound when player presses R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            Ribbit();
        }

        RunSound();


        //set moving bool for anim depending on if player is moving
        if (grounded)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                anim.SetBool("moving", true);
            }
            else if (Input.GetAxis("Horizontal") == 0f || Input.GetAxis("Vertical") == 0f)
            {
                anim.SetBool("moving", false);
            }

            
        }


    }

    // Charge jump logic
    void ChargeJump(bool isGrounded)
    {
        //if player is grounded, perform charge jump
        if (isGrounded)
        {
            // Checks if the charge key is preesed and if the power is less than the max charge
            if (Input.GetKey(KeyCode.E) && (chargePower < MaxchargePower)) 
            {
                
                isCharging = true; // Shows that the jump is being charged
                if (isCharging)
                {
                    chargePower += Time.deltaTime * chargeSpeed; // This is how the charge happens 
                                                                 // Charge speed is used to increase the rate at which the jump chrages up
                    chargeJumpSound.enabled = true;

                }
            }// if the charge button is pressed and held
            else if (!Input.GetKey(KeyCode.E))
            {
                // resets charge 
                chargePower = 0f;
                isCharging = false;
                chargeJumpSound.enabled = false;
                // Then checks if the space is pressed down instead of leaving it
                //perform normal jump
                if (Input.GetButtonDown("Jump") && (chargePower < MaxchargePower))
                {  
                    playerVel.y = jumpSpeed;
                    canDoubleJump = true;

                    am.StartAudio(AudioManager.Sound.jump, transform.position, .04f);
                    anim.SetBool("jumping", true);

                    Instantiate(jumpEffect, groundCheck.transform.position, jumpEffect.transform.rotation);
                    justJumped = true;

                }

            }
            //if release space before full charge
            // This is used for the charged jump also checks if the charge power is maxed out
            if (Input.GetButtonDown("Jump") && chargePower >= MaxchargePower)  
            {
                ReleaseCharge();
                am.StartAudio(AudioManager.Sound.jump, transform.position, .04f);
                anim.SetBool("jumping", true);

                justJumped = true;
            }

        } //if not grounded, it resets charge and doesnt perform charge jump
        else
        {
            isCharging = false;
            chargePower = 0f;
            chargeJumpSound.enabled = false;

            // Checks if the player has the double jump power up and allows a 2nd jump
            if (Input.GetButtonDown("Jump") && canDoubleJump == true && doubleJumpPower == true)
            {
                
                playerVel.y = jumpSpeed * jumpSpeedMultiplier; // takes the jump speed multiplies it and allows for abother jump 
                canDoubleJump = false;
                am.StartAudio(AudioManager.Sound.jump, transform.position, .08f);
            }
               
            
            
            
        }

    }


    // Method for the charged jump
    void ReleaseCharge()
    {
        playerVel.y += Mathf.Sqrt(-jumpHeight * chargePower * gravity); // Here it takes the charge time and increments the jumpheight
        isCharging = false; // Now the jump isnt charging 
        chargePower = 0f; // Resets charge time
        canDoubleJump = true;

        Instantiate(jumpEffect, groundCheck.transform.position, jumpEffect.transform.rotation);
    }



    void SwitchCam()
    {
        //THIRD PERSON
        //when letting go of the switch persective key
        if (Input.GetKeyUp(switchPers))
        {
            //set the main camera as a parent of the player
            cam.parent = null;

            Cursor.lockState = CursorLockMode.Confined;
            thirdPCam.SetActive(true);
            firstPCam.SetActive(false);
            thirdCamActive = !thirdCamActive;

            //set normal speed when 3rd person
            speed = originalSpeed;

            timeBetweenSteps = .6f;
        }

        //FIRST PERSON
        //when holding down the switch persective key
        if (Input.GetKeyDown(switchPers))
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            thirdPCam.SetActive(false);
            firstPCam.SetActive(true);
            thirdCamActive = !thirdCamActive;

            //reset then reduce speed when 1st person
            speed = originalSpeed;
            speed *= sprintMult;

            timeBetweenSteps = .7f;
        }
    }

    void ThirdPersonMove()
    {
        //fetch key presses (W & S for vertical axis, A & D for horizontal axis) and store them as direction
        //use GetAxisRaw to get 3 raw values (0. 1. 2), no increments, avoid smoothing 
        //use .normalized (keep values between 0 and 1) to account for 2 key being pressed at the same time and maintain consistent speeds in all directions
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //can only sprint in 3rd person
        SprintIncreaseSpeed();

        //check if movement happened. direction.magnitude = length of movement
        if (direction.magnitude >= 0.1f)
        {
            //calculate angle player is facing 
            //Atan2 calculates the radians between starting point (0 on x-axis) and new direction player is facing (x, y)
            //use Rad2Deg to convert radians to degrees
            //use cam.eulerAngles.y to reference camera y position so player moves forwards where camera is facing  
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //use SmoothDampAngle between positions to avoid snapping and make a fluid motion when changing direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //rotate player according to the angle found above
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //get direction by multiplying rotation with Vector3.forward
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            SlowDownSpeed();
            //tell controller to change position according to direction found above, add defined speed and make movement frame independent
            controller.Move(moveDir.normalized * speed * Time.deltaTime);


        }
    }

    void FirstPersonMove()
    {
        //inputs for movement
        float forward = Input.GetAxis("Vertical");
        float side = Input.GetAxis("Horizontal");

        //vector 3 movement with those inputs
        Vector3 movePlayer = transform.right * side + transform.forward * forward;
        SlowDownSpeed();

        //moving the player controller with a given speed
        controller.Move(movePlayer * speed * Time.deltaTime);
    }

    void SprintIncreaseSpeed()
    {
        //if holding down shift, multiply speed by sprint value
        //also set footstep time and running anim depending on if player is running
        if (Input.GetKeyDown(KeyCode.LeftShift) && thirdCamActive)
        {
            //plus 1 to make it increase not decrease
            speed *= (sprintMult + 1);
            timeBetweenSteps = .3f;

            anim.SetBool("running", true);

        }
        //if let go of shift, set speed back to normal
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = originalSpeed;
            timeBetweenSteps = .6f;

            anim.SetBool("running", false);

        }
    }

    // This method will slow down the movement speed when in first person mode
    private void SlowDownSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && thirdCamActive)
        {
           
            speed = originalSpeed * (sprintMult + 1);
            timeBetweenSteps = .3f;
            
            
        }
        else if (!isCharging && !thirdCamActive)
        {
            speed = originalSpeed * sprintMult;
            timeBetweenSteps = .7f;
        }
        else
        {
            speed = originalSpeed; // Changes the speed to normal
            timeBetweenSteps = .6f;
        }
    }


    void WaterHealthSection()
    {
        if (!inWater)
        {
            //when out of water
            //if player has health
            if (Manager.health > 0)
            {
                //timer increases until greater than time to reduce health
                healthTimer += Time.deltaTime;
                if (healthTimer >= reduceHealthTime)
                {
                    //then removes 1 health
                    Manager.AddHealth(-1f);
                    healthTimer = 0f;
                }
            }


        }
        else
        {
            //when in water
            //if health isn't max
            if (Manager.health < 100)
            {
                //timer increases until greater than time to replenish health
                healthTimer += Time.deltaTime;
                if (healthTimer >= addHealthTime)
                {
                    //adds 2 health
                    Manager.AddHealth(1f);
                    healthTimer = 0f;
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = true;

            //instantite splash effect at players position with rotation of the prefab
            Instantiate(splashEffect, transform.position, splashEffect.transform.rotation);

            am.StartAudio(AudioManager.Sound.splash, transform.position, .1f);

            anim.SetBool("swimming", true);
        }

        if (other.CompareTag("DoubleJumpPower"))
        {
            doubleJumpPower = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = false;

            anim.SetBool("swimming", false);

        }

        if (other.CompareTag("DoubleJumpPower"))
        {
            doubleJumpPower = false;
        }

    }


    //rotates the player to be facing the same way as the slope
    //allows player to jump while moving down a slope and stops bouncing down slopes
    private Vector3 MoveDownSlope(Vector3 velocity)
    {
        //cast ray down from player 
        var ray = new Ray(transform.position, Vector3.down);

        //use raycast to get information of the ground below at 0.5m so only when player is on the ground
        if(Physics.Raycast(ray, out RaycastHit hit, 0.5f))
        {
            //rotate current up direction to be the same as the grounds normal
            var slope = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var rotatedVelocity = slope * velocity;

            //if rotation is less than 0, return new rotation
            if(rotatedVelocity.y != 0)
            {
                return rotatedVelocity;
            }
        }

        //if no slope, return normal velocity
        return velocity;
    }
    
    //play ribbit sound
    void Ribbit()
    {
        am.StartAudio(AudioManager.Sound.ribbit, transform.position, .2f);
    }

    //run sound plays when moving in time with each footstep when walking or running
    void RunSound()
    {
        if((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && grounded)
        {
            timeSinceStep += Time.deltaTime;
            if(timeSinceStep > timeBetweenSteps)
            {
                timeSinceStep = 0f;
                am.StartAudio(AudioManager.Sound.walk, transform.position, .05f);
            }
            


        }
    }

    //cheats allow player to give themselves 10 flies, replenish health or restart cave 
    void Cheats()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Manager.AddFlies(10);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Manager.AddHealth(100 - Manager.health);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            loadLevel.LoadLevel(1);

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


    public void SetYVel(float inYVel)
    {
        playerVel.y = inYVel;
    }

    public void SetInWater(bool inBool)
    {
        inWater = inBool;
        
    }

    public void SetDoubleJump(bool doubleJump)
    {
        doubleJumpPower = doubleJump;
    }

    public bool ReturnInWater()
    {
        return inWater;
    }
    public bool ReturnThridPers()
    {
        return thirdCamActive;
    }
   public KeyCode ReturnSwitchPers()
    {
        return switchPers;
    }
   
    public float ReturnCharge()
    {
        return chargePower;
    }
    
    public float ReturnMaxCharge()
    {
        return MaxchargePower;
    }



}
