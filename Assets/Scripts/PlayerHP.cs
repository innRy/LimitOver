using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    private int hp;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;

    [SerializeField] private GameObject gameOverText; // ← 追加

    void Start()
    {
        hp = maxHp;

        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;

        gameOverText.SetActive(false); // 最初は非表示

        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            UpdateHPBar();

            StartCoroutine(GameOverProcess()); // ← ここ重要
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

    IEnumerator GameOverProcess()
    {
        gameOverText.SetActive(true); // 表示

        yield return new WaitForSeconds(2f); // 2秒待つ

        Destroy(gameObject); // プレイヤー消滅
    }
}