using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int hp;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;

    void Start()
    {
        hp = maxHp;

        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;

        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            UpdateHPBar();
            Die();
            return;
        }

        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        hpSlider.value = hp;

        float ratio = (float)hp / maxHp;
        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}