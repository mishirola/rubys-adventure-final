using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    //create variable to hold reference to Ruby game object
    public GameObject rupee;
    private float time;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void Update()
    {
        if (transform.position.magnitude > 1000.0f || time >= 1)
        {
            Destroy(gameObject);
            time = 0;
        }
        time += Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        //opens reference to Ruby Controller from Ruby Game Object
        RubyController player = rupee.GetComponent<RubyController>();
        if (e != null)
        {
            e.Fix();
            //Calls FixCountUp function in RubyController Script
            player.FixCountUp();
        }
        if (other.gameObject.name != "CogBullet(Clone)")
        {
            Destroy(gameObject);
        }
        //Destroy(gameObject);
    }
}