//gestione della raccolta dei vari collectable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectable : MonoBehaviour
{
    //eventi richiamati alla raccolta
    public static event Action OnWaterCollected;
    public static event Action OnAkAmmoCollected;
    public static event Action OnFoodCollected;
    public static event Action OnPistolAmmoCollected;
    public static event Action OnShotgunAmmoCollected;
    public static event Action OnSMGAmmoCollected;
    public static event Action OnRocketAmmoCollected;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    void Update()
    {
        transform.localRotation = Quaternion.Euler(90f, Time.time * 100, 0);

        if (this.CompareTag("Water") || this.CompareTag("ShotgunUI") || this.CompareTag("AK47UI") || this.CompareTag("SMGUI") || this.CompareTag("PistolUI") || this.CompareTag("RocketUI"))
        {
            transform.localRotation = Quaternion.Euler(0f, Time.time * 100, 0);
        }

        if (this.CompareTag("SMGAmmo"))
        {
            transform.localRotation = Quaternion.Euler(-90f, Time.time * 100, 0);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.CompareTag("Water"))
            {
                audioManager.Play("DrinkWater");
                OnWaterCollected?.Invoke();
            }

            if (this.CompareTag("AKAmmo"))
            {
                audioManager.Play("PickUpAmmo");
                OnAkAmmoCollected?.Invoke();
            }

            if (this.CompareTag("PistolAmmo"))
            {
                audioManager.Play("PickUpAmmo");
                OnPistolAmmoCollected?.Invoke();
            }

            if (this.CompareTag("ShotgunAmmo"))
            {
                audioManager.Play("PickUpAmmo");
                OnShotgunAmmoCollected?.Invoke();
            }

            if (this.CompareTag("SMGAmmo"))
            {
                audioManager.Play("PickUpAmmo");
                OnSMGAmmoCollected?.Invoke();
            }

            if (this.CompareTag("RocketAmmo"))
            {
                audioManager.Play("PickUpAmmo");
                OnRocketAmmoCollected?.Invoke();
            }

            if (this.CompareTag("Medikit"))
            {
                audioManager.Play("Bandage");
                OnFoodCollected?.Invoke();
            }
            Destroy(gameObject);
        }
    }
}
