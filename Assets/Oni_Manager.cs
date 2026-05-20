using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Oni_Manager : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent navMeshAgent;

    [Header("索敵設定")]
    [SerializeField] private float viewDistance = 10.0f; // 視界の届く距離
    [SerializeField] private float viewAngle = 90.0f;    // 視野角（前方の扇形の角度）
    [SerializeField] private LayerMask obstacleMask;     // 壁などの障害物レイヤー

    [Header("徘徊設定")]
    [SerializeField] private float patrolRadius = 15.0f; // ランダム移動の最大半径

    private bool isChasing = false; // 追跡中かどうか

    void Start()
    {
        player = GameObject.Find("ninngen");
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 2.0f;

        // 最初にランダムな目的地を設定
        SetRandomDestination();
    }

    void Update()
    {
        if (player == null) return;

        // プレイヤーが視界に入っているかチェック
        if (CheckVisualField())
        {
            // 視界に入ったら追跡モード
            isChasing = true;
            navMeshAgent.destination = player.transform.position;
        }
        else
        {
            // 見失った、または最初から見えていない場合
            if (isChasing)
            {
                // 追跡モードだったのに見失った場合、その場で一度徘徊モードに戻す
                isChasing = false;
                SetRandomDestination();
            }

            // 徘徊中、目的地に近づいたら次のランダム目的地を設定
            // pathPendingは経路計算中かどうか、remainingDistanceは目的地までの残り距離
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                SetRandomDestination();
            }
        }
    }

    // 視界の判定（距離、角度、遮蔽物）
    private bool CheckVisualField()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 1. 距離のチェック
        if (distanceToPlayer > viewDistance) return false;

        // 2. 角度のチェック（鬼の正面ベクトルとプレイヤーへのベクトルのなす角）
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2.0f) return false;

        // 3. 障害物（壁）のチェック（Raycastを飛ばす）
        // 鬼の足元からではなく、少し浮かせた位置（Vector3.up * 0.5f など）から飛ばすと安定します
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = (player.transform.position + Vector3.up * 0.5f) - rayOrigin;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, viewDistance, obstacleMask))
        {
            // もし何かに当たって、それがプレイヤーじゃなければ「壁の裏にいる」と判定
            if (hit.collider.gameObject != player)
            {
                return false;
            }
        }

        // すべての条件をクリアしたら「見えている」
        return true;
    }

    // ナブメッシュ上でランダムな目的地を決める関数
    private void SetRandomDestination()
    {
        // 自身の周囲のランダムな方向・距離の点を計算
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        // 計算した点がちゃんと歩ける場所（NavMesh上）にあるか確認し、一番近い歩ける場所を取得
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            navMeshAgent.destination = navHit.position;
        }
    }
}