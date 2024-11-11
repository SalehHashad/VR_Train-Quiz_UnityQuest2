using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TrainAgent : MonoBehaviour
{
    public static TrainAgent Instance;
    [SerializeField] Transform[] desPoints;
    [SerializeField] NavMeshAgent trainAgent;
    [SerializeField] AudioClip trainSound;

    LearningManager learningManager;
    private bool isFirstArrival = false;
    private int currentDestinationIndex = 0;

    private void Awake()
    {
        learningManager = FindObjectOfType<LearningManager>();
        Instance = this;
    }

    private void Start()
    {
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
        if (currentDestinationIndex == 0 && !isFirstArrival)
        {
            float trainSpeed = trainAgent.speed;
            trainAgent.speed = 0;
            yield return new WaitUntil(() => isFirstArrival);
            trainAgent.speed = trainSpeed;
            isFirstArrival = false;
        }

        currentDestinationIndex = (currentDestinationIndex + 1) % desPoints.Length;
        SetNextDestination();
        isFirstArrival = false;
    }

    [ContextMenu("Move The Train")]
    public void MoveTheTrain()
    {
        isFirstArrival = true;
    }
}
