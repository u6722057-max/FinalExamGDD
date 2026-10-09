using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup
    [SerializeField] private float turboStrength = 15f;
    private ParticleSystem turboParticles;
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        // The supplied smoke effect is used for the turbo boost.  Parenting it
        // here keeps it aligned with the camera/focal point during play.
        GameObject smokeObject = GameObject.Find("Smoke_Particle");
        if (smokeObject != null)
        {
            smokeObject.transform.SetParent(focalPoint.transform);
            smokeObject.transform.localPosition = Vector3.zero;
            turboParticles = smokeObject.GetComponent<ParticleSystem>();
            turboParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) verticalInput = 1f;
            else if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) verticalInput = -1f;
        }
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            playerRb.AddForce(focalPoint.transform.forward * turboStrength, ForceMode.Impulse);
            if (turboParticles != null)
            {
                turboParticles.Play();
            }
        }

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);

    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }



}
