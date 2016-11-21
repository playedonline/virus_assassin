using System;
using UnityEngine;
using UnitySteer2D.Behaviors;

public class WondererPositionLimits : MonoBehaviour {
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Update(){
        if (transform.position.x < minX)
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        else if (transform.position.x > maxX)
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);

        if (transform.position.y < minY)
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        else if (transform.position.y > maxY)
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
    }
}