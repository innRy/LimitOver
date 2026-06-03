
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject mapWindow;       // MapWindowを割り当てる
    [SerializeField] private RectTransform gridContainer;// GridContainerを割り当てる
    [SerializeField] private RectTransform playerIcon;   // PlayerIconを割り当てる
    [SerializeField] private GameObject cellPrefab;      // 壁のクローンとして使うImageプレハブ
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

    [Tooltip("プレイヤーの周囲何マス分の霧を晴らすか")]
    [SerializeField] private int revealRadius = 2;

    // ★★★【ここから新規追加】ゴールと鍵のアイコン設定 ★★★
    [Header("🎯 Goal & Key Icons")]
    [SerializeField] private Transform goalTransform;      // 3D空間のゴールオブジェクト
    [SerializeField] private GameObject goalIconPrefab;    // ミニマップ用ゴールアイコンのPrefab（Image等）

    [SerializeField] private Transform[] keyTransforms;    // 3D空間の鍵オブジェクト（インスペクターで5つセットする）
    [SerializeField] private GameObject keyIconPrefab;     // ミニマップ用鍵アイコンのPrefab（Image等）
    // ★★★【新規追加ここまで】★★★

    private UnityEngine.UI.Image[,] fogGrid;

    // 内部計算用のスケール因子（縦横個別）
    private float scaleFactorX;
    private float scaleFactorY;

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

        // エディタ上の GridContainer の実際のピクセルサイズを自動取得
        float uiWidth = gridContainer.rect.width;
        float uiHeight = gridContainer.rect.height;

        scaleFactorX = uiWidth / maze3DSize;
        scaleFactorY = uiHeight / maze3DSize;

        // ★★★【描画順の改良】
        // 1. 【最背面】最初に黒い壁を生成します
        if (wallsParent != null)
        {
            GenerateMinimapWalls();
        }

        // 2. 【中間】その上からグレーの霧を被せて、壁を隠します
        GenerateFogGrid();

        // ★★★【新規追加】霧の上にゴールと鍵のアイコンを生成・配置します
        GenerateMapIcons();

        // 3. 【最前面】プレイヤーアイコンを一番手前に持ってきます
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

            // 毎フレームの霧晴らし処理
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

            // 提示コードの配置ルール
            float uiX = relZ * scaleFactorX;
            float uiY = -relX * scaleFactorY;
            rt.anchoredPosition = new Vector2(uiX, uiY);

            float sizeX = wall.lossyScale.z * scaleFactorX * wallThicknessMultiplier;
            float sizeZ = wall.lossyScale.x * scaleFactorY * wallThicknessMultiplier;
            rt.sizeDelta = new Vector2(sizeX, sizeZ);

            rt.localRotation = Quaternion.Euler(0, 0, -wall.eulerAngles.y);
        }
    }

    void UpdatePlayerIcon()
    {
        float pX = playerTransform.position.x - mazeOrigin.position.x;
        float pZ = playerTransform.position.z - mazeOrigin.position.z;

        playerIcon.anchorMin = Vector2.zero;
        playerIcon.anchorMax = Vector2.zero;
        playerIcon.pivot = new Vector2(0.5f, 0.5f);

        // 提示コードのプレイヤー配置ルール
        float playerUiX = pZ * scaleFactorX;
        float playerUiY = -pX * scaleFactorY;
        playerIcon.anchoredPosition = new Vector2(playerUiX, playerUiY);

        playerIcon.localRotation = Quaternion.Euler(0, 0, -playerTransform.eulerAngles.y);
    }

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

                if (img != null) img.color = Color.gray;

                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.zero;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.localScale = Vector3.one;

                // 3D空間の基準点からの相対メートルを計算
                float relX = ((x + 0.5f) / (float)fogResolution) * maze3DSize;
                float relZ = ((z + 0.5f) / (float)fogResolution) * maze3DSize;

                // ★★★ 壁・プレイヤーのルールと完全に一致（-relX に修正）
                float uiX = relZ * scaleFactorX;
                float uiY = relX * scaleFactorY;
                rt.anchoredPosition = new Vector2(uiX, uiY);

                float fogSizeX = (maze3DSize / (float)fogResolution) * scaleFactorX;
                float fogSizeY = (maze3DSize / (float)fogResolution) * scaleFactorY;
                rt.sizeDelta = new Vector2(fogSizeX + 0.5f, fogSizeY + 0.5f);

                fogGrid[x, z] = img;
            }
        }
    }

    void UpdateFogVisibility()
    {
        if (fogGrid == null) return;

        float pX = -(playerTransform.position.x - mazeOrigin.position.x);
        float pZ = playerTransform.position.z - mazeOrigin.position.z;

        // 1. まずプレイヤーの3D座標から、シンプルなインデックス（0〜100）を逆算
        int pGridX = Mathf.FloorToInt((pX / maze3DSize) * fogResolution);
        int pGridZ = Mathf.FloorToInt((pZ / maze3DSize) * fogResolution);

        // ⚠️【ここが今回の修正ポイント：安全装置の追加】
        // プレイヤーが迷路の外枠ギリギリや、ほんの少し外側にいた場合でも、
        // インデックスが「0～100」の間に絶対に収まるようにここでガチガチに固定（Clamp）します。
        pGridX = Mathf.Clamp(pGridX, 0, fogResolution - 1);
        pGridZ = Mathf.Clamp(pGridZ, 0, fogResolution - 1);

        // 3. 周囲の霧を晴らすループ処理（ここからは変更なし）
        for (int z = -revealRadius; z <= revealRadius; z++)
        {
            for (int x = -revealRadius; x <= revealRadius; x++)
            {
                int targetX = pGridX + x;
                int targetZ = pGridZ + z;

                if (targetX >= 0 && targetX < fogResolution && targetZ >= 0 && targetZ < fogResolution)
                {
                    if (fogGrid[targetX, targetZ] != null && fogGrid[targetX, targetZ].enabled)
                    {
                        fogGrid[targetX, targetZ].enabled = false;
                    }
                }
            }
        }
    }

    // ★★★【ここから完全新規追加】ゴールと鍵のアイコンを生成する処理 ★★★
    void GenerateMapIcons()
    {
        // 1. ゴールアイコンの配置
        if (goalTransform != null && goalIconPrefab != null)
        {
            InstantiateMapIcon(goalTransform, goalIconPrefab, "GoalIcon_UI");
        }

        // 2. 鍵アイコンの配置（設定した数だけループ）
        if (keyTransforms != null && keyIconPrefab != null)
        {
            foreach (Transform keyTransform in keyTransforms)
            {
                if (keyTransform != null)
                {
                    InstantiateMapIcon(keyTransform, keyIconPrefab, "KeyIcon_UI");
                }
            }
        }
    }

    // アイコンのUIを生成し、壁やプレイヤーと全く同じルールで位置合わせをする関数
    void InstantiateMapIcon(Transform target3D, GameObject prefab, string objName)
    {
        GameObject iconObj = Instantiate(prefab, gridContainer);
        iconObj.name = objName; // Hierarchyで見分けがつくように名前を変更

        RectTransform rt = iconObj.GetComponent<RectTransform>();

        // ピボットとアンカーの初期化
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;

        // 3D座標の相対位置を取得
        float relX = target3D.position.x - mazeOrigin.position.x;
        float relZ = target3D.position.z - mazeOrigin.position.z;

        // ★壁・プレイヤーと全く同じ「入れ替え・反転ルール」を適用
        float uiX = relZ * scaleFactorX;
        float uiY = -relX * scaleFactorY;
        rt.anchoredPosition = new Vector2(uiX, uiY);

        // アイコンの向きも3D空間に合わせる
        rt.localRotation = Quaternion.Euler(0, 0, -target3D.eulerAngles.y);
    }
}