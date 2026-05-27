using UnityEngine;

public class Enemy_Ball : MonoBehaviour
{
    [Header("弾の設定")]
    public float speed = 10f;
    public int damage = 10;
    public float lifeTime = 5f; // 自動で消える時間

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // 一定時間後に弾を削除
        Destroy(gameObject, lifeTime);
    }

    // 発射処理
    public void Shoot(Vector3 direction)
    {
        rb.velocity = direction * speed;
    }

    // 当たり判定
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 親も含めてHP取得
            PlayerHP hp = other.GetComponentInParent<PlayerHP>();

            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject); // 当たったら消える
        }
    }
}