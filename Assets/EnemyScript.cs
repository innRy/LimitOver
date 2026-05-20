using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // ★敵を倒した時にドロップさせたいアイテム（プレハブ）を入れる箱
    public GameObject dropItemPrefab;

    void Start()
    {
    }

    void Update()
    {
    }

    // マウスクリックで敵を倒せるようにするテスト用
    void OnMouseDown()
    {
        Die();
    }

    // 敵が倒れる（消滅する）時の処理
    public void Die()
    {
        Debug.Log("敵を倒した！");

        // 箱にアイテムがセットされていればドロップする
        if (dropItemPrefab != null)
        {
            dropItemPrefab.SetActive(true);
            // Instantiate（生み出す）命令のみを残す
            GameObject drop = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);

            // シーン内から「KeyParent」という名前のオブジェクトを探し出して、親に設定する
            GameObject parentObj = GameObject.Find("KeyParent");
            if (parentObj != null)
            {
                drop.transform.SetParent(parentObj.transform);
            }
        }

        // 最後に敵自身を消す
        Destroy(gameObject);
    }
}