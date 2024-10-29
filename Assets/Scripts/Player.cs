using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;
    public Crosshair crosshair;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

        // Dashing skill variables
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
        Debug.Log("OnNewWave called with waveNumber: " + waveNumber); // Debug statement
    }

     void Update() 
    {
        // Regular movement input
        Vector3 moveInput = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
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

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }

        // Weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
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
            TakeDamage(health); // Causes the player to die if out of bounds
        }
    }

    //player die
    public override void Die()
    {
        base.Die();
    }
}
