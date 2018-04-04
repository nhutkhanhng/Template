using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : SingletonMono<Teleporter>
{

    protected bool m_Transitioning;


    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void Teleport(TransitionPoint transitionPoint)
    {
        Transform destinationTransform = Instance.GetDestination(transitionPoint.transitionDestinationTag).transform;
        Instance.StartCoroutine(Instance.Transition(transitionPoint.transitioningGameObject, true, transitionPoint.resetInputValuesOnTransition, destinationTransform.position, true));
    }

    public static void Teleport(GameObject transitioningGameObject, Transform destination)
    {
        Instance.StartCoroutine(Instance.Transition(transitioningGameObject, destination.position, false));
    }

    public static void Teleport(GameObject transitioningGameObject, Vector3 destinationPosition)
    {
        Instance.StartCoroutine(Instance.Transition(transitioningGameObject, destinationPosition, false));
    }

    public static void Teleport(GameObject transitioningGameObject, Vector3 destinationPosition, bool fade, System.Action callback)
    {
        Instance.StartCoroutine(Instance.Transition(transitioningGameObject, destinationPosition, fade, callback));
    }

    protected IEnumerator Transition(GameObject transitioningGameObject, Vector3 destinationPosition, bool fade)
    {
        m_Transitioning = true;

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneOut());

        transitioningGameObject.transform.position = destinationPosition;

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneIn());

        m_Transitioning = false;
    }


    protected IEnumerator Transition(GameObject transitioningGameObject, Vector3 destinationPosition, bool fade = false, System.Action beforTeleport = null, System.Action InTelePort = null, System.Action afterTeleport = null)
    {
        m_Transitioning = true;

        if (beforTeleport != null)
            beforTeleport();

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneOut());

        transitioningGameObject.transform.position = destinationPosition;
        if(InTelePort != null)
            InTelePort();

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneIn());

        m_Transitioning = false;

        if (afterTeleport != null)
            afterTeleport();
    }

    protected IEnumerator Transition(GameObject transitioningGameObject, bool releaseControl, bool resetInputValues, Vector3 destinationPosition, bool fade, System.Action callback = null)
    {
        m_Transitioning = true;

        if (releaseControl)
        {
            //if (m_PlayerInput == null)
            //    m_PlayerInput = FindObjectOfType<PlayerInput>();
            //m_PlayerInput.ReleaseControl(resetInputValues);
        }

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneOut());

        transitioningGameObject.transform.position = destinationPosition;

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneIn());

        if (releaseControl)
        {
            //m_PlayerInput.GainControl();
        }

        m_Transitioning = false;

        if (callback != null)
            callback();
    }


    protected SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
    {
        SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        Debug.LogWarning("No entrance was found with the " + destinationTag + " tag.");
        return null;
    }
}
