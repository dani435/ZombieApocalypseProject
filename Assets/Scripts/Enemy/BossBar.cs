//boos health bar
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public EnemyIA boss;
    public Image fillBossBar;
    private Slider bossHealthSlider;

    private void Start()
    {
        bossHealthSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (bossHealthSlider.value <= bossHealthSlider.minValue)
        {
            fillBossBar.enabled = false;
        }

        else if (bossHealthSlider.value > bossHealthSlider.minValue && !fillBossBar.enabled)
        {
            fillBossBar.enabled = true;
        }

        float fillValue = boss.currentHealth / boss.maxHealth;

        bossHealthSlider.value = fillValue;

        if (boss.currentHealth <= boss.maxHealth / 4)
        {
            fillBossBar.color = Color.red;

        }
        else if (boss.currentHealth <= boss.maxHealth / 2)
        {
            fillBossBar.color = Color.yellow;
        }
        else
        {
            fillBossBar.color = Color.green;
        }
    }
}
