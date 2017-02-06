using UnityEngine;
using System.Collections;

public class AudioSingleton : MonoBehaviour
{

    private static AudioSingleton instance = null;
    public static AudioSingleton Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
