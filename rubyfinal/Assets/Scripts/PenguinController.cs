using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    Animator animator;
    AudioSource audioSource;

    float timer;
    int direction = 1;
    bool sliding = false;
    bool slidePlayed = false;

    public AudioClip slideSound;

    //Penguin was done by Jaime Abrego
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        if (timer > 2.2f || timer < 0.8f) 
        {
            sliding = false;
            speed = 1.2f;
            animator.SetBool("Sliding", false);
            slidePlayed = false;
        }
        else
        {
            sliding = true;
            speed = 2.7f;
            animator.SetBool("Sliding", true);
            if (slidePlayed == false)
            {
                slidePlayed = true;
                PlaySoundF(slideSound);
            }
            
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);
    }
    //
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }

    public void PlaySoundF(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
