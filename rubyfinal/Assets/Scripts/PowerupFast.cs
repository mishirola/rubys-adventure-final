using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupFast : MonoBehaviour
{
    public AudioClip collectedClip;
    public static bool powerupFast = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            powerupFast = true;
            //Joan Mikel Amaya - Fast Powerup
            Destroy(gameObject);
            controller.PlaySound(collectedClip);

        }
    }
}