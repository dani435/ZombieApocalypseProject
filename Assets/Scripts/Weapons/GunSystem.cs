//gestione delle caratteristiche delle varie armi
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
using System;


public class GunSystem : MonoBehaviour
{

    // Gun stats
    public int damage;
    public float timeBetweenShooting, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold; //bool che permette di sparare a raffica, se false si spara a colpo singolo
    private int bulletsLeft, bulletsShot;
    public int ammo;
    private int remainingAmmo;
    public int pickUpAmmo;
    public int buyableAmmo;
    public int hitPoints;

    // Bools
    private bool shooting, readyToShoot, reloading;
    
    //Shake camera (rocket)
     public float shakeDuration = 0.2f;
     public float shakeMagnitude = 0.1f;
     private CameraShake cameraShake;

    // Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public RaycastHit rayHit1;
    public RaycastHit rayHit2;
    public RaycastHit rayHit3;
    public RaycastHit rayHit4;
    public LayerMask whatIsEnemy;
    public LayerMask bulletHole;

    // Graphics
    public GameObject bulletHoleGraphic;
    public TextMeshProUGUI text;

    public TextMeshProUGUI reloadingText;

    private Animator gunAnimator;

    //Player
    private GameObject playerObject;
    private PlayerController player;

    //Implement powerup
    private bool fastReload = false;
    private bool plusDamage = false;

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
    public Rigidbody missile;
    public GameObject rocketUI;
    private Rigidbody clonedRocket;

    public bool shooted = false;

    public ParticleSystem muzle;

    private void Awake()
    {
        muzle.Stop();
        reloadingText.enabled = false;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        gunAnimator = GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();

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

        cameraShake = Camera.main.GetComponent<CameraShake>();

        text.SetText(bulletsLeft + " / " + ammo);

    }

    private void Start()
    {
        muzle.Stop();
    }


    private void Update()
    {
        //acquisto perma power up
        if (fastReload == false && player.FastReloadingCanBuy == false)
        {
            reloadTime = reloadTime / 2;
            fastReload = true;
        }
        if (plusDamage == false && player.PlusDamageCanBuy == false)
        {
            damage = damage * 2;
            hitPoints = hitPoints * 2;
            plusDamage = true;
        }

        if (reloading || Cursor.lockState == CursorLockMode.None)
        {
            gunAnimator.SetBool("IsShooting", false);
        }

        MyInput();

        // SetText

        text.SetText(bulletsLeft + " / " + ammo);
    }

    private void MyInput()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (allowButtonHold)
            {
                shooting = Input.GetKey(KeyCode.Mouse0);
            }
            else
            {
                shooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

           
            if (Input.GetKeyDown(KeyCode.Mouse0) && this.gameObject.CompareTag("Minigun"))
            {
                FindObjectOfType<AudioManager>().Play("MachineGunShoot");
            }
            else if (this.gameObject.CompareTag("Minigun") && !shooting)
            {

                FindObjectOfType<AudioManager>().Stop("MachineGunShoot");
            }



            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading && gameObject.activeSelf &&
                (this.gameObject.CompareTag("Shotgun") || this.gameObject.CompareTag("Pistol") || this.gameObject.CompareTag("AK47") || this.gameObject.CompareTag("SMG") || this.gameObject.CompareTag("Rocket")) && ammo > 0)
            {
                gunAnimator.SetBool("IsShooting", false);
                Reload();
            }
        }

        // Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        else
        {
            gunAnimator.SetBool("IsShooting", false);
        }
    }

    private void Shoot()
    {
        gunAnimator.SetBool("IsShooting", true);
        if (this.gameObject.CompareTag("Rocket"))
        {
            rocketUI.SetActive(false);
            Quaternion additionalRotation = Quaternion.Euler(0f, -2.5f, 0f);
            Quaternion rotation = fpsCam.transform.rotation;
            rotation = additionalRotation * rotation;
            shooted = true;

            // Shake the camera
            if (cameraShake != null)
            {
                cameraShake.Shake(shakeDuration, shakeMagnitude);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && this.gameObject.CompareTag("Pistol"))
        {
            FindObjectOfType<AudioManager>().Play("PistolShoot");
        }
        else if (bulletsLeft <= 0 || reloading)
        {
            FindObjectOfType<AudioManager>().Stop("PistolShoot");
        }
        if (Input.GetKey(KeyCode.Mouse0) && this.gameObject.CompareTag("AK47"))
        {
            FindObjectOfType<AudioManager>().Play("AK47Shoot");
        }
        else if (this.gameObject.CompareTag("AK47") && !shooting || bulletsLeft <= 0 || reloading)
        {
            FindObjectOfType<AudioManager>().Stop("AK47Shoot");
        }
        if (Input.GetKey(KeyCode.Mouse0) && this.gameObject.CompareTag("Shotgun") && readyToShoot)
        {
            FindObjectOfType<AudioManager>().Play("ShotGunShoot");
        }
        else if (bulletsLeft <= 0 || reloading)
        {
            FindObjectOfType<AudioManager>().Stop("ShotGunShoot");
        }
        if (Input.GetKey(KeyCode.Mouse0) && this.gameObject.CompareTag("SMG"))
        {
            FindObjectOfType<AudioManager>().Play("SMGShoot");
        }
        else if (this.gameObject.CompareTag("SMG") && !shooting || bulletsLeft <= 0 || reloading)
        {
            FindObjectOfType<AudioManager>().Stop("SMGShoot");
        }


        readyToShoot = false;
        if (this.gameObject.CompareTag("Rocket"))
        {
            rocketUI.SetActive(false);
            Quaternion additionalRotation = Quaternion.Euler(0f, -2.5f, 0f);
            Quaternion rotation = fpsCam.transform.rotation;
            rotation = additionalRotation * rotation;
            clonedRocket = Instantiate(missile, attackPoint.position, rotation);
            shooted = true;
        }
        else
        {
            muzle.Play();

            // Calculate Direction
            Vector3 direction = fpsCam.transform.forward;

            //shotgun spray
            if (this.gameObject.CompareTag("Shotgun"))
            {
                // Raycast
                if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(0.2f, 0f, 0f), out rayHit, range, whatIsEnemy))
                {
                    if (rayHit.collider.CompareTag("Enemy") || rayHit.collider.CompareTag("Boss"))
                    {
                        EnemyIA enemy = rayHit.collider.GetComponent<EnemyIA>();
                        enemy.TakeDamage((int)damage, hitPoints);
                    }
                }

                else if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(0.2f, 0f, 0f), out rayHit, range, bulletHole))
                {
                    if (CheckHit())


                    {

                    }
                    else
                    {
                        GameObject buco = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
                        Destroy(buco, 3f);
                    }
                }

                // Raycast1
                if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(-0.2f, 0f, 0f), out rayHit1, range, whatIsEnemy))
                {
                    if (rayHit1.collider.CompareTag("Enemy") || rayHit1.collider.CompareTag("Boss"))
                    {
                        EnemyIA enemy = rayHit1.collider.GetComponent<EnemyIA>();
                        enemy.TakeDamage((int)damage, hitPoints);
                    }
                }
                else if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(-0.2f, 0f, 0f), out rayHit1, range, bulletHole))
                {
                    if (CheckHit())

                    {

                    }
                    else
                    {
                        GameObject buco = Instantiate(bulletHoleGraphic, rayHit1.point + rayHit1.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, rayHit1.normal));
                        Destroy(buco, 3f);
                    }
                }

                // Raycast2
                if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(0f, 0.2f, 0f), out rayHit2, range, whatIsEnemy))
                {
                    if (rayHit2.collider.CompareTag("Enemy") || rayHit2.collider.CompareTag("Boss"))
                    {
                        EnemyIA enemy = rayHit2.collider.GetComponent<EnemyIA>();
                        enemy.TakeDamage((int)damage, hitPoints);
                    }
                }
                else if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(0f, 0.2f, 0f), out rayHit2, range, bulletHole))
                {
                    if (CheckHit())

                    {

                    }
                    else
                    {
                        GameObject buco = Instantiate(bulletHoleGraphic, rayHit2.point + rayHit2.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, rayHit2.normal));
                        Destroy(buco, 3f);
                    }

                }

                // Raycast3
                if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(0f, -0.2f, 0f), out rayHit3, range, whatIsEnemy))
                {
                    if (rayHit3.collider.CompareTag("Enemy") || rayHit3.collider.CompareTag("Boss"))
                    {
                        EnemyIA enemy = rayHit3.collider.GetComponent<EnemyIA>();
                        enemy.TakeDamage((int)damage, hitPoints);
                    }
                }
                else if (Physics.Raycast(fpsCam.transform.position, direction + new Vector3(0f, -0.2f, 0f), out rayHit3, range, bulletHole))
                {
                    if (CheckHit())

                    {

                    }
                    else
                    {
                        GameObject buco = Instantiate(bulletHoleGraphic, rayHit3.point + rayHit3.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, rayHit3.normal));
                        Destroy(buco, 3f);
                    }
                }

                // Raycast4
                if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit4, range, whatIsEnemy))
                {
                    if (rayHit4.collider.CompareTag("Enemy") || rayHit4.collider.CompareTag("Boss"))
                    {
                        EnemyIA enemy = rayHit4.collider.GetComponent<EnemyIA>();
                        enemy.TakeDamage((int)damage, hitPoints);
                    }
                }
                else if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit4, range, bulletHole))
                {
                    if (CheckHit())

                    {

                    }
                    else
                    {
                        GameObject buco = Instantiate(bulletHoleGraphic, rayHit4.point + rayHit4.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, rayHit4.normal));
                        Destroy(buco, 3f);
                    }
                }
            }
            else
            {
                // Raycast
                if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
                {
                    if (rayHit.collider.CompareTag("Enemy") || rayHit.collider.CompareTag("Boss"))
                    {
                        EnemyIA enemy = rayHit.collider.GetComponent<EnemyIA>();
                        enemy.TakeDamage((int)damage, hitPoints);
                    }
                }
                else if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, bulletHole))
                {
                    if (CheckHit())
                    {

                    }
                    else
                    {
                        GameObject buco = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
                        Destroy(buco, 3f);
                    }
                }
            }
        }

        bulletsLeft -= bulletsShot;

        Invoke("ResetShot", timeBetweenShooting);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        gunAnimator.SetBool("IsShooting", false);
    }

    private void Reload()
    {
        reloadingText.enabled = true;
        reloading = true;
        gunAnimator.SetBool("IsShooting", false);
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        //annullo la ricarica al cambio arma
        if (!reloading)
        {
            return;
        }

        remainingAmmo = magazineSize - bulletsLeft;
        ammo -= remainingAmmo;
        if (ammo > 0)
        {
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsLeft = magazineSize + ammo;
            ammo = 0;
        }
        reloadingText.enabled = false;
        reloading = false;

        if (this.gameObject.CompareTag("Rocket"))
        {
            rocketUI.SetActive(true);
        }
    }

    private bool CheckHit()
    {
        bool noHole;
        noHole = rayHit.collider.CompareTag("Water") || rayHit.collider.CompareTag("Medikit") || rayHit.collider.CompareTag("AKAmmo") || rayHit.collider.CompareTag("PickUpObject")
                        || rayHit.collider.CompareTag("PlusHealth") || rayHit.collider.CompareTag("FastReloading") || rayHit.collider.CompareTag("PlusSpeed")
                        || rayHit.collider.CompareTag("PlusDamage") || rayHit.collider.CompareTag("Enemy") || rayHit.collider.CompareTag("PistolAmmo") || rayHit.collider.CompareTag("Boss")
                        || rayHit.collider.CompareTag("ShotgunAmmo") || rayHit.collider.CompareTag("SMGAmmo") || rayHit.collider.CompareTag("ShotgunShop")
                        || rayHit.collider.CompareTag("Ak47Shop") || rayHit.collider.CompareTag("SMGShop") || rayHit.collider.CompareTag("PistolShop")
                        || this.gameObject.CompareTag("Rocket") || rayHit.collider.CompareTag("RocketShop") || rayHit.collider.CompareTag("Ally") || rayHit.collider.CompareTag("Death")
                        || rayHit.collider.CompareTag("Elicopter") || rayHit.collider.CompareTag("Door1") || rayHit.collider.CompareTag("Door2") || rayHit.collider.CompareTag("RocketAmmo");
        return noHole;
    }

    //gestione eventi per l'aumento delle muzizioni delle varie armi
    private void OnDisable()
    {
        //annullo la ricarica al cambio arma
        reloading = false;
        reloadingText.enabled = false;
        gunAnimator.SetBool("IsShooting", false);
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
        muzle.Stop();
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
    public int GetBulletsLeft()
    {
        return bulletsLeft;
    }
}
