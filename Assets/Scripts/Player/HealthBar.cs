//barra della vita del player
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerController player;
    public Image fillBar;
    private Slider healthSlider;

    //damage on screen
    public Image screenDamage;

    private void Start()
    {
        healthSlider = GetComponent<Slider>();

        Color blood = screenDamage.color;
        blood.a = 0f;
        screenDamage.color = blood;

    }

    private void Update()
    {
        if (healthSlider.value <= healthSlider.minValue)
        {
            fillBar.enabled = false;
        }

        else if (healthSlider.value > healthSlider.minValue && !fillBar.enabled)
        {
            fillBar.enabled = true;
        }

        float fillValue = player.currentHealth / player.maxHealth;

        healthSlider.value = fillValue;

        if (player.currentHealth <= player.maxHealth / 4)
        {
            fillBar.color = Color.red;

            //change alpha for screen damage
            Color blood = screenDamage.color;
            blood.a = 1f;
            screenDamage.color = blood;

        }
        else if (player.currentHealth <= player.maxHealth / 2)
        {
            fillBar.color = Color.yellow;

            //change alpha for screen damage
            Color blood = screenDamage.color;
            blood.a = 0.5f;
            screenDamage.color = blood;
        }
        else
        {
            fillBar.color = Color.green;

            //change alpha for screen damage
            Color blood = screenDamage.color;
            blood.a = 0f;
            screenDamage.color = blood;
        }
    }
}

