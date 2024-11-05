using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TrainAgent : MonoBehaviour
{
    [SerializeField] Transform[] desPoints;
    [SerializeField] NavMeshAgent trainAgent;
    [SerializeField] AudioClip trainSound;

    public bool isFirstArrival = false;
    private int currentDestinationIndex = 0;
    private AudioSource currentAudioSource;

    private void Start()
    {
        currentAudioSource = GetComponent<AudioSource>();
        SetNextDestination();
    }

    private void SetNextDestination()
    {
        trainAgent.SetDestination(desPoints[currentDestinationIndex].position);
        StartCoroutine(WaitForArrival());
    }

    private IEnumerator WaitForArrival()
    {
        while (trainAgent.pathPending || trainAgent.remainingDistance > trainAgent.stoppingDistance)
        {
            yield return null;
        }
        currentAudioSource.PlayOneShot(trainSound);
        if (currentDestinationIndex == 0 && !isFirstArrival)
        {
            trainAgent.isStopped = true;
            yield return new WaitUntil(() => isFirstArrival);
            trainAgent.isStopped = false;
            isFirstArrival = false;
        }

        currentDestinationIndex = (currentDestinationIndex + 1) % desPoints.Length;
        SetNextDestination();
        isFirstArrival = false;
    }
}
