using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    //caratteristiche della spada
    public float attackDamage = 100f;
    public float attackRange = 1f;
    public Animator animator;
    public int hitPoints;

    public bool isAttacking = false;

    private GameObject ak47Object;
    private GunSystem ak47;

    private GameObject pistolObject;
    private GunSystem pistol;

    private GameObject shotgunObject;
    private GunSystem shotgun;

    private GameObject smgObject;
    private GunSystem smg;

    private GameObject rocketObject;
    private GunSystem rocket;

    private void Awake()
    {
        ak47Object = GameObject.FindGameObjectWithTag("AK47");
        ak47 = ak47Object.GetComponent<GunSystem>();

        pistolObject = GameObject.FindGameObjectWithTag("Pistol");
        pistol = pistolObject.GetComponent<GunSystem>();

        shotgunObject = GameObject.FindGameObjectWithTag("Shotgun");
        shotgun = shotgunObject.GetComponent<GunSystem>();

        smgObject = GameObject.FindGameObjectWithTag("SMG");
        smg = smgObject.GetComponent<GunSystem>();

        rocketObject = GameObject.FindGameObjectWithTag("Rocket");
        rocket = rocketObject.GetComponent<GunSystem>();
    }

    private void Update()
    {

        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        FindObjectOfType<AudioManager>().Play("SwordAttack");

        // Play attack animation
        animator.SetTrigger("Attack");

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

        // Perform attack hit detection
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                EnemyIA enemy = collider.GetComponent<EnemyIA>();
                enemy.TakeDamage((int)attackDamage, hitPoints);
            }
        }

        // Wait for a short delay after the attack
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

        // Finish the attack
        isAttacking = false;
        animator.ResetTrigger("Attack");
    }


    //gestione della raccolta delle varie munizioni
    private void OnDisable()
    {
        Collectable.OnAkAmmoCollected -= OnCollectableAKAmmoCollected;
        Collectable.OnPistolAmmoCollected -= OnCollectablePistolAmmoCollected;
        Collectable.OnShotgunAmmoCollected -= OnCollectableShotgunAmmoCollected;
        Collectable.OnSMGAmmoCollected -= OnCollectableSMGAmmoCollected;
        Collectable.OnRocketAmmoCollected -= OnCollectableRocketAmmoCollected;
        PlayerController.OnShotgunAmmoBought -= OnShotgunPlusAmmo;
        PlayerController.OnAK47AmmoBought -= OnAK47PlusAmmo;
        PlayerController.OnSMGAmmoBought -= OnSMGPlusAmmo;
        PlayerController.OnPistolAmmoBought -= OnPistolPlusAmmo;
        PlayerController.OnRocketAmmoBought -= OnRocketPlusAmmo;
    }

    private void OnEnable()
    {
        Collectable.OnAkAmmoCollected += OnCollectableAKAmmoCollected;
        Collectable.OnPistolAmmoCollected += OnCollectablePistolAmmoCollected;
        Collectable.OnShotgunAmmoCollected += OnCollectableShotgunAmmoCollected;
        Collectable.OnSMGAmmoCollected += OnCollectableSMGAmmoCollected;
        Collectable.OnRocketAmmoCollected += OnCollectableRocketAmmoCollected;
        PlayerController.OnShotgunAmmoBought += OnShotgunPlusAmmo;
        PlayerController.OnAK47AmmoBought += OnAK47PlusAmmo;
        PlayerController.OnSMGAmmoBought += OnSMGPlusAmmo;
        PlayerController.OnPistolAmmoBought += OnPistolPlusAmmo;
        PlayerController.OnRocketAmmoBought += OnRocketPlusAmmo;

    }

    public void OnCollectableAKAmmoCollected()
    {
        ak47.ammo += ak47.pickUpAmmo;
    }

    public void OnCollectablePistolAmmoCollected()
    {
        pistol.ammo += pistol.pickUpAmmo;
    }

    public void OnCollectableShotgunAmmoCollected()
    {
        shotgun.ammo += shotgun.pickUpAmmo;
    }

    public void OnCollectableSMGAmmoCollected()
    {
        smg.ammo += smg.pickUpAmmo;
    }

    public void OnCollectableRocketAmmoCollected()
    {
        rocket.ammo += rocket.pickUpAmmo;
    }

    public void OnShotgunPlusAmmo()
    {
        shotgun.ammo += shotgun.buyableAmmo;
    }

    public void OnAK47PlusAmmo()
    {
        ak47.ammo += ak47.buyableAmmo;
    }

    public void OnSMGPlusAmmo()
    {
        smg.ammo += smg.buyableAmmo;
    }

    public void OnPistolPlusAmmo()
    {
        pistol.ammo += pistol.buyableAmmo;
    }
    public void OnRocketPlusAmmo()
    {
        rocket.ammo += rocket.buyableAmmo;
    }


}
