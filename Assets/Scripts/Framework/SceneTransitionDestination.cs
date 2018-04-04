using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        A, B, C, D, E, F, G,
    }

    // This matches the tag chosen on the TransitionPoint that this is the destination for.
    [Tooltip("This is the game object that has transitioned")]
    public DestinationTag destinationTag;
    public GameObject transitioningGameObject;
    public UnityEvent OnReachDestination;
}