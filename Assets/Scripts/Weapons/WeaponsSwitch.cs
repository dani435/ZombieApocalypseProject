//gestione armi nell'inventario
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour
{

    [SerializeField] private Transform[] weapons;

    [HideInInspector] public int selectedWeapon;
    private int previousSelectedWeapon;
    private int beforeMinugun;
    [HideInInspector] public bool equipMinigun;
    private float minigunDuration = 10f;
    private float minigunCurrentDuration;
    [HideInInspector] public int gun1;
    [HideInInspector] public int gun2;

    private SwordAttack katana;
    private GameObject katanaObject;

    [SerializeField] private Image minigun;

    //gunImage
    [SerializeField] private Image gun1Image;
    [SerializeField] private Image gun2Image;
    [SerializeField] private Image gun3Image;
    private Color imageColor;

    //ammo
    [SerializeField] public TextMeshProUGUI numAmmo;

    //attivazione layout arma equipaggiata e armi in possesso
    private void Start()
    {
        imageColor = gun1Image.color;

        gun1 = 3;
        gun2 = 7;
        selectedWeapon = gun1;
        previousSelectedWeapon = selectedWeapon;
        imageColor.a = 1f;
        gun1Image.color = imageColor;
        imageColor.a = 0.5f;
        gun3Image.color = imageColor;
        imageColor.a = 0f;
        gun2Image.color = imageColor;

        katanaObject = GameObject.FindGameObjectWithTag("Katana");
        katana = katanaObject.GetComponent<SwordAttack>();

        minigunCurrentDuration = minigunDuration;
        SetWeapons();
        Select(selectedWeapon);
    }

    private void SetWeapons()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            weapons[i] = transform.GetChild(i);
    }

    private void Update()
    {
    

        if (Input.GetKey(KeyCode.Alpha1))
        {

            numAmmo.enabled = true;
            katana.isAttacking = false;
            selectedWeapon = gun1;

            if (selectedWeapon != previousSelectedWeapon)
            {
                FindObjectOfType<AudioManager>().Play("EquipWeapon");

            }
            equipMinigun = false;
            minigunCurrentDuration = minigunDuration;
            imageColor.a = 1f;
            gun1Image.color = imageColor;
            imageColor.a = 0.5f;
            gun2Image.color = imageColor;
            gun3Image.color = imageColor;
            if (gun2 == 7)
            {
                imageColor.a = 0f;
                gun2Image.color = imageColor;
            }
        }

        if (Input.GetKey(KeyCode.Alpha2) && gun2 != 7)
        {
           
            numAmmo.enabled = true;
            katana.isAttacking = false;
            selectedWeapon = gun2;

            if (selectedWeapon != previousSelectedWeapon)
            {
                FindObjectOfType<AudioManager>().Play("EquipWeapon");

            }
            equipMinigun = false;
            minigunCurrentDuration = minigunDuration;
            imageColor.a = 1f;
            gun2Image.color = imageColor;
            imageColor.a = 0.5f;
            gun1Image.color = imageColor;
            gun3Image.color = imageColor;
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            numAmmo.enabled = false;
            selectedWeapon = 0;
            equipMinigun = false;
            minigunCurrentDuration = minigunDuration;
            imageColor.a = 1f;
            gun3Image.color = imageColor;
            imageColor.a = 0.5f;
            gun1Image.color = imageColor;
            gun2Image.color = imageColor;
            if (gun2 == 7)
            {
                imageColor.a = 0f;
                gun2Image.color = imageColor;
            }
        }

        if (equipMinigun)
        {
            katana.isAttacking = false;
            if (minigunCurrentDuration >= 0)
            {
                numAmmo.enabled = false;
                minigun.enabled = true;
                selectedWeapon = 2;
                minigunCurrentDuration -= Time.deltaTime;

                imageColor.a = 0.5f;
                gun2Image.color = imageColor;
                gun1Image.color = imageColor;
                gun3Image.color = imageColor;
                if (gun2 == 7)
                {
                    imageColor.a = 0f;
                    gun2Image.color = imageColor;
                }
            }
            else
            {
                equipMinigun = false;
                minigunCurrentDuration = minigunDuration;
                selectedWeapon = beforeMinugun;
                if (selectedWeapon == gun1)
                {
                    imageColor.a = 1f;
                    gun1Image.color = imageColor;
                    imageColor.a = 0.5f;
                    gun2Image.color = imageColor;
                    gun3Image.color = imageColor;
                }
                else
                {
                    imageColor.a = 1f;
                    gun2Image.color = imageColor;
                    imageColor.a = 0.5f;
                    gun1Image.color = imageColor;
                    gun3Image.color = imageColor;
                }

                
                if (gun2 == 7)
                {
                    imageColor.a = 0f;
                    gun2Image.color = imageColor;
                }
                numAmmo.enabled = true;
            }
        }
        else
        {
            minigun.enabled = false;
            FindObjectOfType<AudioManager>().Stop("MachineGunShoot");
        }

        if (previousSelectedWeapon != selectedWeapon) 
        {
            Select(selectedWeapon);
            previousSelectedWeapon = selectedWeapon;
        }

    }

    public void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].gameObject.SetActive(i == weaponIndex);
    }

    private void OnEnable()
    {
        RandomUP.OnMinigunCollected += startShoot;
    }

    private void OnDisable()
    {
        RandomUP.OnMinigunCollected -= startShoot;
    }

    private void startShoot()
    {
        equipMinigun = true;
        if (selectedWeapon != 2)
        {
            beforeMinugun = selectedWeapon;
        }
    }
}