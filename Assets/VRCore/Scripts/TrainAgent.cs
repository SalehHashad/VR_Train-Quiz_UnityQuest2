using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TrainAgent : MonoBehaviour
{
    public static TrainAgent Instance;
    [SerializeField] private Transform[] desPoints;
    [SerializeField] private NavMeshAgent trainAgent;
    [SerializeField] private VoidEventChannelSO TrainStoppedEventSO;
    [SerializeField] private VoidEventChannelSO PlayLetterIntroEventSO;

    private int currentDestinationIndex = 0;
    private int lastPointIndex;
    private Coroutine arrivalCoroutine;
    public bool isTrainStopped = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (desPoints == null || desPoints.Length == 0 || trainAgent == null)
        {
            Debug.LogError("TrainAgent is not properly configured.");
            enabled = false;
            return;
        }

        lastPointIndex = desPoints.Length - 1;
    }

    private void Start()
    {
        SetNextDestination();
    }

    private void SetNextDestination()
    {
        if (arrivalCoroutine != null)
            StopCoroutine(arrivalCoroutine);

        trainAgent.SetDestination(desPoints[currentDestinationIndex].position);
        arrivalCoroutine = StartCoroutine(WaitForArrival());
    }

    private IEnumerator WaitForArrival()
    {
        while (trainAgent.pathPending || trainAgent.remainingDistance > trainAgent.stoppingDistance)
        {
            yield return null;
        }

        if (currentDestinationIndex == lastPointIndex - 1)
            TriggerLetterIntro();

        if (currentDestinationIndex == lastPointIndex)
        {
            TrainStoppedEventSO?.RaiseEvent();
            isTrainStopped = true; // Mark train as stopped
            Debug.Log("Train stopped at the last position.");
        }
        else
        {
            currentDestinationIndex = (currentDestinationIndex + 1) % desPoints.Length;
            SetNextDestination();
        }
    }

    private void TriggerLetterIntro()
    {
        Debug.Log("Train is at the second-to-last position: " + currentDestinationIndex);
        PlayLetterIntroEventSO?.RaiseEvent();
    }

    public void ResumeTrainMovement()
    {
        if (isTrainStopped)
        {
            isTrainStopped = false;

            if (currentDestinationIndex == lastPointIndex)
            {
                currentDestinationIndex = 0; 
            }

            SetNextDestination(); 
            Debug.Log("Train resumed movement.");
        }
        else
        {
            Debug.LogWarning("Train is already moving!");
        }
    }
}
