//Gestione del player

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //gestione delle animazioni
    public Animation DoorOpen; //animazione apertura porta

    // caratteristiche del player
    public float speed = 7.5f; // velocità
    public float jumpSpeed = 8.0f; // velocità salto
    public float gravity = 20.0f; // gravità
    public Camera playerCamera; // Main Camera
    public float lookSpeed = 2.0f; // velocità visuale
    public float lookXLimit = 60.0f; // limite visuale
    public Texture2D cursorTexture; // Texture per il puntino rosso
    public float cursorSize = 16f; // Dimensione del puntino rosso
    public float maxHealth = 100; // vita massima
    public float currentHealth; // vita corrente durante il gameplay

    // controllo dello stato dei powerup
    public bool isInvincible;
    public bool isInvisible;

    private bool isHealing; // controllo se il player si sta curando
    private bool isMoving = false; // controllo se il player si sta muovendo

    //gestione movimento del player
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    //gestione delle cure
    private float timeForHealing = 0.2f;
    //water
    public int waterCounter = 0;
    private float waterHeal = 20f;
    //food
    public int medikitCounter = 0;
    private float foodHeal = 40f;

    [HideInInspector]
    public bool canMove = true; 

    //score del giocatore
    private PointsSystem points;
    private GameObject pointsObject;

    //gestione dei vari oggetti comprabili (power up e zone)
    private bool plusHealthCanBuy = true;
    private bool PlusSpeedCanBuy = true;
    public bool FastReloadingCanBuy = true;
    public bool PlusDamageCanBuy = true;
    public bool DoorOpenCanBuy = true;
    public bool ElicopterCanBuy = true;

    //Costo dei vari powerup
    private int plusHealthCost = 2500;
    private int PlusSpeedCost = 2000;
    private int FastReloadingCost = 3000;
    private int PlusDamageCost = 5000;
    private int DoorOpen1Cost = 2000;
    private int ElicopterCost = 10000;

    //Rocket shop
    public bool rocketCanBuy = true;
    private int rocketCost = 5000;
    private int rocketAmmoCost = 2000;
    public static event Action OnRocketAmmoBought;

    //Shotgun shop
    public bool ShotgunCanBuy = true;
    private int ShotgunCost = 750;
    private int ShotgunAmmoCost = 300;
    public static event Action OnShotgunAmmoBought;

    //AK47 shop
    public bool aK47CanBuy = true;
    private int aK47Cost = 3000;
    private int aK47AmmoCost = 1000;
    public static event Action OnAK47AmmoBought;

    //SMG Shop
    public bool SMGCanBuy = true;
    private int SMGCost = 1500;
    private int SMGAmmoCost = 600;
    public static event Action OnSMGAmmoBought;

    //Pistol shop
    public bool PistolCanBuy = false;
    private int PistolCost = 300;
    private int PistolAmmoCost = 100;
    public static event Action OnPistolAmmoBought;

    //Change weapons image
    [SerializeField] private Image gun1;
    [SerializeField] private Image gun2;
    [SerializeField] private Image gun3;
    [SerializeField] private Sprite aK47Sprite;
    [SerializeField] private Sprite shotgunSprite;
    [SerializeField] private Sprite pistolSprite;
    [SerializeField] private Sprite smgSprite;
    [SerializeField] private Sprite rocketSprite;
    private Color imageColor;

    //gestione dei round
    public int round = 1;
    public int round2 = 0;
    public int round3 = 0;
    public int currentZone = 1; //zona corrente per gestire i vari spawn

    //ui 
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text instructionText;
    public TMP_Text roundText;

    [SerializeField] private Image invincibility;
    [SerializeField] private Image invisibility;

    //PermaUPs
    [SerializeField] private Image plusHealth;
    [SerializeField] private Image fastReloading;
    [SerializeField] private Image plusSpeed;
    [SerializeField] private Image plusDamage;

    public TMP_Text waterNumText;
    public TMP_Text foodNumText;

    //timer per comprare ammo
    private float refreshTime = 0.2f;
    private bool canBuy = true;

    private GameObject weaponObject;
    private WeaponSwitch weapons;

    private AudioManager audioManager;

    void Start()
    {
        Time.timeScale = 1f;
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("EquipWeapon");
        imageColor = gun1.color;
        imageColor.a = 1f;

        PistolCanBuy = false;

        plusDamage.enabled = false;
        plusHealth.enabled = false;
        fastReloading.enabled = false;
        plusSpeed.enabled = false;

        currentHealth = maxHealth;
        healthText.text = $"{currentHealth}";
        instructionText.enabled = false;

        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;

        weaponObject = GameObject.FindGameObjectWithTag("Weapons");
        weapons = weaponObject.GetComponent<WeaponSwitch>();

        pointsObject = GameObject.FindGameObjectWithTag("Points");
        points = pointsObject.GetComponent<PointsSystem>();

        // Nascondi il cursore del mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        points.AddPoints(100000);
    }

    void Update()
    {
        this.gameObject.tag = "Player";

        //movimento della camera e del player
        if (characterController.isGrounded)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = speed * Input.GetAxis("Vertical");
            float curSpeedY = speed * Input.GetAxis("Horizontal");
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (curSpeedX != 0 || curSpeedY != 0)
            {
                if (!isMoving)
                {
                    audioManager.Play("PlayerFootStep");
                    isMoving = true;
                }
            }
            else
            {
                audioManager.Stop("PlayerFootStep");
                isMoving = false;
            }
            if (Input.GetButton("Jump"))
            {
                audioManager.Play("PlayerJump");
                moveDirection.y = jumpSpeed;
            }


        }

        moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
                rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            }
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }

        //utilizzo dell'acqua
        if (Input.GetKey(KeyCode.Q) && waterCounter > 0 && currentHealth < maxHealth)
        {
            UseWater();
        }

        //utilizzo del medikit
        if (Input.GetKey(KeyCode.F) && medikitCounter > 0 && currentHealth < maxHealth)
        {
            UseMedikit();
        }

        //controllo se i power up sono attivi
        if (isInvisible)
        {
            invisibility.enabled = true;
        }
        else
        {
            invisibility.enabled = false;
        }

        if (isInvincible)
        {
            invincibility.enabled = true;
        }
        else
        {
            invincibility.enabled = false;
        }
    }

    //metodo per l'uso dell'acqua
    public void UseWater()
    {
        if (isHealing == false)
        {
            audioManager.Play("DrinkWater");
            isHealing = true;
            waterCounter--;
            Heal(waterHeal);
            waterNumText.text = waterCounter.ToString();
            Invoke("GetHealth", timeForHealing);
        }

    }

    //metodo per l'uso dell'acqua
    public void UseMedikit()
    {
        if (isHealing == false)
        {
            audioManager.Play("Bandage");
            isHealing = true;
            medikitCounter--;
            Heal(foodHeal);
            foodNumText.text = medikitCounter.ToString();
            Invoke("GetHealth", timeForHealing);
        }
    }

    public void GetHealth()
    {
        isHealing = false;
    }

    //eventi per i collezzionabili e per l'attivazione della terza zona dopo aver distrutto il muro
    private void OnEnable()
    {
        Collectable.OnWaterCollected += OnCollectableWaterCollected;
        Collectable.OnFoodCollected += OnCollectableFoodCollected;
        RocketProjectile.OnWallDestroyed += ActivateThirdZone;
    }

    private void OnDisable()
    {
        Collectable.OnWaterCollected -= OnCollectableWaterCollected;
        Collectable.OnFoodCollected -= OnCollectableFoodCollected;
        RocketProjectile.OnWallDestroyed -= ActivateThirdZone;
    }

    public void ActivateThirdZone()
    {
        currentZone = 3;
    }

    public void OnCollectableWaterCollected()
    {
        waterCounter++;
        waterNumText.text = waterCounter.ToString();
    }

    public void OnCollectableFoodCollected()
    {
        medikitCounter++;
        foodNumText.text = medikitCounter.ToString();
    }

    //danno subito
    public void TakeDamage(int damage)
    {
        if (isInvincible == false)
        {
            // Riduce la vita del giocatore
            currentHealth -= damage;
            healthText.text = $"{currentHealth}";
            if (currentHealth <= 0)
            {
                SceneManager.LoadScene("DeathUI");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            if (currentHealth <= 0)
            {
                AudioListener.pause = true;
            }
        }
    }

    //metodo per curarsi
    public void Heal(float amount)
    {
        // Aumenta la vita del giocatore
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthText.text = $"{currentHealth}";
    }

    //punto centrale
    void OnGUI()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Disegna il puntino rosso al centro dello schermo
            float centerX = Screen.width / 2;
            float centerY = Screen.height / 2;
            float halfSize = cursorSize / 2;
            GUI.DrawTexture(new Rect(centerX - halfSize, centerY - halfSize, cursorSize, cursorSize), cursorTexture);
        }

    }

    //gestione collisioni con i vari oggetti della mappa
    private void OnTriggerStay(Collider other)
    {
        //BUYABLE POWERUP
        if (other.CompareTag("PlusHealth"))
        {
            if (plusHealthCanBuy)
            {
                //Scritta prezzo powerup e tasto da premere
                instructionText.text = $"COST : {plusHealthCost}\n PRESS E TO BUY MORE HEALTH";
                instructionText.enabled = true;
                if (points.points >= plusHealthCost && Input.GetKey(KeyCode.E))
                {
                    audioManager.Play("PermaPowerUpPickUp");
                    points.RemovePoints(plusHealthCost);
                    plusHealthCanBuy = false;
                    maxHealth += 50;
                    currentHealth = maxHealth;
                    healthText.text = $"{currentHealth}";
                    plusHealth.enabled = true;
                }
            }

            else
            {
                instructionText.enabled = true;
                instructionText.text = $"YOU ALREADY HAVE IT";
            }
        }

        if (other.CompareTag("FastReloading"))
        {
            if (FastReloadingCanBuy)
            {
                //Scritta prezzo powerup
                instructionText.text = $"COST : {FastReloadingCost}\n PRESS E TO BUY FAST RELOADING";
                instructionText.enabled = true;
                if (points.points >= FastReloadingCost && Input.GetKey(KeyCode.E))
                {
                    audioManager.Play("PermaPowerUpPickUp");
                    points.RemovePoints(FastReloadingCost);
                    FastReloadingCanBuy = false;
                    fastReloading.enabled = true;
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.text = $"YOU ALREADY HAVE IT";
            }
        }

        if (other.CompareTag("PlusSpeed"))
        {
            if (PlusSpeedCanBuy)
            {
                //Scritta prezzo powerup
                instructionText.text = $"COST : {PlusSpeedCost}\n PRESS E TO BUY MORE SPEED";
                instructionText.enabled = true;
                if (points.points >= PlusSpeedCost && Input.GetKey(KeyCode.E))
                {
                    audioManager.Play("PermaPowerUpPickUp");
                    points.RemovePoints(PlusSpeedCost);
                    PlusSpeedCanBuy = false;
                    speed += 3.5f;
                    plusSpeed.enabled = true;
                }
            }

            else
            {
                instructionText.enabled = true;
                instructionText.text = $"YOU ALREADY HAVE IT";
            }
        }

        if (other.CompareTag("PlusDamage"))
        {
            if (PlusDamageCanBuy)
            {
                //Scritta prezzo powerup
                instructionText.text = $"COST : {PlusDamageCost}\n PRESS E TO BUY MORE DAMAGE";
                instructionText.enabled = true;
                if (points.points >= PlusDamageCost && Input.GetKey(KeyCode.E))
                {
                    audioManager.Play("PermaPowerUpPickUp");
                    points.RemovePoints(PlusDamageCost);
                    PlusDamageCanBuy = false;
                    plusDamage.enabled = true;
                }
            }

            else
            {
                instructionText.enabled = true;
                instructionText.text = $"YOU ALREADY HAVE IT";
            }
        }

        if (other.CompareTag("Door1"))
        {
            if (DoorOpenCanBuy)
            {
                instructionText.text = $"COST : {DoorOpen1Cost}\n PRESS E TO UNLOCK THE SECOND ZONE";
                instructionText.enabled = true;
                if (points.points >= DoorOpen1Cost && Input.GetKey(KeyCode.E))
                {
                    audioManager.Play("DoorOpen");
                    currentZone = 2;
                    DoorOpen.Play();
                    points.RemovePoints(DoorOpen1Cost);
                    DoorOpenCanBuy = false;
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.enabled = false;
            }
        }

 



        if (other.CompareTag("Elicopter"))
        {
            if (ElicopterCanBuy)
            {
                instructionText.text = $"COST : {ElicopterCost}\n PRESS E TO ESCAPE";
                instructionText.enabled = true;
                if (points.points >= ElicopterCost && Input.GetKey(KeyCode.E))
                {
                    points.RemovePoints(ElicopterCost);
                    ElicopterCanBuy = false;

                    AudioListener.pause = true;
                    SceneManager.LoadScene("WinUI");
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.enabled = false;
            }
        }

        if (other.CompareTag("ShotgunShop"))
        {
            if (ShotgunCanBuy)
            {
                if (weapons.selectedWeapon == 0 && weapons.gun2 != 7)
                {
                    instructionText.text = "YOU NEED TO HANDLE A GUN";
                    instructionText.enabled = true;
                }
                else
                {
                    instructionText.text = $"PRESS E TO BUY COST: {ShotgunCost}";
                    instructionText.enabled = true;
                    if (points.points >= ShotgunCost && Input.GetKey(KeyCode.E))
                    {
                        audioManager.Play("EquipWeapon");
                        points.RemovePoints(ShotgunCost);
                        ShotgunCanBuy = false;
                        if (weapons.gun2 == 7)
                        {
                            weapons.gun2 = 4;
                            weapons.selectedWeapon = 4;
                            gun2.sprite = shotgunSprite;
                            gun2.color = imageColor;
                            imageColor.a = 0.5f;
                            gun1.color = imageColor;
                            gun3.color = imageColor;
                            weapons.numAmmo.enabled = true;
                        }
                        else if (weapons.selectedWeapon == weapons.gun1)
                        {
                            if (weapons.gun1 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun1 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun1 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            if (weapons.gun1 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun1 = 4;
                            weapons.selectedWeapon = 4;
                            gun1.sprite = shotgunSprite;
                        }
                        else
                        {
                            if (weapons.gun2 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun2 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun2 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            if (weapons.gun2 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun2 = 4;
                            weapons.selectedWeapon = 4;
                            gun2.sprite = shotgunSprite;
                        }
                        canBuy = false;
                        Invoke("Refresh", refreshTime);
                    }
                }
            }

            else
            {
                instructionText.enabled = true;
                instructionText.text = $"PRESS E TO BUY AMMO COST: {ShotgunAmmoCost}";
                if (points.points >= ShotgunAmmoCost && Input.GetKey(KeyCode.E) && canBuy)
                {
                    points.RemovePoints(ShotgunAmmoCost);
                    OnShotgunAmmoBought?.Invoke();
                    canBuy = false;
                    Invoke("Refresh", refreshTime);
                }
            }
        }

        if (other.CompareTag("Ak47Shop"))
        {
            if (aK47CanBuy)
            {
                if (weapons.selectedWeapon == 0 && weapons.gun2 != 7)
                {
                    instructionText.text = "YOU NEED TO HANDLE A GUN";
                    instructionText.enabled = true;
                }
                else
                {
                    instructionText.text = $"PRESS E TO BUY COST: {aK47Cost}";
                    instructionText.enabled = true;
                    if (points.points >= aK47Cost && Input.GetKey(KeyCode.E))
                    {
                        audioManager.Play("EquipWeapon");
                        points.RemovePoints(aK47Cost);
                        aK47CanBuy = false;

                        if (weapons.gun2 == 7)
                        {
                            weapons.gun2 = 1;
                            weapons.selectedWeapon = 1;
                            gun2.sprite = aK47Sprite;
                            gun2.color = imageColor;
                            imageColor.a = 0.5f;
                            gun1.color = imageColor;
                            gun3.color = imageColor;
                            weapons.numAmmo.enabled = true;
                        }
                        else if (weapons.selectedWeapon == weapons.gun1)
                        {
                            if (weapons.gun1 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun1 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun1 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            if (weapons.gun1 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun1 = 1;
                            weapons.selectedWeapon = 1;
                            gun1.sprite = aK47Sprite;
                        }
                        else
                        {
                            if (weapons.gun2 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun2 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun2 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            if (weapons.gun2 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun2 = 1;
                            weapons.selectedWeapon = 1;
                            gun2.sprite = aK47Sprite;
                        }
                        canBuy = false;
                        Invoke("Refresh", refreshTime);
                    }
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.text = $"PRESS E TO BUY AMMO COST: {aK47AmmoCost}";
                if (points.points >= aK47AmmoCost && Input.GetKey(KeyCode.E) && canBuy)
                {
                    points.RemovePoints(aK47AmmoCost);
                    OnAK47AmmoBought?.Invoke();
                    canBuy = false;
                    Invoke("Refresh", refreshTime);
                }
            }
        }

        if (other.CompareTag("SMGShop"))
        {
            if (SMGCanBuy)
            {
                if (weapons.selectedWeapon == 0 && weapons.gun2 != 7)
                {
                    instructionText.text = "YOU NEED TO HANDLE A GUN";
                    instructionText.enabled = true;
                }
                else
                {
                    instructionText.text = $"PRESS E TO BUY COST: {SMGCost}";
                    instructionText.enabled = true;
                    if (points.points >= SMGCost && Input.GetKey(KeyCode.E))
                    {
                        audioManager.Play("EquipWeapon");
                        points.RemovePoints(SMGCost);
                        SMGCanBuy = false;

                        if (weapons.gun2 == 7)
                        {
                            weapons.gun2 = 5;
                            weapons.selectedWeapon = 5;
                            gun2.sprite = smgSprite;
                            gun2.color = imageColor;
                            imageColor.a = 0.5f;
                            gun1.color = imageColor;
                            gun3.color = imageColor;
                            weapons.numAmmo.enabled = true;
                        }
                        else if (weapons.selectedWeapon == weapons.gun1)
                        {
                            if (weapons.gun1 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun1 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun1 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            if (weapons.gun1 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun1 = 5;
                            weapons.selectedWeapon = 5;
                            gun1.sprite = smgSprite;
                        }
                        else
                        {
                            if (weapons.gun2 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun2 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun2 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            if (weapons.gun2 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun2 = 5;
                            weapons.selectedWeapon = 5;
                            gun2.sprite = smgSprite;
                        }
                        canBuy = false;
                        Invoke("Refresh", refreshTime);
                    }
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.text = $"PRESS E TO BUY AMMO COST: {SMGAmmoCost}";
                if (points.points >= SMGAmmoCost && Input.GetKey(KeyCode.E) && canBuy)
                {
                    points.RemovePoints(SMGAmmoCost);
                    OnSMGAmmoBought?.Invoke();
                    canBuy = false;
                    Invoke("Refresh", refreshTime);
                }
            }
        }

        if (other.CompareTag("PistolShop"))
        {
            if (PistolCanBuy)
            {
                if (weapons.selectedWeapon == 0 && weapons.gun2 != 7)
                {
                    instructionText.text = "YOU NEED TO HANDLE A GUN";
                    instructionText.enabled = true;
                }
                else
                {
                    instructionText.text = $"PRESS E TO BUY COST: {PistolCost}";
                    instructionText.enabled = true;
                    if (points.points >= PistolCost && Input.GetKey(KeyCode.E))
                    {
                        audioManager.Play("EquipWeapon");
                        points.RemovePoints(PistolCost);
                        PistolCanBuy = false;

                        if (weapons.gun2 == 7)
                        {
                            weapons.gun2 = 3;
                            weapons.selectedWeapon = 3;
                            gun2.sprite = pistolSprite;
                            gun2.color = imageColor;
                            imageColor.a = 0.5f;
                            gun1.color = imageColor;
                            gun3.color = imageColor;
                            weapons.numAmmo.enabled = true;
                        }
                        else if (weapons.selectedWeapon == weapons.gun1)
                        {
                            if (weapons.gun1 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun1 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun1 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun1 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun1 = 3;
                            weapons.selectedWeapon = 3;
                            gun1.sprite = pistolSprite;
                        }
                        else
                        {
                            if (weapons.gun2 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun2 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun2 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun2 == 6)
                            {
                                rocketCanBuy = true;
                            }
                            weapons.gun2 = 3;
                            weapons.selectedWeapon = 3;
                            gun2.sprite = pistolSprite;
                        }
                        canBuy = false;
                        Invoke("Refresh", refreshTime);
                    }
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.text = $"PRESS E TO BUY AMMO COST: {PistolAmmoCost}";
                if (points.points >= PistolAmmoCost && Input.GetKey(KeyCode.E) && canBuy)
                {
                    audioManager.Play("EquipWeapon");
                    points.RemovePoints(PistolAmmoCost);
                    OnPistolAmmoBought?.Invoke();
                    canBuy = false;
                    Invoke("Refresh", refreshTime);
                }
            }
        }

        if (other.CompareTag("RocketShop"))
        {
            if (rocketCanBuy)
            {
                if (weapons.selectedWeapon == 0 && weapons.gun2 != 7)
                {
                    instructionText.text = "YOU NEED TO HANDLE A GUN";
                    instructionText.enabled = true;
                }
                else
                {
                    instructionText.text = $"PRESS E TO BUY COST: {rocketCost}";
                    instructionText.enabled = true;
                    if (points.points >= rocketCost && Input.GetKey(KeyCode.E))
                    {
                        audioManager.Play("EquipWeapon");
                        points.RemovePoints(rocketCost);
                        rocketCanBuy = false;

                        if (weapons.gun2 == 7)
                        {
                            weapons.gun2 = 6;
                            weapons.selectedWeapon = 6;
                            gun2.sprite = rocketSprite;
                            gun2.color = imageColor;
                            imageColor.a = 0.5f;
                            gun1.color = imageColor;
                            gun3.color = imageColor;
                            weapons.numAmmo.enabled = true;
                        }
                        else if (weapons.selectedWeapon == weapons.gun1)
                        {
                            if (weapons.gun1 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun1 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun1 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun1 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            weapons.gun1 = 6;
                            weapons.selectedWeapon = 6;
                            gun1.sprite = rocketSprite;
                        }
                        else
                        {
                            if (weapons.gun2 == 4)
                            {
                                ShotgunCanBuy = true;
                            }
                            if (weapons.gun2 == 1)
                            {
                                aK47CanBuy = true;
                            }
                            if (weapons.gun2 == 5)
                            {
                                SMGCanBuy = true;
                            }
                            if (weapons.gun2 == 3)
                            {
                                PistolCanBuy = true;
                            }
                            weapons.gun2 = 6;
                            weapons.selectedWeapon = 6;
                            gun2.sprite = rocketSprite;
                        }
                        canBuy = false;
                        Invoke("Refresh", refreshTime);
                    }
                }
            }
            else
            {
                instructionText.enabled = true;
                instructionText.text = $"PRESS E TO BUY AMMO COST: {rocketAmmoCost}";
                if (points.points >= rocketAmmoCost && Input.GetKey(KeyCode.E) && canBuy)
                {
                    points.RemovePoints(rocketAmmoCost);
                    OnRocketAmmoBought?.Invoke();
                    canBuy = false;
                    Invoke("Refresh", refreshTime);
                }
            }
        }
    }

    private void Refresh()
    {
        canBuy = true;
    }

    private void OnTriggerExit(Collider other)
    {
        instructionText.enabled = false;
    }
}