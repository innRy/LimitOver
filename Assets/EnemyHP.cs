using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int hp;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject itemPrefab; // 敵がドロップするアイテムのプレハブ
    [SerializeField, Range(0, 100)] private int dropChance = 100; // 確率0〜100%で設定可能

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
        // --- 追加分：アイテム生成処理 ---
        // ランダムな数（0〜99）を生成
        int randomValue = UnityEngine.Random.Range(0, 100);

        // 生成した数字が確率（dropChance）以下ならアイテムをドロップ
        if (randomValue < dropChance)
        {
            if (itemPrefab != null)
            {
                Instantiate(itemPrefab, transform.position +Vector3.up*1.0f, Quaternion.identity);
                Debug.Log("アイテムをドロップしました！");
            }
        }
        else
        {
            Debug.Log("アイテムはドロップしませんでした...");
        }
    
        Destroy(gameObject);
    }
}