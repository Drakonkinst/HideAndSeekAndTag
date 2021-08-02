/*
 * Name: Wesley Ho
 * ID: 2382489
 * Email: weho@chapman.edu
 * CPSC 236-02
 * Assignment: Final Project - Hide and Seek (and Tag)
 * This is my own work, and I did not cheat on this assignment.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Steerable
{
    private const int maxStaminaCooldown = 2 * 100; // The cooldown before stamina begins to regenerate, in hundreth of seconds
    private const int maxSprintingTime = 3 * 100; // The maximum time the player can sprint before regenerating, in hundreth of seconds
    private const float sprintingRechargePerTick = 0.5f; // How much sprinting time is regenerated each update, in hundred of seconds
    private const float timeBeforeLose = 0.4f; // Time in seconds
    private const float footstepVolume = 0.3f;
    private const float sprintingPitch = 1.5f;

    public RectTransform StaminaDisplay;
    public Text ScoreDisplay;
    public float WalkingSpeed = 4.0f;
    public float SprintingSpeed = 6.0f;
    public AudioClip CollectiblePickupSound;
    public Sprite SpotIdle;
    public Sprite SpotMoving1;
    public Sprite SpotMoving2;

    private bool isSprinting = false;
    private Rigidbody2D rb;
    private LoopingAudio footstepAudio;
    private float sprintingTime;
    private int staminaCooldown = 0;
    private float staminaBarWidth;
    private int score;
    private float collisionStart;

    public override void OnStart()
    {
        rb = GetComponent<Rigidbody2D>();
        footstepAudio = GetComponent<LoopingAudio>();
        sprintingTime = maxSprintingTime;
        staminaBarWidth = StaminaDisplay.parent.GetComponent<RectTransform>().sizeDelta.x;
        score = 0;
        InvokeRepeating("UpdateStamina", 0.0f, 0.01f);

        if(rb == null)
        {
            Debug.Log("RB is null");
        }
        if(steering == null)
        {
            Debug.Log("Steering is null");
        }

        Invoke("UpdateSprite", 0.0f);
    }

    public override void DoBehavior()
    {
        // Controller inputs: https://www.reddit.com/r/Unity3D/comments/1syswe/ps4_controller_map_for_unity/

        // Update sprinting behavior
        isSprinting = Input.GetAxis("Sprint") > 0;
        UpdateMaxVelocity();

        // Get velocity
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 velocityDir = (new Vector2(h, v)).normalized;

        // Update footstep audio
        if(velocityDir.sqrMagnitude > 0)
        {
            footstepAudio.SetVolume(footstepVolume);
        }
        else
        {
            footstepAudio.SetVolume(0.0f);
        }

        // Apply velocity to player
        rb.velocity = velocityDir * steering.MaxVelocity;
        SetRotation(steering.GetFacing());
    }

    private void UpdateSprite()
    {
        // I don't know how to do animations lmao
        const float updateSpeed = 0.3f;
        if (IsPlayerMoving())
        {
            if (sprite.sprite == SpotMoving1)
            {
                sprite.sprite = SpotMoving2;
            }
            else
            {
                sprite.sprite = SpotMoving1;
            }

            if (isSprinting && sprintingTime > 0)
            {
                Invoke("UpdateSprite", updateSpeed * (WalkingSpeed / SprintingSpeed));
            }
            else
            {
                Invoke("UpdateSprite", updateSpeed);
            }
        }
        else
        {
            sprite.sprite = SpotIdle;
            Invoke("UpdateSprite", updateSpeed);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Start timer
        if(collision.gameObject.tag == "Hunter")
        {
            // Lose game
            collisionStart = Time.time;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        // Ensure collision stay does not cause a loss
        if(collision.gameObject.tag == "Hunter")
        {
            collisionStart = Time.time;
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        // If collision for more than timeBeforeLose, lose game.
        if(collision.gameObject.tag == "Hunter")
        {
            float currTime = Time.time;
            if(currTime - collisionStart >= timeBeforeLose)
            {
                // Lose game
                manager.PlayerLose();
            }
        }
    }

    // Check for collectible trigger collision
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Collectible")
        {
            PickupCollectible(collision);
        }
    }

    private void PickupCollectible(Collider2D collision)
    {
        // Pick up collectible and increase score
        manager.RemoveCollectible(collision.gameObject.GetComponent<Collectible>());
        Destroy(collision.gameObject);
        ++score;
        UpdateScoreDisplay();
        SoundManager.Instance.Play(CollectiblePickupSound, myTransform, 0.2f);

        if (GameSettings.GameMode == GameSettings.Mode.COLLECTION)
        {
            if (score >= GameSettings.NumCollectibles)
            {
                manager.PlayerWin();
            }
        }
    }

    // Updated every 0.01s for smooth appearance
    // After sprinting, there is a short cooldown before stamina regenerates again
    private void UpdateStamina()
    {
        if(isSprinting && sprintingTime > 0 && IsPlayerMoving())
        {
            footstepAudio.SetPitch(sprintingPitch);
            sprintingTime--;
            staminaCooldown = maxStaminaCooldown;
        }
        else
        {
            footstepAudio.SetPitch(1.0f);
            if (staminaCooldown > 0)
            {
                staminaCooldown--;
            } else
            {
                if(sprintingTime < maxSprintingTime)
                {
                    sprintingTime += sprintingRechargePerTick;
                    if(sprintingTime > maxSprintingTime)
                    {
                        sprintingTime = maxSprintingTime;
                    }
                }
            }
        }
        UpdateStaminaDisplay();
    }

    private void UpdateStaminaDisplay()
    {
        StaminaDisplay.sizeDelta = new Vector2(staminaBarWidth * GetStaminaPercentage(), StaminaDisplay.sizeDelta.y);
    }

    private void UpdateScoreDisplay()
    {
        ScoreDisplay.text = "Coins: " + score;
    }

    // Ses maximum velocity of steering object based on sprinting status
    private void UpdateMaxVelocity()
    {
        if(isSprinting && sprintingTime > 0.0f)
        {
            steering.MaxVelocity = SprintingSpeed;
        }
        else
        {
            steering.MaxVelocity = WalkingSpeed;
        }
    }

    private float GetStaminaPercentage()
    {
        return sprintingTime / maxSprintingTime;
    }

    private bool IsPlayerMoving()
    {
        return steering.GetHostVelocity().sqrMagnitude > 0.0f;
    }

    public int GetScore()
    {
        return score;
    }
}
