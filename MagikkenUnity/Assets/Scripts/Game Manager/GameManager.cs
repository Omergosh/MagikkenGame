using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public AnimationDataLibrary animationDataLibrary;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error, SINGLETON - Trying to create another instance of a singleton!");
            Destroy(this.gameObject); // Someday, this will cause a bug, I'm sure.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
