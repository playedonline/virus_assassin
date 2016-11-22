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
//		minX = horzExtent - GameManager.Instance.totalScreenWidth / 2.0f;
//		maxX = GameManager.Instance.totalScreenWidth / 2.0f - horzExtent;
//		minY = vertExtent - GameManager.Instance.totalScreenHeight / 2.0f;
//		maxY = GameManager.Instance.totalScreenHeight / 2.0f - vertExtent;
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
			//targetPos.x = Mathf.Clamp (targetPos.x, GameManager.Instance.TopLeft.x + 15, GameManager.Instance.BottomRight.x - 15);
			//targetPos.y = Mathf.Clamp (targetPos.y, GameManager.Instance.BottomRight.y + 5, GameManager.Instance.TopLeft.y - 5);
			targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
			targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

			//targetPos.y = Mathf.Clamp (targetPos.y, -1, 9999);
			transform.position = Vector3.Lerp (transform.position, targetPos + offset, 0.5f);
			//transform.position = new Vector3(target.position.x,target.position.y,-10);
		}

	}
}