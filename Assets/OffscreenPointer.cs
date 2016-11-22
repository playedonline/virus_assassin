using DG.Tweening;
using UnityEngine;
using System.Collections;

public class OffscreenPointer : MonoBehaviour {
	public Transform target;    
	public Transform origin; 

	// Use this for initialization
	void Start () {
		transform.DOScale (Vector3.one * 0.2f, 1).SetEase (Ease.OutBounce).SetLoops (-1, LoopType.Yoyo).SetRelative(true);
	}

	public void Init(Transform target, Transform origin)
	{
		this.target = target;
		this.origin = origin;
	}

	void Update()
	{
		Vector3 directionToTarget = target.transform.position - origin.transform.position;
		directionToTarget.Normalize ();

		float pointerAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
		if (pointerAngle < 0) pointerAngle += 360;

		Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint (target.transform.position);
		bool inViewport = (targetViewportPosition.x > 0 && targetViewportPosition.x < 1 && targetViewportPosition.y > 0 && targetViewportPosition.y < 1);
		gameObject.SetActive (!inViewport);
		transform.localEulerAngles = new Vector3 (0, 0, pointerAngle); 

		targetViewportPosition.x = Mathf.Clamp (targetViewportPosition.x, 0, 1);
		targetViewportPosition.y = Mathf.Clamp (targetViewportPosition.y, 0, 1);
		transform.position = Camera.main.ViewportToWorldPoint (targetViewportPosition);
	}
}
