using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5.0f;
    // Start is called before the first frame update

    private Rigidbody  rb;

    private Animator animator;

    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ChangeAnimation();
    }

    private void FixedUpdate()
    {
        Walk();
    }

    private void GetInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(x,0,z).normalized;
    }

    private void Walk()
    {
        rb.velocity = new Vector3(
        moveDirection.x * moveSpeed,
        rb.velocity.y,
        moveDirection.z * moveSpeed
        );
    }

    private void ChangeAnimation()
    {
        if (moveDirection == new Vector3(0,0,0))
        {
            animator.SetBool("Walk",false);
        }
        else 
        {
            animator.SetBool("Walk",true);
        }
    }

}
