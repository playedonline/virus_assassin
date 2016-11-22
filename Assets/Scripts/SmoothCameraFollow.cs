using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class SmoothCameraFollow : MonoBehaviour {

	public float interpVelocity;
	public float minDistance;
	public float followDistance;
	public Transform target;
	public Vector3 offset;
	Vector3 targetPos;
	// Use this for initialization
	void Start () {
		targetPos = transform.position;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if (target) {
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.position.z;

			Vector3 targetDirection = (target.position - posNoZ);

			interpVelocity = targetDirection.magnitude * 30f;

			targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

            if(targetPos.x < GameManager.Instance.TopLeft.x)
                targetPos.x = GameManager.Instance.TopLeft.x;
            else if(targetPos.x > GameManager.Instance.BottomRight.x)
                targetPos.x = GameManager.Instance.BottomRight.x;

            if(targetPos.y < GameManager.Instance.BottomRight.y)
                targetPos.y = GameManager.Instance.BottomRight.y;
            else if(targetPos.y > GameManager.Instance.TopLeft.y)
                targetPos.y = GameManager.Instance.TopLeft.y;
            
			targetPos.y = Mathf.Clamp (targetPos.y, -1, 9999);
			transform.position = Vector3.Lerp (transform.position, targetPos + offset, 0.5f);
			//transform.position = new Vector3(target.position.x,target.position.y,-10);
		}

	}
}