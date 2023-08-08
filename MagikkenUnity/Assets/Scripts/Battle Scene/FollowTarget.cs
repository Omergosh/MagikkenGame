using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform beAt;
    public Transform lookAt;

    private void Update()
    {
        transform.position = beAt.position;
        transform.LookAt(lookAt);
    }

    private void LateUpdate()
    {
        transform.position = beAt.position;
        transform.LookAt(lookAt);
    }
}
