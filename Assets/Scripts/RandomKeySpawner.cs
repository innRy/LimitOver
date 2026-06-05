using UnityEngine;

public class RandomKeySpawner : MonoBehaviour
{
    public GameObject keyPrefab;       // 生成したい鍵のプレハブ
    public Transform[] spawnPoints;    // インスペクターで生成地点を指定する配列

    // ★【追加】生成した鍵のクローンを記憶しておくための配列
    private GameObject[] spawnedKeys;

    void Start()
    {
        // 生成地点と同じ数の記憶枠を用意する
        spawnedKeys = new GameObject[spawnPoints.Length];

        SpawnAtSelectedPoints();
    }

    void SpawnAtSelectedPoints()
    {
        // 配置した地点すべてに生成する
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (keyPrefab != null && spawnPoints[i] != null)
            {
                // 鍵を生成し、後で監視できるように配列に記憶しておく
                GameObject newKey = Instantiate(keyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                spawnedKeys[i] = newKey;
            }
        }
    }

    // ★【新規追加】毎フレーム、鍵が拾われたか（消えたか）を監視する処理
    void Update()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // 生成地点が既に消えている場合はスキップ
            if (spawnPoints[i] == null || !spawnPoints[i].gameObject.activeInHierarchy) continue;

            // もし「生成したはずの鍵」がプレイヤーに拾われてDestroyされた（null）、
            // あるいは SetActive(false) で見えなくなった場合
            if (spawnedKeys[i] == null || !spawnedKeys[i].activeInHierarchy)
            {
                // MapManagerが参照している「生成地点オブジェクト」も道連れにして非表示にする！
                // （これによってMapManagerが「鍵が消えた」と正確に認識し、ミニマップからアイコンを消してくれます）
                spawnPoints[i].gameObject.SetActive(false);
            }
        }
    }
}