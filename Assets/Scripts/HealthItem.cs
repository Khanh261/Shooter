using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
     void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);

        if (other.CompareTag("Player")) 
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Player found: " + player.name); 
                player.RestoreFullHealth();
                Destroy(gameObject);  
            }
        }
    }
}
