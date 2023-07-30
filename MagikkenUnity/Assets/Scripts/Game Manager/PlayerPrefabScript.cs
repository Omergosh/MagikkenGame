using UnityEngine;

public class PlayerPrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GlobalInputManager.instance.transform);
    }
}
