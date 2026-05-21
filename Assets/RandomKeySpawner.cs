using UnityEngine;

public class RandomKeySpawner : MonoBehaviour
{
    public GameObject keyPrefab;          // 生成したい鍵のプレハブ
    public Transform[] spawnPoints;      // インスペクターで生成地点を指定する配列

    void Start()
    {
        SpawnAtSelectedPoints();
    }

    void SpawnAtSelectedPoints()
    {
        // 配置した地点すべてに生成する
        foreach (Transform point in spawnPoints)
        {
            if (keyPrefab != null)
            {
                Instantiate(keyPrefab, point.position, point.rotation);
            }
        }
    }
}