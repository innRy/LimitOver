using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Oni_Manager : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        player = GameObject.Find("ninngen");
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 2.0f;
    }

    void Update()
    {
        navMeshAgent.destination = player.transform.position;
    }
}
