using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class TrainAgent : MonoBehaviour
{
    public static TrainAgent Instance;
    [SerializeField] Transform[] desPoints;
    [SerializeField] NavMeshAgent trainAgent;
    [SerializeField] AudioClip trainSound;

    public event System.Action OnTrainStopped;  

    LearningManager learningManager;
    public bool isFirstArrival = false;
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

        OnTrainStopped?.Invoke();

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