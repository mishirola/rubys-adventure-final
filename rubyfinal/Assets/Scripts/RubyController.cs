using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip freezeSound;
    public ParticleSystem damagePrefab;
    public ParticleSystem healPrefab;
    public ParticleSystem icePrefab;
    public bool gameOver;
    public bool winSoundPlayed;
    //declares the public integer variable called robotsFixed
    static int robotsFixed = 0;
    public TextMeshProUGUI textField1, textField2;
    //public TextMeshProUGUI textField2;
    public int health { get { return currentHealth; } }
    int currentHealth;



    public float timeInvincible = 1.5f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    public float horizontal;
    public float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        //sets the variable robotsFixed equal to zero (not working)
        robotsFixed = 0;

        audioSource = GetComponent<AudioSource>();

        textField2.text = "";

        gameOver = false;
        winSoundPlayed = false;
    }


    // Update is called once per frame
    void Update()
    {
        textField1.text = "Robots Fixed:" + robotsFixed + "/3";
        //PowerupFast - Joan Mikel Amaya
        if (PowerupFast.powerupFast)
        {
            horizontal = 2 * Input.GetAxis("Horizontal");
            vertical = 2 * Input.GetAxis("Vertical");
        }
        else
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (invincibleTimer < 0.7)
        {
            speed = 3f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (PowerupShotgun.powerupShotgun)
            {
                StartCoroutine(LaunchShot());
            }
            else
            {
                Launch();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (Input.GetKey(KeyCode.R) && (gameOver))
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
        }

        if (robotsFixed >= 3)
        {
            textField2.text = "You Win! Press R to Restart";
            speed = 0f;
            gameOver = true;
            if (!winSoundPlayed)
            {
                PlaySound(winSound);
            }
            winSoundPlayed = true;
        }

        if (currentHealth < 1)
        {
            textField2.text = "You Lose! Press R to Restart";
            speed = 0f;
            gameOver = true;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    /*  each change health amount should have its own unique value
        for example: if the player takes (-1) health, it plays the normal damage fx
        if the player take (-2) though, it knows to play the freeze fx
        if you're adding specific fx for a new health collectable or damage item,
        you should use an unused value (ie. -4, -3, 2, 3)
    */
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        //This is where damage particles and heal particles are instantiated
        if (amount == -1)
        {
            ParticleSystem damageParticles = Instantiate(damagePrefab, rigidbody2d.position + Vector2.up * 0.65f, Quaternion.identity);
        }

        else if (amount > 0)
        {
            ParticleSystem healParticles = Instantiate(healPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        if (amount == -2)
        {
            speed = 0f;
            PlaySound(freezeSound);
            ParticleSystem healParticles = Instantiate(icePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    //declares a new function to be called by projectile script
    public void FixCountUp()
    {
        robotsFixed += 1;
        Debug.Log("Robots Fixed:" + robotsFixed + "/3");
        if (robotsFixed >= 3)
        {
            Debug.Log("All Robots Fixed:You win");
        }
    }

    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta), v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta));
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);

        //robotsFixed = robotsFixed + 1;
        //Debug.Log("cog thrown:" + robotsFixed + "/3");
    }
    //Part of the shotgun/burst Powerup
    IEnumerator LaunchShot()
    {
        Debug.Log("text");
        for (int i = 0; i < 3; i++)
        {
            GameObject projectileObject1 = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            //GameObject projectileObject2 = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity); 
            //GameObject projectileObject3 = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);  
            Debug.Log("text2");
            Projectile projectile1 = projectileObject1.GetComponent<Projectile>();
            //Projectile projectile2 = projectileObject2.GetComponent<Projectile>();
            //Projectile projectile3 = projectileObject3.GetComponent<Projectile>();

            projectile1.Launch(lookDirection, 300);
            //rojectile1.Launch(rotate(lookDirection, 45), 300);
            //projectile1.Launch(rotate(lookDirection, 90), 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);
            yield return new WaitForSeconds(0.12f);
        }
    }
}