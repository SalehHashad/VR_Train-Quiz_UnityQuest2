using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = " Events / Void Events")]
public class VoidEventChannelSO : ScriptableObject
{
    public UnityAction onEventRaised;

    public void RaiseEvent()
    {
        onEventRaised.Invoke();
    }

}
