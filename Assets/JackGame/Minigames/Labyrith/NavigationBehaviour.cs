using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBehaviour : MonoBehaviour
    {
        public Transform player;
        public NavMeshAgent agent;
        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            agent.destination = player.position;
        Debug.Log(player.position);
        }
    }

