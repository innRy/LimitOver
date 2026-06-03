using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("finalfinalstage")]
    public Vector3 nextPosition = new Vector3(83, 772, -8); // ここに固定座標を入力

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = nextPosition;
        }
    }
}