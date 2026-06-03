using UnityEngine;

public class DemonAttack : MonoBehaviour
{
    [Header("弾の設定")]
    [SerializeField] private GameObject projectilePrefab; // 弾プレハブ
    [SerializeField] private Transform firePoint;         // 発射位置
    [SerializeField] private Transform player;            // プレイヤー

    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 3f;   // 発射間隔
    [SerializeField] private float attackRange = 10f;     // 攻撃距離

    private float timer;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 一定距離以内でのみ攻撃
        if (distance <= attackRange)
        {
            timer += Time.deltaTime;

            if (timer >= attackInterval)
            {
                Shoot();
                timer = 0f;
            }
        }
    }

    void Shoot()
    {
        // プレイヤー方向を計算
        Vector3 direction = (player.position - firePoint.position).normalized;

        // 弾生成
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // 弾を飛ばす
        Enemy_Ball projectile = bullet.GetComponent<Enemy_Ball>();
        if (projectile != null)
        {
            projectile.Shoot(direction);
        }
    }
}
