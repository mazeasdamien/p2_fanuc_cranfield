using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject toFollow;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = toFollow.transform.position;
        gameObject.transform.rotation = toFollow.transform.rotation;
    }
}
