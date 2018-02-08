using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// Singleton scriptableObject
/// </summary>
public class SingletonScriptableOBject<T> : ScriptableObject where T: ScriptableObject {

    private static T singleton;

    public static bool IsInstanceValid() { return singleton != null; }

    public static T Instance
    {
        get
        {
            if (SingletonScriptableOBject<T>.singleton == null)
            {
                SingletonScriptableOBject<T>.singleton = (T)FindObjectOfType(typeof(T));
                if (SingletonScriptableOBject<T>.singleton == null)
                {
                    //GameObject obj = new GameObject();
                    //obj.name = "[@" + typeof(T).Name + "]";
                    SingletonScriptableOBject<T>.singleton = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
            }

            return SingletonScriptableOBject<T>.singleton;
        }
    }

}



/// <summary>
/// Singleton base class
/// </summary>
public class SingleTon<T> where T : class, new()
{ 
    private static readonly T singleton = new T();

    public static T instance
    {
        get
        {
            return singleton;
        }
    }
}

/// <summary>
/// Singleton for mono behavior object
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T singleton;

    public static bool IsInstanceValid() { return singleton != null; }

    void Reset()
    {
        gameObject.name = typeof(T).Name;
    }

    public static T instance
    {
        get
        {
            if (SingletonMono<T>.singleton == null)
            {
                SingletonMono<T>.singleton = (T)FindObjectOfType(typeof(T));
                if (SingletonMono<T>.singleton == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "[@" + typeof(T).Name + "]";
                    SingletonMono<T>.singleton = obj.AddComponent<T>();
                }
            }

            return SingletonMono<T>.singleton;
        }
    }

}