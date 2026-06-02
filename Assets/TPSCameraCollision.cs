using UnityEngine;

public class TPSCameraCollision : MonoBehaviour
{
    [Header("ターゲット（プレイヤーオブジェクト）")]
    public Transform target;

    [Header("カメラの設定")]
    [Tooltip("プレイヤーからカメラまでの通常時の距離")]
    public float defaultDistance = 5.0f;
    [Tooltip("カメラがターゲットに最接近できる距離")]
    public float minDistance = 1.0f;
    [Tooltip("カメラの移動のなめらかさ（値が大きいほど遅い）")]
    public float smoothTime = 0.1f;
    [Tooltip("衝突判定から少し離すオフセット（ニアクリップ貫通防止）")]
    public float collisionOffset = 0.2f;

    [Header("衝突判定の設定")]
    [Tooltip("壁として扱うレイヤー（'Default' レイヤーなどを指定）")]
    public LayerMask wallLayer;

    private Vector3 currentVelocity; // SmoothDamp用の速度変数
    private float currentDistance;   // 現在のカメラ距離

    void Start()
    {
        // カメラがプレイヤーの子オブジェクトになっている場合は、親子関係を解除する
        // （物理移動と干渉させないため。もし親子関係が必要ならこの行をコメントアウト）
        if (transform.parent != null && transform.parent == target)
        {
            transform.parent = null;
        }

        currentDistance = defaultDistance;

        // もしLayerMaskが設定されていない場合、すべてのレイヤーを対象にする
        if (wallLayer.value == 0)
        {
            wallLayer = ~0; // すべてのレイヤー
            // ※必要に応じて、プレイヤー自身のコライダーを除外する処理を入れると良い
        }
    }

    void LateUpdate()
    {
        // ターゲットが設定されていない場合は何もしない
        if (target == null) return;

        // --- 1. カメラが本来あるべき「理想の移動先」を計算 ---
        // ターゲットの真後ろの座標を計算
        Vector3 defaultPosition = target.position - target.forward * defaultDistance;

        // --- 2. 壁との衝突判定（Raycast） ---
        // ターゲットの中心から、理想のカメラ位置に向かってレイを飛ばす
        Vector3 rayDirection = (defaultPosition - target.position).normalized;
        float rayLength = defaultDistance;

        RaycastHit hit;
        float finalDistance = defaultDistance; // 最終的なカメラ距離

        // レイが壁にぶつかったかチェック
        // ※ターゲット自身を撃ち抜かないように、ターゲットの少し前からレイを飛ばす工夫をする
        Vector3 rayStartPoint = target.position + rayDirection * 0.1f; // 少し前方から開始

        if (Physics.Raycast(rayStartPoint, rayDirection, out hit, rayLength, wallLayer))
        {
            // 壁にぶつかったら、ターゲットから衝突点までの距離を計算
            // そこからさらに衝突オフセット分だけ手前にカメラを配置する
            finalDistance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, defaultDistance);
        }

        // --- 3. カメラ位置の更新（なめらかに移動） ---
        // 計算された最終的な距離を使って、カメラの新しい位置を計算
        Vector3 targetPosition = target.position + rayDirection * finalDistance;

        // 現在の位置から新しい位置へ、なめらかに移動させる（SmoothDamp）
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // カメラを常にターゲットの方向に向ける（好みに合わせて）
        transform.LookAt(target.position + Vector3.up * 1.5f); // ターゲットの少し上（頭付近）を見る
    }
}