using System.Linq;
using UnityEngine;

/**
    Do not destroy this game object when loading a new scene.
*/
public class DoNotDestroy : MonoBehaviour
{
    void Awake()
    {
        // Prevent duplicate instances.
        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name == this.gameObject.name).ToArray();

        // Destroy duplicate instances.
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Do not destroy this instance.
        DontDestroyOnLoad(this.gameObject);
    }
}