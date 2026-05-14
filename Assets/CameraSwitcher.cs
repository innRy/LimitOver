using System.Diagnostics;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("カメラの設定")]
    public Camera mainCamera;     // 普段のカメラ（プレイヤー視点など）
    public Camera overheadCamera; // 上空からの俯瞰カメラ

    void Start()
    {
        // ゲーム開始時はメインカメラを有効、俯瞰カメラを無効にする
        if (mainCamera != null && overheadCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            overheadCamera.gameObject.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("カメラが設定されていません！Inspectorで割り当ててください。");
        }
    }

    void Update()
    {
        // スペースキーが押されているかどうかの判定
        if (Input.GetKey(KeyCode.Space))
        {
            // 押している間：俯瞰カメラをオン、メインをオフ
            mainCamera.gameObject.SetActive(false);
            overheadCamera.gameObject.SetActive(true);
        }
        else
        {
            // 離している間：メインカメラをオン、俯瞰をオフ
            mainCamera.gameObject.SetActive(true);
            overheadCamera.gameObject.SetActive(false);
        }
    }
}