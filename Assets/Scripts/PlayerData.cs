using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : SingletonScriptableOBject<PlayerData>
{
    public int maximumPlayerHealth;
    public int startingLives;
}
