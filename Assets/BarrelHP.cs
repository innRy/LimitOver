using UnityEngine;

public class BarrelHP : MonoBehaviour
{
    public void TakeDamage(int damageAmount)
    {
        Destroy(gameObject);
    }
}