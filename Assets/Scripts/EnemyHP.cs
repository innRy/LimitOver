using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int hp;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private UnityEngine.UI.Image fillImage; // エラー対策済みのフルネーム指定
    [SerializeField] private GameObject itemPrefab; // 敵がドロップするアイテムのプレハブ
    [SerializeField, Range(0, 100)] private int dropChance = 100; // 確率0〜100%で設定可能

    // ★★★【新規追加】死んだときに起動したいテキスト（またはパネル）のゲームオブジェクト ★★★
    [SerializeField] private GameObject deathText;

    void Start()
    {
        hp = maxHp;

        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;

        // ゲーム開始時はテキストを確実に隠しておく
        if (deathText != null) deathText.SetActive(false);

        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0) return; // すでに死んでいる場合は処理しない安全装置

        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            UpdateHPBar();
            Die(); // 死亡処理へ
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
        // アイテムドロップ処理（元のロジックのまま変更なし）
        int randomValue = UnityEngine.Random.Range(0, 100);

        if (randomValue < dropChance)
        {
            if (itemPrefab != null)
            {
                Instantiate(itemPrefab, transform.position + Vector3.up * 1.0f, Quaternion.identity);
                Debug.Log("アイテムをドロップしました！");
            }
        }
        else
        {
            Debug.Log("アイテムはドロップしませんでした...");
        }

        // ★★★【変更点】即座にDestroyせず、死亡演出コルーチンを開始する ★★★
        StartCoroutine(DieProcess());
    }

    // ★★★【完全新規追加】時間を制御してテキストを表示・非表示にするコルーチン ★★★
    IEnumerator DieProcess()
    {
        // 1. 指定されたテキストオブジェクトを画面に起動（表示）する
        if (deathText != null) deathText.SetActive(true);

        // 2. 敵の「当たり判定（Collider）」をオフにして、プレイヤーが通り抜けられるようにする
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 3. 敵の頭上の「HPバー」を非表示にする
        if (hpSlider != null) hpSlider.gameObject.SetActive(false);

        // 4. 敵の「見た目（3Dモデル）」を一瞬で非表示にして、その場で消滅したように見せる
        // (オブジェクト自体を非アクティブにするとコルーチンまで止まってしまうため、メッシュだけをオフにします)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        // 5. テキストを画面に表示したまま「2秒間」待つ
        yield return new WaitForSeconds(2f);

        // 6. 2秒経ったら、表示していたテキストを非表示にする
        if (deathText != null) deathText.SetActive(false);

        // 7. 最後に、用済みとなった敵のゲームオブジェクトをメモリから完全に削除する
        Destroy(gameObject);
    }
}