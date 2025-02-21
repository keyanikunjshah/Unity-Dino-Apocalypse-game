
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("OPlayer Health & Damage")]
    private int maxHealth = 100;
    public int currentHealth;
    //health slider
    //deathscreen
    [Header("Player Movement & Gravity")]
    public float MovementSpeed = 5f;
    private CharacterController controller;

    public float gravity = -9.81f; // Standard gravity value
    public Transform GroundCheck; // Reference to GroundCheck transform
    public LayerMask groundMask; // Mask for detecting ground
    public float groundDistance = 0.4f; // Distance to check for ground
    private bool isGrounded; // Is the player grounded
    private Vector3 velocity; // Velocity vector

    public float jumpForce = 2f; // Jump force value
    [Header("Foot Steps")]
    public AudioSource leftFootAudioSource;

    public AudioSource
    rightFootAudioSource;
    public AudioClip[] footstepSounds;
    public float footstepinterval = 0.5f;
    private float nextfootstepTime;

    public bool isLeftFootstep = true;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();

    }

    void Update()
    {
        // Get player position for custom ground check
        GameObject player = GameObject.Find("Player");
        Vector3 playerPosition = player.transform.position;
        Vector3 groundCheckPosition = new Vector3(playerPosition.x, playerPosition.y - 1.0f, playerPosition.z); // 1 unit below player

        // Debugging positions

        // Raycasting downwards to check for ground
        if (Physics.Raycast(groundCheckPosition, Vector3.down, out RaycastHit hit, 0.2f)) // Adjust the distance as needed

            // Checking if player is grounded using CheckSphere
            isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);

        // Reset downward velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Resetting the downward velocity to stick to ground
        }

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {

            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Jumping upwards
        }

        HandleMovement(); // Handling player movement
        HandleGravity();  // Applying gravity to the player

        //Handle Footstep
        if (controller.velocity.magnitude > 0.1f && Time.time > nextfootstepTime)
        {
            PlayerFoostepSound();
            nextfootstepTime = Time.time + footstepinterval;
        }

        // Apply movement
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;
        movement.y = 0; // Ensuring the player doesn't move up/down while walking
        controller.Move(movement * MovementSpeed * Time.deltaTime);
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime; // Adding gravity over time
    }

    void PlayerFoostepSound()
    {
        AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        if (isLeftFootstep)
        {
            leftFootAudioSource.PlayOneShot(footstepClip);
        }
        else
        {
            rightFootAudioSource.PlayOneShot(footstepClip);
        }

        isLeftFootstep = !isLeftFootstep;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    public void Die()
    {
        //deathscreen
        Debug.Log("Player has died");
    }
}
