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

			float offshoot_x = GameManager.SCREEN_WIDTH * 0.5f;
			if(targetPos.x < GameManager.Instance.TopLeft.x + offshoot_x)
				targetPos.x = GameManager.Instance.TopLeft.x + offshoot_x;
			else if(targetPos.x  > GameManager.Instance.BottomRight.x - offshoot_x)
				targetPos.x = GameManager.Instance.BottomRight.x - offshoot_x;

			float offshoot_y = GameManager.SCREEN_HEIGHT * 0.5f;
			if(targetPos.y < GameManager.Instance.BottomRight.y  + offshoot_y)
				targetPos.y = GameManager.Instance.BottomRight.y + offshoot_y;
			else if(targetPos.y > GameManager.Instance.TopLeft.y - offshoot_y)
				targetPos.y = GameManager.Instance.TopLeft.y - offshoot_y;
            
			targetPos.y = Mathf.Clamp (targetPos.y, -1, 9999);
			transform.position = Vector3.Lerp (transform.position, targetPos + offset, 0.5f);
			//transform.position = new Vector3(target.position.x,target.position.y,-10);
		}

	}
}