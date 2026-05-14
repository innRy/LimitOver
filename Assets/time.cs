using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class time : MonoBehaviour
{
    // Textを外部に出力するための変数を定義する
    // [...]をつけることで,内部変数がインスペクターから操作できるようになる.
    // private を public にしても操作可能になる.
    [SerializeField] private TextMeshProUGUI TextTime;
    [SerializeField] private TextMeshProUGUI GoalMessage;

    // 経過時間を格納する変数 ここに開始からの時間が格納される.
    private float elapsedTime;
    // Start is called before the first frame update

    private int f_Goal;
    private int Stage_level;
    Coroutine coroutine1; //コルーチンの定義

    void Start()
    {
        elapsedTime = 0.0F; // 時間を初期化する
        f_Goal = 0;
        Stage_level = 0;
        GoalMessage.text = "";
    }
    // Update is called once per frame
    void Update()
    {
        if (f_Goal == 0)    //f_Goalが0の間経過時間を増やす
        {
            elapsedTime += Time.deltaTime;
        }
        // 経過時間を表示するために,経過時間を秒にしたストリングを作成する.
        TextTime.text = string.Format("Time {0:f2} sec", elapsedTime);
    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.name == "goal"|| other.gameObject.name == "GoalCube")    // ステージクリア(ゴール名を参照)
        {
            f_Goal = 1;
            Stage_level++;  //ステージレベルアップ
            GoalMessage.text = "Goal!";
            coroutine1 = StartCoroutine(Coroutine1());

            if (Stage_level == 1)//ステージ1へ遷移
            {
                MazeGenerator mazeGen = FindObjectOfType<MazeGenerator>();  //シーン内にある MazeGenerator スクリプトを探して取得する
                // もし取得できたら、その座標を利用する
                if (mazeGen != null)
                {   // 例：このオブジェクト（プレイヤー等）をスタート地点の真上に移動させる
                    transform.position = mazeGen.startWorldPosition;
                }

            }

        }
    }

    IEnumerator Coroutine1()    //ゴール時の画面表示の入れ替えと内部の数値を初期化
    {
        UnityEngine.Debug.Log("Coroutine1 Start.");
        yield return new WaitForSeconds(1.0f); // Sample2()の処理は1秒待機
        GoalMessage.text = "";  //Goalメッセージの解除
        f_Goal = 0; //時間とゴール表示を元に戻す
        elapsedTime = 0.0F;
        UnityEngine.Debug.Log("Coroutine1 End.");
    }

}
