using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.ComponentModel;

public class MoveTo : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private Worker worker;
    private Animator animator;

    bool isMoving = false;
    bool atWork = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        worker = GetComponent<Worker>();
        animator = GetComponent<Animator>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        InvokeRepeating("Move", 10.0f, 75.0f);
    }


    void GotoNextPoint()
    {

            // Returns if no points have been set up
            if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
      
    }


    void Update()
    {

            if(worker._job != null && worker._house != null && !isMoving)
            {
                points = new Transform[2];
                points[0] = worker._job._building.transform;
                points[1] = worker._house.transform;
                isMoving = true;
            }
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            animator.enabled = false;
        }

    }


    void Move()
    {
        if (isMoving)
        {
            if (!atWork)
            {
                agent.destination = worker._job._building.transform.position;
                atWork = true;
            }
            else
            {
                agent.destination = worker._house.transform.position;
                atWork = false;
            }
            animator.enabled = true;
        }

    }
}