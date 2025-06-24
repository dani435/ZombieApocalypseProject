using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class RandomUP : MonoBehaviour
{
    //gestione eventi rispetto al tipo di power up raccolto
    public static event Action OnMinigunCollected;
    public static event Action OnConverterCollected;

    public float duration; // durata dei powerup

    //controllo collisione con il player
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup(other));
        }
    }

    //attivazione di un power up casuale alla raccolta
    IEnumerator Pickup(Collider player)
    {
        FindObjectOfType<AudioManager>().Play("pickUpPowerUp");
        PlayerController powerUp = player.GetComponent<PlayerController>();
        int random = UnityEngine.Random.Range(0, 4);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;


        if (random == 0)
        {           
            powerUp.isInvisible = true;
            Debug.Log("Invisible");

            yield return new WaitForSeconds(duration);

            powerUp.isInvisible = false;
        }
        else if (random == 1)
        {            
            powerUp.isInvincible = true;
            Debug.Log("INVINCIBLE");

            yield return new WaitForSeconds(duration);

            powerUp.isInvincible = false;

        }
        else if (random == 2) 
        {
            Debug.Log("Minigun");
            OnMinigunCollected?.Invoke();
        }

        else if(random == 3)
        {
            Debug.Log("Converter");
            OnConverterCollected?.Invoke();
        }
        
        Destroy(gameObject);
    }
}
