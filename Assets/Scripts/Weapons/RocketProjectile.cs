using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public GameObject wallDestroyedPrefab;
    public GameObject explosionPrefab;
    private GameObject rocketObject;
    private GunSystem rocket;
    private GameObject enemyObject;
    private EnemyIA enemy;
    private float rocketSpeed = 30f;

    public static event Action OnWallDestroyed;

    private void Update()
    {
        transform.Translate(Vector3.forward * rocketSpeed * Time.deltaTime);
    }
    private void Start()
    {
        rocketObject = GameObject.FindGameObjectWithTag("Rocket");
        rocket = rocketObject.GetComponent<GunSystem>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Effettua l'esplosione
        Explode();
        FindObjectOfType<AudioManager>().Play("RocketExplosion");

        // Verifica se il proiettile ha colpito un muro
        if (collision.gameObject.CompareTag("Muro"))
        {
            FindObjectOfType<AudioManager>().Play("BreakWall");
            // Ottieni la posizione e la rotazione del muro intatto
            Vector3 position = collision.gameObject.transform.position;
            Quaternion rotation = collision.gameObject.transform.rotation;

            // Distruggi il muro intatto
            Destroy(collision.gameObject);

            // Instantia il muro spaccato nella stessa posizione e rotazione
            Instantiate(wallDestroyedPrefab, position, rotation);

            OnWallDestroyed?.Invoke();
        
        }
    }
    private void Explode()
    {
        // Crea l'effetto di esplosione
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        rocket.shooted = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 4f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy") || collider.gameObject.CompareTag("Boss"))
            {
                enemyObject = collider.gameObject;
                enemy = enemyObject.GetComponent<EnemyIA>();
                enemy.TakeDamage(350, 50);
            }
            
        }
        Destroy(gameObject);
    }
}

