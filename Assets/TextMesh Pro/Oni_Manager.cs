
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Oni_Manager : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent navMeshAgent;

    [Header("���G�ݒ�")]
    [SerializeField] private float viewDistance = 10.0f; // ���E�̓͂�����
    [SerializeField] private float viewAngle = 90.0f;    // ����p�i�O���̐�`�̊p�x�j
    [SerializeField] private LayerMask obstacleMask;     // �ǂȂǂ̏�Q�����C���[

    [Header("�p�j�ݒ�")]
    [SerializeField] private float patrolRadius = 15.0f; // �����_���ړ��̍ő唼�a

    private bool isChasing = false; // �ǐՒ����ǂ���

    void Start()
    {
        player = GameObject.Find("unitychan");
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 2.0f;

        // �ŏ��Ƀ����_���ȖړI�n��ݒ�
        SetRandomDestination();
    }

    void Update()
    {
        if (player == null) return;

        // �v���C���[�����E�ɓ����Ă��邩�`�F�b�N
        if (CheckVisualField())
        {
            // ���E�ɓ�������ǐՃ��[�h
            isChasing = true;
            navMeshAgent.destination = player.transform.position;
        }
        else
        {
            // ���������A�܂��͍ŏ����猩���Ă��Ȃ��ꍇ
            if (isChasing)
            {
                // �ǐՃ��[�h�������̂Ɍ��������ꍇ�A���̏�ň�x�p�j���[�h�ɖ߂�
                isChasing = false;
                SetRandomDestination();
            }

            // �p�j���A�ړI�n�ɋ߂Â����玟�̃����_���ړI�n��ݒ�
            // pathPending�͌o�H�v�Z�����ǂ����AremainingDistance�͖ړI�n�܂ł̎c�苗��
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                SetRandomDestination();
            }
        }
    }

    // ���E�̔���i�����A�p�x�A�Օ����j
    private bool CheckVisualField()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 1. �����̃`�F�b�N
        if (distanceToPlayer > viewDistance) return false;

        // 2. �p�x�̃`�F�b�N�i�S�̐��ʃx�N�g���ƃv���C���[�ւ̃x�N�g���̂Ȃ��p�j
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2.0f) return false;

        // 3. ��Q���i�ǁj�̃`�F�b�N�iRaycast���΂��j
        // �S�̑�������ł͂Ȃ��A�������������ʒu�iVector3.up * 0.5f �Ȃǁj�����΂��ƈ��肵�܂�
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = (player.transform.position + Vector3.up * 0.5f) - rayOrigin;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, viewDistance, obstacleMask))
        {
            // ���������ɓ������āA���ꂪ�v���C���[����Ȃ���΁u�ǂ̗��ɂ���v�Ɣ���
            if (hit.collider.gameObject != player)
            {
                return false;
            }
        }

        // ���ׂĂ̏������N���A������u�����Ă���v
        return true;
    }

    // �i�u���b�V����Ń����_���ȖړI�n�����߂�֐�
    private void SetRandomDestination()
    {
        // ���g�̎��͂̃����_���ȕ����E�����̓_���v�Z
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        // �v�Z�����_�������ƕ�����ꏊ�iNavMesh��j�ɂ��邩�m�F���A��ԋ߂�������ꏊ���擾
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            navMeshAgent.destination = navHit.position;
        }
    }
}