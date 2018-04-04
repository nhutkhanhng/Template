﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControllerWrapper : MonoBehaviour
{

    public void RestartZone(bool resetHealth)
    {
        SceneController.RestartZone(resetHealth);
    }

    public void TransitionToScene(TransitionPoint transitionPoint)
    {
        SceneController.TransitionToScene(transitionPoint);
    }

    public void RestartZoneWithDelay(float delay)
    {
        SceneController.RestartZoneWithDelay(delay, false);
    }

    public void RestartZoneWithDelayAndHealthReset(float delay)
    {
        SceneController.RestartZoneWithDelay(delay, true);
    }
}
