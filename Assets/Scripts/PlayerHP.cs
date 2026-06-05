using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    private int hp;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private UnityEngine.UI.Image fillImage;

    [SerializeField] private GameObject gameOverText;

    // ★追加：スタート地点を記憶するための変数
    private Vector3 startPosition;

    void Start()
    {
        hp = maxHp;

        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;

        gameOverText.SetActive(false); // 最初は非表示

        // ★追加：ゲーム開始時の位置（スタート地点）を記憶しておく
        startPosition = transform.position;

        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        // ★追加：すでにHPが0以下の時は、追加のダメージ処理を無視する（連続で死ぬのを防ぐ）
        if (hp <= 0) return;

        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            UpdateHPBar();

            StartCoroutine(GameOverProcess()); // 死亡処理へ
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
        gameOverText.SetActive(true); // ゲームオーバー表示

        // （※必要であれば、ここでプレイヤーの移動操作を無効化する処理を入れると自然です）

        yield return new WaitForSeconds(4f); // 2秒待つ

        // ＝＝＝＝ ★ここから復活（リスポーン）処理に変更 ＝＝＝＝

        // 1. ゲームオーバー文字を隠す
        gameOverText.SetActive(false);

        // 2. HPを全回復させてバーの表示を更新する
        hp = maxHp;
        UpdateHPBar();

        // 3. プレイヤーを記憶しておいたスタート地点にワープさせる
        // ⚠️ CharacterControllerを使っている場合は、一度オフにしないとワープできない仕様があります
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = startPosition;

        if (cc != null) cc.enabled = true;

        // （※移動操作を無効化していた場合は、ここで再度有効化します）
    }
}