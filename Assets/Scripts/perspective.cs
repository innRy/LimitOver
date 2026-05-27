using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rigidbodyの付け忘れを防ぐ属性
[RequireComponent(typeof(Rigidbody))]
public class perspective : MonoBehaviour
{
    private float speed = 7.5f;
    private float sensitivity = 3.0f;

    private Rigidbody rb;

    void Start()
    {
        // Rigidbodyのコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 物理演算で勝手にパタンと倒れないように回転を固定
       // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // マウスカーソルで左右視点移動（回転はUpdateのままでOK）
        float mx = Input.GetAxis("Mouse X");
        if (Mathf.Abs(mx) > 0.001f)
        {
            transform.RotateAround(transform.position, Vector3.up, mx * sensitivity);
        }
    }

    // 物理移動は FixedUpdate で行うのがUnityの鉄則です
    void FixedUpdate()
    {
        // 入力の取得
        // 【修正1】 GetAxis から GetAxisRaw に変更（遊びをなくす）
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        // プレイヤーの「向いている向き」を基準にした移動方向を計算
        Vector3 moveDirection = (transform.forward * zInput) + (transform.right * xInput);

        // 【修正2】 入力がない時は速度を完全にゼロ（0, rb.linearVelocity.y, 0）にする
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 velocity = moveDirection.normalized * speed;
            velocity.y = rb.velocity.y; // 2022以前なら rb.velocity.y
            rb.velocity = velocity;
        }
        else
        {
            // キーが離されたら、横移動の速度を完全にゼロにして急停止！
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}