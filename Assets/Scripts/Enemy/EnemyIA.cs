using System;
using System.Collections;
using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using System.Linq;


public class EnemyIA : MonoBehaviour
{
    //animazioni e audio
    public AudioClip EnemyDeath;
    private AudioSource audioSource;
    private Animator zombieAnimator;

    public string playerTag; //tag del player

    //caratteristiche del nemico
    public float movementSpeed;
    public float maxHealth;
    public int damageAmount;
    private float hitTimer = 0f;
    public float maxHitTime;

    private GameObject playerObject;
    private Transform playerTransform;
    private NavMeshAgent agent;
    [HideInInspector] public float currentHealth;

    private PlayerController player;

    private bool IsChasingEnemy;

    private float standardConverterDuration = 10f;
    private float converterDuration;

    //gestione dei punti dati
    private PointsSystem points;
    private GameObject pointsObject;
    public int deathPoints;
    public int allyPoints;

    //loot
    public GameObject aKAmmo;
    public GameObject pistolAmmo;
    public GameObject medikit;
    public GameObject water;
    public GameObject powerUp;
    public GameObject shotgunAmmo;
    public GameObject smgAmmo;
    public GameObject rocketAmmo;

    //Ammo lootable
    private GameObject weaponObject;
    private WeaponSwitch weapons;

    private bool isDead = false;
    private bool isDisabled = false;

    private NavMeshObstacle obstacle;

    public static event Action OnEnemyKilled;
    public static event Action OnBossKilled;

    [SerializeField] private ParticleSystem blood;
    [SerializeField] private TMP_Text allyText;

    private bool invisible = false;

    private void Start()
    {
        audioSource= GetComponent<AudioSource>();    
        zombieAnimator = GetComponent<Animator>();

        blood.Stop();

        playerObject = GameObject.FindGameObjectWithTag(playerTag);
        playerTransform = playerObject.transform;
        player = playerObject.GetComponent<PlayerController>();

        //points system
        pointsObject = GameObject.FindGameObjectWithTag("Points");
        points = pointsObject.GetComponent<PointsSystem>();

        //Ammo lootable
        weaponObject = GameObject.FindGameObjectWithTag("Weapons");
        weapons = weaponObject.GetComponent<WeaponSwitch>();

        //gestione del potenziamento degli zombie dopo tot round
        if (player.round % 3 == 0)
        {
            player.round3 = player.round / 3;

        }

        if (player.round % 2 == 0)
        {
            player.round2 = player.round / 2;
        }

        if (player.round2 != 0)
        {
            maxHealth = maxHealth + (10 * player.round2);
        }

        if (player.round3 != 0)
        {
            movementSpeed = movementSpeed + (0.1f * player.round3);
            damageAmount = damageAmount + (5 * player.round3);
        }

        currentHealth = maxHealth;
        //navmesh
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        agent.speed = movementSpeed;

        converterDuration = standardConverterDuration;

        //se l'alleato sta seguendo il nemico
        IsChasingEnemy = false;
    }

    private void Update()
    {
        if (isDead || isDisabled)
            return;

        //se si tratta di un nemico o boss
        if (CompareTag("Enemy") || CompareTag("Boss"))
        {
            allyText.enabled = false;
            if (playerObject != null)
            {
                Vector3 playerPosition = playerTransform.position;

                if (CanSeePlayer(playerObject))
                {
                    zombieAnimator.SetBool("IsMoving", true);
                    invisible = false;
                    agent.isStopped = false;
                    agent.SetDestination(playerPosition);
                    obstacle.enabled = true;
                }
                else
                {
                    zombieAnimator.SetBool("IsMoving", false);
                    invisible = true;
                    agent.isStopped = true;
                    obstacle.enabled = false;
                    agent.SetDestination(transform.position);
                }
            }
        }
        //se e' un alleato impostare il target a un nemico
        else
        {
            allyText.enabled = true;
            agent.speed = movementSpeed * 2f;
            if (!IsChasingEnemy)
            {                
                converterDuration = standardConverterDuration;
                playerObject = GameObject.FindGameObjectWithTag("Enemy");
                IsChasingEnemy = true;

            }

            if (playerObject != null)
            {
                if (!playerObject.CompareTag("Enemy"))
                {
                    playerObject = GameObject.FindGameObjectWithTag("Enemy");
                }
            }

            if (playerObject == null)
            {
                if (GameObject.FindGameObjectWithTag("Enemy"))
                {
                    playerObject = GameObject.FindGameObjectWithTag("Enemy");
                }
                else
                {
                    OnEnemyKilled?.Invoke();
                    StartCoroutine(DestroyEnemy());
                }
            }

            else
            {
                playerTransform = playerObject.transform;
                Vector3 playerPosition = playerTransform.position;
                agent.SetDestination(playerPosition);
                agent.isStopped = false;

                converterDuration -= Time.deltaTime;
                if (converterDuration <= 0)
                {
                    playerObject = GameObject.FindGameObjectWithTag("Player");
                    playerTransform = playerObject.transform;
                    gameObject.tag = "Enemy";
                    agent.speed = 1;
                }
            }
        }
    }

    //controllo se il player non ha l'invisibilita'
    private bool CanSeePlayer(GameObject playerObject)
    {
        if (playerObject != null)
        {
            if (!player.isInvisible)
            {
                Renderer playerRenderer = playerObject.GetComponent<Renderer>();
                if (playerRenderer != null && playerRenderer.isVisible && isDead == false)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isDead || isDisabled || invisible)
            return;

        if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss"))
        {
            if (collision.gameObject.CompareTag(playerTag))
            {
                if (hitTimer <= 0)
                {
                    // Infligge danni al giocatore
                    PlayerController playerHealth = collision.gameObject.GetComponent<PlayerController>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damageAmount);
                        zombieAnimator.SetTrigger("Attack");
                    }
                    hitTimer = maxHitTime;
                }
                else
                {
                    hitTimer -= Time.deltaTime;
                }
            }
        }

        else if (gameObject.CompareTag("Ally"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                agent.speed = movementSpeed * 2f;
                EnemyIA enemy = collision.gameObject.GetComponent<EnemyIA>();
                if (hitTimer <= 0)
                {
                    // Infligge danni all'alleato
                    if (enemy.currentHealth >= 0)
                    {
                        enemy.TakeDamage(damageAmount * 5, allyPoints);
                        zombieAnimator.SetTrigger("Attack");
                    }
                    hitTimer = maxHitTime;
                }
                else
                {
                    hitTimer -= Time.deltaTime;
                }
            }
        }
    }

    //percentuali dei drop alla morte dello zombie
    private void DropLoot()
    {
        int random = UnityEngine.Random.Range(1, 101);
        Vector3 lootPosition = transform.position + new Vector3(0,1,0);
        if (random <= 15)
        {
            if (weapons.gun1 == 1 || weapons.gun2 == 1)
            {
                Instantiate(aKAmmo, lootPosition, Quaternion.identity);
            }
        }
        else if (random <= 30)
        {
            if (weapons.gun1 == 3 || weapons.gun2 == 3)
            {
                Instantiate(pistolAmmo, lootPosition, Quaternion.identity);
            }
        }
        else if (random <= 45)
        {
            if (weapons.gun1 == 4 || weapons.gun2 == 4)
            {
                Instantiate(shotgunAmmo, lootPosition, Quaternion.identity);
            }
        }
        else if (random <= 60)
        {
            if (weapons.gun1 == 5 || weapons.gun2 == 5)
            {
                Instantiate(smgAmmo, lootPosition, Quaternion.identity);
            }
        }
        else if (random <= 70)
        {
            if (weapons.gun1 == 6 || weapons.gun2 == 6)
            {
                Instantiate(rocketAmmo, lootPosition, Quaternion.identity);
            }
        }
        else if (random <= 85)
        {
            Instantiate(water, lootPosition, Quaternion.identity);
        }
        else if (random <= 94)
        {
            Instantiate(medikit, lootPosition, Quaternion.identity);
        }
        else if (random <= 100)
        {
            Instantiate(powerUp, lootPosition, Quaternion.identity);
        }
    }

    //danno al nemico
    public void TakeDamage(int damage, int hitPoints)
    {
        // Riduce la vita dell'IA nemica
        if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss") || gameObject.CompareTag("Death"))
        {
            currentHealth -= damage;

            blood.Play();

            points.AddPoints(hitPoints);
            if (currentHealth <= 0 && !isDead)
            {
                if (gameObject.CompareTag("Enemy"))
                {
                    OnEnemyKilled?.Invoke();
                }
                else if (gameObject.CompareTag("Boss"))
                {
                    OnBossKilled?.Invoke();
                }
                StartCoroutine(DestroyEnemy());
              //  audioSource.Play();
            }
        }
    }

    //morte del nemico
    IEnumerator DestroyEnemy()
    {
        if (isDead == false)
        {
            audioSource.clip = EnemyDeath;
            audioSource.loop= false;
            audioSource.Play();

            isDead = true;
            this.gameObject.tag = "Death";
            agent.isStopped = true;
            obstacle.enabled = false;
            agent.SetDestination(transform.position);

            DropLoot();
            points.AddPoints(deathPoints);
          
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            zombieAnimator.SetTrigger("Death");

            yield return new WaitForSeconds(4f);

            Destroy(gameObject);
        }

    }
}
