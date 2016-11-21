using UnityEngine;
using System.Collections;

public class Flingable : MonoBehaviour {
	bool didInitiatedFling;

	Vector3 flingStartPosition;
	SpriteRenderer coneSprite;
	Vector2 aimDirection;
	float aimAngle;

	// Use this for initialization
	void Start () {
		coneSprite = GameObject.Find ("cone").GetComponent<SpriteRenderer> ();
		coneSprite.enabled = false;
	}
		
	// Update is called once per frame
	void Update () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (Input.GetMouseButtonDown (0) && !didInitiatedFling) {
			// begin aim
			didInitiatedFling = true;
			flingStartPosition = mousePos;
			coneSprite.enabled = true;
		//	GameObject.Find ("marker").transform.position = new Vector3 (mousePos.x, mousePos.y, 0);
		}
		if (Input.GetMouseButtonUp (0) && didInitiatedFling) {
			// launch
			didInitiatedFling = false;
			coneSprite.enabled = false;
			GetComponent<Virus> ().Launch (aimDirection.normalized, aimAngle);

		}

		if (didInitiatedFling) {			
			// update aim arrow
			aimDirection = transform.position - mousePos;
			aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
			if(aimAngle < 0) aimAngle += 360;

			coneSprite.transform.localEulerAngles = new Vector3 (0, 0, aimAngle);
		}
			

	}
}
