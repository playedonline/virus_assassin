using UnityEngine;

public class PositionLimit : MonoBehaviour {
    public float minX = -5.5f;
    public float maxX = 5.5f;
    public float minY = -9.8f;
    public float maxY = 9.8f;

    void Update() {
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