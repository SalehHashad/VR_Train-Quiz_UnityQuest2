using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrainMover : MonoBehaviour
{
    [SerializeField] private List<Transform> destinations = new List<Transform>();

    [SerializeField] private float waitTimeAtStation = 1f;

    [SerializeField] private float trainSpeed = 5f;

    [SerializeField] private AudioSource audiosource;
    
    private NavMeshAgent agent;
    private int currentDestinationIndex = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("TrainMover requires a NavMeshAgent component. Please add one to the train GameObject.");
            enabled = false;
            return;
        }

        agent.speed = trainSpeed;
        agent.autoBraking = true;

        if (destinations.Count > 0)
        {
            StartCoroutine(MoveTrain());
        }
        else
        {
            Debug.LogError("No destinations assigned. Please add destination points to the TrainMover script.");
        }
    }

    private IEnumerator MoveTrain()
    {
        while (true)
        {
            if (destinations.Count == 0)
            {
                Debug.LogWarning("Destination list is empty. Train cannot move.");
                yield break;
            }

            agent.SetDestination(destinations[currentDestinationIndex].position);

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            Debug.Log($"Train reached destination: {destinations[currentDestinationIndex].name}");

            audiosource.Play();
            yield return new WaitForSeconds(waitTimeAtStation);

            currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Count;
        }
    }
}
