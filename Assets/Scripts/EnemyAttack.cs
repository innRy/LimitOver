using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    [SerializeField] private int attackPower = 1;
    // Start is called before the first frame update
    
    private void OnCollisionEnter(Collision collision )
    {
        PlayerHP playerHP = collision.gameObject.GetComponent<PlayerHP>();

        if (playerHP != null)
        {
            playerHP.TakeDamage(attackPower);
        }
    }
}
