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
        // Movement input
        Vector3 moveInput = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );
        if (moveInput != Vector3.zero)
        {
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);
        }
        else
        {
            controller.Move(Vector3.zero);
        }

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin,point,Color.red);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);
            if (
                (
                    new Vector2(point.x, point.z)
                    - new Vector2(transform.position.x, transform.position.z)
                ).sqrMagnitude > 1
            )
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

        if (transform.position.y < -5) {
			TakeDamage (health);
		}
    }

    //player die
    public override void Die()
    {
        base.Die();
    }
}
