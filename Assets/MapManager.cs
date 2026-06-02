using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject mapWindow;       // MapWindowを割り当てる
    [SerializeField] private RectTransform gridContainer;// GridContainerを割り当てる
    [SerializeField] private RectTransform playerIcon;   // PlayerIconを割り当てる
    [SerializeField] private GameObject cellPrefab;      // 壁、およびフォグのマスとして使う共通のImageプレハブ
    [SerializeField] private Transform mazeOrigin;       // 迷路の基準点（左下の角）

    [Header("Map Scanner")]
    [SerializeField] private Transform wallsParent;      // 壁の親オブジェクト

    [Header("Player Tracking")]
    [SerializeField] private Transform playerTransform;  // 3D空間のプレイヤー

    [Header("3D Maze Settings")]
    [Tooltip("迷路全体の3D空間での実際の横幅・奥行き（メートル）。101マスの正確な実寸は 251 です")]
    [SerializeField] private float maze3DSize = 251f;

    [Header("Visual Adjustment")]
    [Tooltip("壁が太すぎて道が潰れる場合は数値を小さく(1.0等)してください")]
    [SerializeField] private float wallThicknessMultiplier = 1.0f;

    [Header("🌫️ フォグ（霧）の設定")]
    [Tooltip("何×何マスのグリッドで霧を作るか（101×101なら 101）")]
    [SerializeField] private int fogResolution = 101;

    [Tooltip("プレイヤーの周囲何マス分の霧を晴らすか（2〜3がおすすめです）")]
    [SerializeField] private int revealRadius = 2;

    // 内部計算用のスケール因子
    private float scaleFactorX;
    private float scaleFactorY;

    // フォグのImageを管理する2次元配列
    private UnityEngine.UI.Image[,] fogGrid;

    void Start()
    {
        if (gridContainer == null || mapWindow == null)
        {
            Debug.LogError("UIオブジェクトが割り当てられていません！");
            return;
        }

        // ⚠️親たちのScaleを強制的に1に戻します
        gridContainer.localScale = Vector3.one;
        mapWindow.transform.localScale = Vector3.one;

        float uiWidth = gridContainer.rect.width;
        float uiHeight = gridContainer.rect.height;

        scaleFactorX = uiWidth / maze3DSize;
        scaleFactorY = uiHeight / maze3DSize;

        // 1. 【最背面】まず3D空間の壁を生成（黒色）
        GenerateMinimapWalls();

        // 2. 【中間】その上に重なるようにグレーのフォググリッドを自動生成
        GenerateFogGrid();

        // 3. 【最前面】UIの描画順を制御し、プレイヤーアイコンを霧より手前に強制移動
        if (playerIcon != null)
        {
            playerIcon.SetAsLastSibling();
        }

        if (mapWindow != null) mapWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) mapWindow.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Space)) mapWindow.SetActive(false);

        if (playerTransform != null && mazeOrigin != null && playerIcon != null)
        {
            UpdatePlayerIcon();

            // プレイヤーの現在地から、今どのフォグマスの上にいるかを逆算して霧を晴らす
            UpdateFogVisibility();
        }
    }

    void GenerateMinimapWalls()
    {
        Transform[] allWalls = wallsParent.GetComponentsInChildren<Transform>();

        foreach (Transform wall in allWalls)
        {
            if (wall == wallsParent) continue;
            if (wall.GetComponent<MeshRenderer>() == null) continue;
            if (wall.lossyScale.x > 15f && wall.lossyScale.z > 15f) continue;

            GameObject wallUI = Instantiate(cellPrefab, gridContainer);
            RectTransform rt = wallUI.GetComponent<RectTransform>();
            UnityEngine.UI.Image img = wallUI.GetComponent<UnityEngine.UI.Image>();

            if (img != null) img.color = Color.black;

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;

            float relX = wall.position.x - mazeOrigin.position.x;
            float relZ = wall.position.z - mazeOrigin.position.z;

            // XとZの入れ替えルール
            float uiX = relZ * scaleFactorX;
            float uiY = -relX * scaleFactorY;
            rt.anchoredPosition = new Vector2(uiX, uiY);

            float sizeX = wall.lossyScale.z * scaleFactorX * wallThicknessMultiplier;
            float sizeZ = wall.lossyScale.x * scaleFactorY * wallThicknessMultiplier;
            rt.sizeDelta = new Vector2(sizeX, sizeZ);

            rt.localRotation = Quaternion.Euler(0, 0, -wall.eulerAngles.y);
        }
    }

    // ★★★ 今回新しく追加した、フォグ（グレーのマス）を自動生成する関数
    void GenerateFogGrid()
    {
        fogGrid = new UnityEngine.UI.Image[fogResolution, fogResolution];

        for (int z = 0; z < fogResolution; z++)
        {
            for (int x = 0; x < fogResolution; x++)
            {
                GameObject fogUI = Instantiate(cellPrefab, gridContainer);
                RectTransform rt = fogUI.GetComponent<RectTransform>();
                UnityEngine.UI.Image img = fogUI.GetComponent<UnityEngine.UI.Image>();

                if (img != null)
                {
                    img.color = Color.gray; // 初期状態はすべてグレー（未探索）
                }

                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.zero;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.localScale = Vector3.one;

                // 各フォグマスの中心となる3D空間上の相対座標を計算
                float relX = ((x + 0.5f) / (float)fogResolution) * maze3DSize;
                float relZ = ((z + 0.5f) / (float)fogResolution) * maze3DSize;

                // 壁の配置ルールと「100%完全同期」させてUI上に敷き詰める
                float uiX = relZ * scaleFactorX;
                float uiY = -relX * scaleFactorY;
                rt.anchoredPosition = new Vector2(uiX, uiY);

                // 1マス分のUIサイズを計算（隙間が空いて奥が透けないように+0.5ピクセル微増）
                float fogSizeX = (maze3DSize / (float)fogResolution) * scaleFactorX;
                float fogSizeY = (maze3DSize / (float)fogResolution) * scaleFactorY;
                rt.sizeDelta = new Vector2(fogSizeX + 0.5f, fogSizeY + 0.5f);

                // 後からUpdateで消せるように配列に保存
                fogGrid[x, z] = img;
            }
        }
    }

    // ★★★ プレイヤーの周囲の霧を消す（透過する）関数
    void UpdateFogVisibility()
    {
        if (fogGrid == null) return;

        // プレイヤーの3D相対座標を取得
        float pX = playerTransform.position.x - mazeOrigin.position.x;
        float pZ = playerTransform.position.z - mazeOrigin.position.z;

        // 3D座標から、フォグ配列のインデックス（0 〜 100）に逆算マッピング
        int pGridX = Mathf.Clamp(Mathf.FloorToInt((pX / maze3DSize) * fogResolution), 0, fogResolution - 1);
        int pGridZ = Mathf.Clamp(Mathf.FloorToInt((pZ / maze3DSize) * fogResolution), 0, fogResolution - 1);

        // 指定した半径（revealRadius）のぶんだけ周囲のImageの描画をオフ（透過）にする
        for (int z = -revealRadius; z <= revealRadius; z++)
        {
            for (int x = -revealRadius; x <= revealRadius; x++)
            {
                int targetX = pGridX + x;
                int targetZ = pGridZ + z;

                // 配列の範囲内かチェック
                if (targetX >= 0 && targetX < fogResolution && targetZ >= 0 && targetZ < fogResolution)
                {
                    if (fogGrid[targetX, targetZ] != null && fogGrid[targetX, targetZ].enabled)
                    {
                        // Imageコンポーネント自体をオフにすることで、中の黒い壁や道が綺麗に露出します
                        fogGrid[targetX, targetZ].enabled = false;
                    }
                }
            }
        }
    }

    void UpdatePlayerIcon()
    {
        float pX = playerTransform.position.x - mazeOrigin.position.x;
        float pZ = playerTransform.position.z - mazeOrigin.position.z;

        playerIcon.anchorMin = Vector2.zero;
        playerIcon.anchorMax = Vector2.zero;
        playerIcon.pivot = new Vector2(0.5f, 0.5f);

        float playerUiX = pZ * scaleFactorX;
        float playerUiY = -pX * scaleFactorY;
        playerIcon.anchoredPosition = new Vector2(playerUiX, playerUiY);

        playerIcon.localRotation = Quaternion.Euler(0, 0, -playerTransform.eulerAngles.y);
    }
}