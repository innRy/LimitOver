using UnityEngine;

public class camerarotation : MonoBehaviour
{
    [Header("ターゲット（プレイヤーオブジェクト）")]
    public Transform target;

    [Header("回転の設定")]
    public float sensitivityY = 3.0f;
    public float minYAngle = -30.0f; // 下を向く制限（地面に潜らないように）
    public float maxYAngle = 60.0f;  // 上を向く制限

    [Header("衝突判定の設定")]
    public float defaultDistance = 5.0f;
    public float minDistance = 1.0f;
    public float collisionOffset = 0.2f;
    public LayerMask wallLayer;

    private float currentXAngle = 0.0f; // 上下の現在の回転角度
    private Vector3 currentVelocity;

    void Start()
    {
        // カメラの親子関係を解除（プレイヤーの回転に巻き込まれないようにするため）
        if (transform.parent != null && transform.parent == target)
        {
            transform.parent = null;
        }

        // 初期角度を取得
        currentXAngle = transform.localEulerAngles.x;

        if (wallLayer.value == 0)
        {
            wallLayer = ~0; // デフォルトですべてのレイヤーを対象に
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- 1. マウスの入力から「上下の回転角度」を計算 ---
        float mouseY = Input.GetAxis("Mouse Y");
        currentXAngle -= mouseY * sensitivityY; // 反転を防ぐためマイナス
        currentXAngle = Mathf.Clamp(currentXAngle, minYAngle, maxYAngle); // 角度を制限

        // --- 2. プレイヤーの向き（水平）とマウスの上下回転を合成した「理想の回転」を作る ---
        // プレイヤーのY軸回転（左右）を取得し、それにカメラのX軸回転（上下）を掛け合わせる
        Quaternion targetRotation = Quaternion.Euler(currentXAngle, target.eulerAngles.y, 0);

        // --- 3. 回転をベースに、壁がない場合の「理想のカメラ位置」を計算 ---
        // ターゲットの後ろ（targetRotation基準）にdefaultDistance分だけ離した位置
        Vector3 defaultPosition = target.position - (targetRotation * Vector3.forward * defaultDistance);

        // --- 4. プレイヤーから理想の位置に向かってレイを飛ばし、壁を検知 ---
        Vector3 rayDirection = (defaultPosition - target.position).normalized;
        float rayLength = defaultDistance;

        RaycastHit hit;
        float finalDistance = defaultDistance;

        // プレイヤーの中心から少し浮かせた位置（頭付近）からレイを開始すると安定します
        Vector3 rayStartPoint = target.position + Vector3.up * 1.0f;

        if (Physics.Raycast(rayStartPoint, rayDirection, out hit, rayLength, wallLayer))
        {
            // 壁にぶつかったら、距離を縮める
            finalDistance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, defaultDistance);
        }

        // --- 5. 最終的な位置と回転をカメラに適用 ---
        Vector3 targetPosition = rayStartPoint + rayDirection * finalDistance;

        // 位置をなめらかに移動
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.05f);

        // カメラの向きをターゲット（の少し上）に固定
        transform.LookAt(rayStartPoint);
    }
}