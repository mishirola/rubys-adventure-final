using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupShotgun : MonoBehaviour
{
    public AudioClip collectedClip;
    public static bool powerupShotgun = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            powerupShotgun = true;
            //Joan Mikel Amaya - Shotgun/Burst Powerup
            Destroy(gameObject);
            controller.PlaySound(collectedClip);

        }
    }
}