using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ★追加①：UI（テキストなど）を操作するために絶対に必要！

public class PlayerScript : MonoBehaviour
{
    public GameObject goalUI;
    public Text countUI; // ★追加②：数字を表示するテキストを入れる箱

    public int fragmentCount = 0;
    public bool hasKey = false;

    void Start()
    {
        if (goalUI != null)
        {
            goalUI.SetActive(false);
        }

        // ★追加③：ゲーム開始時に「0 / 5」と表示させる
        UpdateCountUI();
    }

    void OnTriggerEnter(Collider other)
    {
        // ① 触れた相手が「Fragment（かけら）」だった場合
        if (other.CompareTag("Fragment"))//この条件を変えることで鍵のかけらの条件を変えることができる
        {
            fragmentCount++; // かけらの数を1増やす
            Destroy(other.gameObject); // 拾ったかけらを消す

            // ★追加④：かけらを拾うたびに、画面の数字を書き換える
            UpdateCountUI();

            Debug.Log("かけらをゲット！ 現在: " + fragmentCount + "個");

            // かけらが5個集まり、かつまだ鍵が完成していない場合
            if (fragmentCount >= 5 && hasKey == false)  //鍵の生成までに必要なかけらの個数を変更できる
            {
                hasKey = true; // 鍵が完成した状態にする
                Debug.Log("5つ集まった！鍵が完成した！");
            }
        }

        // ② 触れた相手が「Goal（ゴール）」だった場合
        if (other.gameObject.name == "Goal")    //この条件を変えることで触れるオブジェクトを変更できる
        {
            if (hasKey == true)
            {
                Debug.Log("ゴール！！");

                if (goalUI != null)
                {
                    goalUI.SetActive(true);
                }
            }
            else
            {
                int needCount = 5 - fragmentCount;
                Debug.Log("ゴールするには鍵が必要だ！ あとかけらが " + needCount + "個 必要だ！");
            }
        }
    }

    // ★追加⑤：文字を書き換える処理（長くなるのでまとめました）
    void UpdateCountUI()
    {
        // 箱の中にちゃんとテキストUIが入っていれば書き換える
        if (countUI != null)
        {
            // .text を使うと、画面の文字を自由に変更できます
            countUI.text = "鍵のかけら: " + fragmentCount + " / 5";
        }
    }
}