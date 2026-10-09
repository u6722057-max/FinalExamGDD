using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class PlayerControllerX : MonoBehaviour
{
    public bool gameOver;
    public bool isLowEnough;

    public float floatForce = 22.0f;
    [SerializeField] private float gravityModifier = 1.5f;
    [SerializeField] private float upperLimit = 14.0f;
    [SerializeField] private float maxRiseSpeed = 4.0f;
    [SerializeField] private float maxFallSpeed = 6.0f;
    private Rigidbody playerRb;
    private bool floatInputHeld;

    public ParticleSystem explosionParticle;
    public ParticleSystem fireworksParticle;

    private AudioSource playerAudio;
    public AudioClip moneySound;
    public AudioClip explodeSound;
    public AudioClip bounceSound;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity = Vector3.down * 9.81f * gravityModifier;
        playerRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        playerAudio = GetComponent<AudioSource>();

        // The fireworks are a Player child and must use the balloon's position.
        if (fireworksParticle != null)
        {
            fireworksParticle.transform.localPosition = Vector3.zero;
        }

        // Give the balloon a gentle initial lift without launching it.
        playerRb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {
        floatInputHeld = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
    }

    private void FixedUpdate()
    {
        isLowEnough = transform.position.y < upperLimit;

        // Physics input belongs in FixedUpdate. Acceleration makes the control
        // independent of the balloon's mass and the frame rate.
        if (floatInputHeld && !gameOver && isLowEnough)
        {
            playerRb.AddForce(Vector3.up * floatForce, ForceMode.Acceleration);
        }

        Vector3 velocity = playerRb.linearVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxRiseSpeed);

        // Stop stored upward momentum at the ceiling so the balloon cannot
        // continue travelling above the camera after Space is released.
        if (!isLowEnough && velocity.y > 0.0f)
        {
            velocity.y = 0.0f;
        }

        playerRb.linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        // if player collides with bomb, explode and set gameOver to true
        if (other.gameObject.CompareTag("Bomb"))
        {
            explosionParticle.Play();
            playerAudio.PlayOneShot(explodeSound, 1.0f);
            gameOver = true;
            Debug.Log("Game Over!");
            Destroy(other.gameObject);
        } 

        // if player collides with money, fireworks
        else if (other.gameObject.CompareTag("Money"))
        {
            fireworksParticle.Play();
            playerAudio.PlayOneShot(moneySound, 1.0f);
            Destroy(other.gameObject);

        }

        // Play a bounce sound when the balloon touches the ground
        else if (other.gameObject.name.StartsWith("Ground"))
        {
            playerAudio.PlayOneShot(bounceSound, 1.0f);
        }

    }

}
