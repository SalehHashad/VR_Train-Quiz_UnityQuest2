using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class TrainAgent : MonoBehaviour
{
    public static TrainAgent Instance;
    [SerializeField] Transform[] desPoints;
    public NavMeshAgent trainAgent;
    [SerializeField] AudioClip trainSound;
    public event System.Action OnTrainStopped;
    LearningManager learningManager;
    public bool isFirstArrival = false;
    public bool isTrainStopped = false;
    public bool isAtLastPosition = false; 
    private int currentDestinationIndex = 0;
    private int lastPointIndex; 

    private void Awake()
    {
        learningManager = FindObjectOfType<LearningManager>();
        Instance = this;
        lastPointIndex = desPoints.Length - 1; 
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

        isAtLastPosition = (currentDestinationIndex == lastPointIndex);

        if (currentDestinationIndex == 0 && !isFirstArrival)
        {
            float trainSpeed = trainAgent.speed;
            trainAgent.speed = 0;
            yield return new WaitUntil(() => isFirstArrival);
            trainAgent.speed = trainSpeed;
            isFirstArrival = false;
            isTrainStopped = true;
        }

        if (!isAtLastPosition)
        {
            currentDestinationIndex = (currentDestinationIndex + 1) % desPoints.Length;
            SetNextDestination();
        }
        else
        {
            isTrainStopped = true;
            OnTrainStopped?.Invoke();
            AudioManager.Instance.StopingAudio();
        }

        isFirstArrival = false;
    }

    private void Update()
    {
        
    }

    [ContextMenu("Move The Train")]
    public void MoveTheTrain()
    {
        isFirstArrival = true;
        isTrainStopped = false;
        isAtLastPosition = false; 

        if (currentDestinationIndex == lastPointIndex)
        {
            currentDestinationIndex = 0;
            SetNextDestination();
        }
    }
}