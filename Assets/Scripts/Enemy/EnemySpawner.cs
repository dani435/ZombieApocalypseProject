using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Prefab dell'enemy da spawnare
    public GameObject boss;
    public float spawnInterval = 3f; // Intervallo di spawn tra un enemy e l'altro
    private int maxEnemies; // Numero massimo di enemy che possono essere presenti contemporaneamente
    public Transform[] spawnPoints1; // Array di punti di spawn degli enemy
    public Transform[] spawnPoints2;
    public Transform[] spawnPoints3;
    private int randomZone;
    private Transform spawnPoint;


    private bool canSpawn = true;

    private int currentEnemiesKilled; // Numero di enemy attualmente presenti
    private int currentEnemiesRound;

    private int maxBoss;
    private int currentMaxBoss;
    private int currentBoss;
    private int currentBossKilled;

    //Player
    private GameObject playerObject;
    private PlayerController player;

    private void Start()
    {
       
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();

        player.roundText.text = $"ROUND: {player.round}";
        // Inizializza il conteggio degli enemy attuali a zero
        currentEnemiesKilled = 0;
        maxEnemies = 5;

        maxBoss = 0;
        currentBoss = 0;
        currentBossKilled = 0;
        currentMaxBoss = 0;
    }

    //gestione dei punti di spawn in base alle zone sbloccate
    private void Update()
    {

        if (canSpawn && currentBoss < currentMaxBoss)
        {
            if (player.currentZone == 2)
            {
                randomZone = Random.Range(0, 2);
            }
            else if (player.currentZone == 3)
            {
                randomZone = Random.Range(0, 3);
            }
            StartCoroutine(SpawnBoss());
        }

        if (canSpawn && currentEnemiesRound < maxEnemies)
        {
            if (player.currentZone == 2)
            {
                randomZone = Random.Range(0, 2);
            }
            else if (player.currentZone == 3)
            {
                randomZone = Random.Range(0, 3);
            }
            StartCoroutine(SpawnEnemie());
        }

        //controllo se tutti gli zombie del round sono morti e passo al round successivo
        if (currentEnemiesKilled >= maxEnemies && currentBossKilled >= currentMaxBoss)
        {
            player.round++;
            player.roundText.text = $"ROUND: {player.round}";
            maxEnemies += 5;
            currentEnemiesKilled = 0;
            currentEnemiesRound = 0;
            currentBoss = 0;
            currentBossKilled = 0;
            currentMaxBoss = 0;
            if (player.round % 4 == 0)
            {
                maxBoss++;
                currentMaxBoss = maxBoss;   
            }
        }
    }


    private IEnumerator SpawnBoss()
    {
        if (player.currentZone == 1)
        {
            spawnPoint = spawnPoints1[Random.Range(0, spawnPoints1.Length)];
            canSpawn = false;
            currentBoss++;
        }
        else if (player.currentZone == 2)
        {
            if (randomZone == 0)
            {
                spawnPoint = spawnPoints1[Random.Range(0, spawnPoints1.Length)];
            }
            else
            {
                spawnPoint = spawnPoints2[Random.Range(0, spawnPoints2.Length)];
            }
            canSpawn = false;
            currentBoss++;
        }
        else if (player.currentZone == 3)
        {
            if (randomZone == 0)
            {
                spawnPoint = spawnPoints1[Random.Range(0, spawnPoints1.Length)];
            }
            else if (randomZone == 1)
            {
                spawnPoint = spawnPoints2[Random.Range(0, spawnPoints2.Length)];
            }
            else
            {
                spawnPoint = spawnPoints3[Random.Range(0, spawnPoints3.Length)];
            }
            canSpawn = false;
            currentBoss++;
        }

        // Crea un nuovo enemy utilizzando il prefab scelto
        GameObject newBoss= Instantiate(boss, spawnPoint.position, spawnPoint.rotation);

        // Aspetta l'intervallo di spawn prima di eseguire un nuovo ciclo
        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }
 
    private IEnumerator SpawnEnemie()
    {

        // Se il numero di enemy attuali è inferiore al limite massimo
        // Scegli casualmente un punto di spawn
        if (player.currentZone == 1)
        {
            spawnPoint = spawnPoints1[Random.Range(0, spawnPoints1.Length)];
            canSpawn = false;
            currentEnemiesRound++;
        }
        else if(player.currentZone == 2)
        {
            if (randomZone == 0)
            {
                spawnPoint = spawnPoints1[Random.Range(0, spawnPoints1.Length)];
            }
            else
            {
                spawnPoint = spawnPoints2[Random.Range(0, spawnPoints2.Length)];
            }
            canSpawn = false;
            currentEnemiesRound++;
        }
        else if (player.currentZone == 3)
        {
            if (randomZone == 0)
            {
                spawnPoint = spawnPoints1[Random.Range(0, spawnPoints1.Length)];
            }
            else if (randomZone == 1)
            {
                spawnPoint = spawnPoints2[Random.Range(0, spawnPoints2.Length)];
            }
            else
            {
                spawnPoint = spawnPoints3[Random.Range(0, spawnPoints3.Length)];
            }
            canSpawn = false;
            currentEnemiesRound++;
        }

        // Scegli casualmente un prefab di enemy
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Crea un nuovo enemy utilizzando il prefab scelto
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Aspetta l'intervallo di spawn prima di eseguire un nuovo ciclo
        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }

    //gestione eventi morte dei nemici
    private void OnEnable()
    {
        EnemyIA.OnEnemyKilled += OnEnemyKill;
        EnemyIA.OnBossKilled += OnBossKill;
    }

    private void OnDisable()
    {
        EnemyIA.OnEnemyKilled -= OnEnemyKill;
        EnemyIA.OnBossKilled -= OnBossKill;
    }

    private void OnEnemyKill()
    {
        currentEnemiesKilled++;
    }

    private void OnBossKill()
    {
        currentBossKilled++;
    }
}
