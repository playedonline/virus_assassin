using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Virus : MonoBehaviour {
	Rigidbody2D body;
	Animator animator;
	Transform spriteObject;
	bool isFlying;

	void Start () {
		body = GetComponent<Rigidbody2D> ();
		body.drag = 3f;
		animator = GetComponentInChildren<Animator> ();
		spriteObject = transform.Find ("Sprite");
		SetIdle ();

	}


	public void Launch(Vector2 direction, float angle)
	{
		body.velocity = (direction * 30);
		//transform.localRotation
		animator.Play("Virus Flying");
		spriteObject.localScale = Vector3.one;
		//spriteObject.localEulerAngles = new Vector3 (0, 0, -angle);
		DOTween.Kill (spriteObject);
		spriteObject.DOScale (new Vector3 (1.7f, 0.2f, 0), 0.2f).SetEase(Ease.InOutSine).SetLoops (-1, LoopType.Yoyo);

		isFlying = true;
	}

	void Update()
	{
		if (body.velocity.magnitude < 5 && isFlying)
			SetIdle ();
	}
		
	void SetIdle()
	{
		isFlying = false;
		animator.Play ("Virus Idle");
		spriteObject.localScale = Vector3.one;
		DOTween.Kill (spriteObject);
		spriteObject.DOScale (new Vector3 (1.3f, 0.7f, 0), 0.25f).SetEase(Ease.InOutSine).SetLoops (-1, LoopType.Yoyo);
	}
}
