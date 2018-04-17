using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : SingletonScriptableOBject<PlayerData>
{
    public int maximumPlayerHealth;
    public int startingLives;
}
