using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] private float attackRange = 1.0f;

    [SerializeField] private Transform attackPoint;

    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private int damage = 1;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Onhit()
    {
        Collider[] hits = Physics.OverlapSphere
        (
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (var enemy in hits)
        {
            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            if (enemyHP != null)
            {
                enemyHP.TakeDamage(damage);
            }
        }
    }
}
