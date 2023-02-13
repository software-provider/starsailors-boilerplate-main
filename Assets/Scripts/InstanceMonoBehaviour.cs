//=============================================================================
// Instance mono behavior // Chris
//=============================================================================

/**
 * This class is used for objects that I need to reference without having to manually attach a reference in the editor, or look for one in the gameobject.find. Instead we can just use "whatever".Instance to get the instance of the object in the scene. It also ensures there is only one of the object - Chris
 */

using UnityEngine;


public class InstanceMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected InstanceMonoBehaviour() { }

    protected static T _instance;
    public static T Instance => GetInstance();

    private static bool m_applicationIsQuitting = false;

    private static T GetInstance()
    {
        if (m_applicationIsQuitting) { return null; }

        if (_instance == null)
        {
            _instance = FindObjectOfType<T>();
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                _instance = obj.AddComponent<T>();
            }
        }
        return _instance;
    }

    protected virtual void Awake()
    {

        if (_instance == null)
            _instance = (T)(object)this;
        else if (_instance != this) // don't let another instance be created if there already is one
            Destroy(this);
    }

}

