using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private GameObject followingTarget;

    // Start is called before the first frame update
    void Start()
    {
        followingTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (followingTarget != null)
        {
            transform.position = followingTarget.transform.position;
        }
    }

    public void Follow(GameObject gameObject)
    {
        followingTarget = gameObject;
    }
}
