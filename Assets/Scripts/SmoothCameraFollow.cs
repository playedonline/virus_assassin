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
	float minX,minY,maxX,maxY;
	// Use this for initialization
	void Start () {
		targetPos = transform.position;

		var vertExtent = Camera.main.orthographicSize;    
		var horzExtent = vertExtent * Screen.width / Screen.height;

		// Calculations assume map is position at the origin
		minX = -30;//GameManager.Instance.spawnableArea.min.x + GameManager.SCREEN_WIDTH/4;
		maxX = 32;//GameManager.Instance.spawnableArea.max.x - GameManager.SCREEN_WIDTH/4;
		minY = -2.35f;//GameManager.Instance.spawnableArea.min.y + GameManager.SCREEN_HEIGHT/4;
		maxY = 40;//GameManager.Instance.spawnableArea.max.y - GameManager.SCREEN_HEIGHT/4;
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

			float offshoot_x = GameManager.SCREEN_WIDTH * 0.25f;
		//	targetPos.x = Mathf.Clamp (targetPos.x, GameManager.Instance.spawnableArea.min.x, GameManager.Instance.spawnableArea.max.x);
		//	targetPos.y = Mathf.Clamp (targetPos.y, GameManager.Instance.spawnableArea.min.y, GameManager.Instance.spawnableArea.max.y);
			targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
			targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

			//targetPos.y = Mathf.Clamp (targetPos.y, -1, 9999);
			transform.position = Vector3.Lerp (transform.position, targetPos + offset, 0.5f);
			//transform.position = new Vector3(target.position.x,target.position.y,-10);
		}

	}
}