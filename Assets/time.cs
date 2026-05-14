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

    void Start()
    {
        elapsedTime = 0.0F; // 時間を初期化する
        f_Goal = 0;
        GoalMessage.text = "";
    }
    // Update is called once per frame
    void Update()
    {
        if (f_Goal == 0)
        {
            elapsedTime += Time.deltaTime;
        }
        // 経過時間を表示するために,経過時間を秒にしたストリングを作成する.
        TextTime.text = string.Format("Time {0:f2} sec", elapsedTime);
    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.name == "goal")
        {
            f_Goal = 1;
            GoalMessage.text = "Goal!";
            // シーン内にある MazeGenerator スクリプトを探して取得する
            MazeGenerator mazeGen = FindObjectOfType<MazeGenerator>();

            // もし取得できたら、その座標を利用する
            if (mazeGen != null)
            {
                // 例：このオブジェクト（プレイヤー等）をスタート地点の真上に移動させる
                transform.position = mazeGen.startWorldPosition;
            }
            
        }
    }
}
