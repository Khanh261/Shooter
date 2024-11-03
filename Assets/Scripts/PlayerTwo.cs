using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class PlayerTwo : LivingEntity
{
    public float moveSpeed = 5;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    public float dashSpeed = 15f; 
    public float dashDuration = 0.2f; 
    private float dashTime;
    private bool isDashing; 

    protected override void Start()
    {
        base.Start();
    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void Update() 
    {
        // Regular movement input
        Vector3 moveInput = new Vector3(
            Input.GetAxisRaw("Horizontal2"),
            0,
            Input.GetAxisRaw("Vertical2")
        );

        if (Input.GetKeyDown(KeyCode.F) && !isDashing)
        {
            isDashing = true;
            dashTime = Time.time;
        }

        if (isDashing)
        {
            if (Time.time - dashTime < dashDuration) 
            {
                Vector3 dashVelocity = moveInput.normalized * dashSpeed; 
                controller.Move(dashVelocity); 
            }
            else
            {
                isDashing = false;
            }
        }
        else
        {
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);
        }

        // Aim in the direction of movement or default forward direction
        Vector3 aimDirection = moveInput != Vector3.zero ? moveInput : transform.forward;
        
        if(aimDirection != Vector3.zero)
        {
            controller.LookAt(transform.position + aimDirection);
        }
        //gunController.Aim(transform.position + aimDirection);

        // Weapon input
        if (Input.GetKey(KeyCode.M))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gunController.SwitchWeapon();
        }

        // Check if player falls out of bounds
        if (transform.position.y < -5) 
        {
            TakeDamage(health);
        }
    }

    public override void Die()
    {
        base.Die();
    }

    public void RestoreFullHealth()
    {
        health = startingHealth;
        Debug.Log("Health restored to full for: " + name); 
    }
}
