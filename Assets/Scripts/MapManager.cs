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

    [Header("🎯 Goal & Key Icons")]
    [SerializeField] private Transform goalTransform;      // 3D空間のゴールオブジェクト
    [SerializeField] private GameObject goalIconPrefab;    // ミニマップ用ゴールアイコンのPrefab

    [SerializeField] private Transform[] keyTransforms;    // 3D空間の鍵オブジェクト
    [SerializeField] private GameObject keyIconPrefab;     // ミニマップ用鍵アイコンのPrefab

    private RectTransform[] keyIcons;                      // ★【新規追加】生成した鍵のUIを記憶する配列

    [Header("👿 Enemy Icons")]
    [SerializeField] private Transform[] enemyTransforms;  // 3D空間の敵キャラクターたち
    [SerializeField] private GameObject enemyIconPrefab;   // ミニマップ用敵アイコンのPrefab

    private RectTransform[] enemyIcons;                    // 生成した敵UIのRectTransformを記憶する配列

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

        // 親たちのScaleを強制的に1に戻します
        gridContainer.localScale = Vector3.one;
        mapWindow.transform.localScale = Vector3.one;

        float uiWidth = gridContainer.rect.width;
        float uiHeight = gridContainer.rect.height;

        scaleFactorX = uiWidth / maze3DSize;
        scaleFactorY = uiHeight / maze3DSize;

        if (wallsParent != null) GenerateMinimapWalls();

        GenerateFogGrid();
        GenerateMapIcons();
        GenerateEnemyIcons();

        if (playerIcon != null) playerIcon.SetAsLastSibling();

        if (mapWindow != null) mapWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) mapWindow.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Space)) mapWindow.SetActive(false);

        if (playerTransform != null && mazeOrigin != null && playerIcon != null)
        {
            UpdatePlayerIcon();
            UpdateFogVisibility();

            // ★★★【新規追加】鍵の入手状況を監視 ★★★
            UpdateKeyIcons();

            // 敵の位置更新 ＋ 倒されたかどうかの監視
            UpdateEnemyIcons();
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

                float relX = ((x + 0.5f) / (float)fogResolution) * maze3DSize;
                float relZ = ((z + 0.5f) / (float)fogResolution) * maze3DSize;

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

        int pGridX = Mathf.Clamp(Mathf.FloorToInt((pX / maze3DSize) * fogResolution), 0, fogResolution - 1);
        int pGridZ = Mathf.Clamp(Mathf.FloorToInt((pZ / maze3DSize) * fogResolution), 0, fogResolution - 1);

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

    void GenerateMapIcons()
    {
        if (goalTransform != null && goalIconPrefab != null)
        {
            InstantiateMapIcon(goalTransform, goalIconPrefab, "GoalIcon_UI");
        }

        // ★★★ 鍵のアイコンを生成しつつ、後で消せるように配列に記憶する ★★★
        if (keyTransforms != null && keyIconPrefab != null)
        {
            keyIcons = new RectTransform[keyTransforms.Length];

            for (int i = 0; i < keyTransforms.Length; i++)
            {
                if (keyTransforms[i] != null)
                {
                    // 生成したアイコンのUIデータを受け取って配列に保存
                    keyIcons[i] = InstantiateMapIcon(keyTransforms[i], keyIconPrefab, $"KeyIcon_UI_{i}");
                }
            }
        }
    }

    // ★★★ 戻り値を「void」から「RectTransform」に変更し、生成したUIを返せるようにしました ★★★
    RectTransform InstantiateMapIcon(Transform target3D, GameObject prefab, string objName)
    {
        GameObject iconObj = Instantiate(prefab, gridContainer);
        iconObj.name = objName;

        RectTransform rt = iconObj.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;

        float relX = target3D.position.x - mazeOrigin.position.x;
        float relZ = target3D.position.z - mazeOrigin.position.z;

        float uiX = relZ * scaleFactorX;
        float uiY = -relX * scaleFactorY;
        rt.anchoredPosition = new Vector2(uiX, uiY);

        rt.localRotation = Quaternion.Euler(0, 0, -target3D.eulerAngles.y);

        return rt; // 生成したUIの操作権を返す
    }

    // ★★★【完全新規追加】鍵の入手状況をチェックして、消えていたらアイコンを隠す ★★★
    void UpdateKeyIcons()
    {
        if (keyTransforms == null || keyIcons == null) return;

        for (int i = 0; i < keyTransforms.Length; i++)
        {
            if (keyIcons[i] == null) continue;

            // 3D空間の鍵が「Destroyされた(null)」か「非表示にされた(activeInHierarchy==false)」かチェック
            if (keyTransforms[i] == null || !keyTransforms[i].gameObject.activeInHierarchy)
            {
                // まだミニマップ上でアイコンが表示されていたら、見えなくする
                if (keyIcons[i].gameObject.activeSelf)
                {
                    keyIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void GenerateEnemyIcons()
    {
        if (enemyTransforms == null || enemyIconPrefab == null) return;

        enemyIcons = new RectTransform[enemyTransforms.Length];

        for (int i = 0; i < enemyTransforms.Length; i++)
        {
            if (enemyTransforms[i] == null) continue;

            GameObject iconObj = Instantiate(enemyIconPrefab, gridContainer);
            iconObj.name = $"EnemyIcon_UI_{i}";

            RectTransform rt = iconObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;

            enemyIcons[i] = rt;
        }
    }

    // ★★★【一部修正】敵の生存をチェックして、倒されていたらアイコンを隠す ★★★
    void UpdateEnemyIcons()
    {
        if (enemyTransforms == null || enemyIcons == null) return;

        for (int i = 0; i < enemyTransforms.Length; i++)
        {
            if (enemyIcons[i] == null) continue;

            // 敵が「Destroyされた」か「非表示にされた」場合
            if (enemyTransforms[i] == null || !enemyTransforms[i].gameObject.activeInHierarchy)
            {
                if (enemyIcons[i].gameObject.activeSelf)
                {
                    enemyIcons[i].gameObject.SetActive(false); // アイコンを隠す
                }
                continue; // 敵がいないので位置更新はスキップ
            }

            // （もし敵が復活する仕様がある場合は、ここでアイコンを再表示）
            if (!enemyIcons[i].gameObject.activeSelf)
            {
                enemyIcons[i].gameObject.SetActive(true);
            }

            // 生きている場合は位置と向きを更新
            float eX = enemyTransforms[i].position.x - mazeOrigin.position.x;
            float eZ = enemyTransforms[i].position.z - mazeOrigin.position.z;

            float enemyUiX = eZ * scaleFactorX;
            float enemyUiY = -eX * scaleFactorY;
            enemyIcons[i].anchoredPosition = new Vector2(enemyUiX, enemyUiY);

            enemyIcons[i].localRotation = Quaternion.Euler(0, 0, -enemyTransforms[i].eulerAngles.y);
        }
    }
}