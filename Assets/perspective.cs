using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perspective : MonoBehaviour
{
    private float speed=10.0f;
    private float sensitivity = 3.0f;

    void Update()
    {
        // Playerの前後左右の移動
        float xMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // 左右の移動
        float zMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime; // 前後の移動
        transform.Translate(xMovement, 0, zMovement); // オブジェクトの位置を更新


        //マウスカーソルで左右視点移動
        float mx = Input.GetAxis("Mouse X");//カーソルの横の移動量を取得
        float my = Input.GetAxis("Mouse Y");//カーソルの縦の移動量を取得
        if (Mathf.Abs(mx) > 0.001f) // X方向に一定量移動していれば横回転
        {
            transform.RotateAround(transform.position, Vector3.up, mx * sensitivity); // 回転軸はplayerオブジェクトのワールド座標Y軸

        }
    }
}
