﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Virus : MonoBehaviour {
	Rigidbody2D body;
	Animator animator;
	Transform spriteTransform;
	HostFigure currentHost;
	bool isFlying;

	void Start () {
		body = GetComponent<Rigidbody2D> ();
		body.drag = 3f;
		animator = GetComponentInChildren<Animator> ();
		spriteTransform = transform.Find ("Sprite");
		SetIdleOutOfHost ();

	}


	public void Launch(Vector2 direction, float angle)
	{
		body.velocity = (direction * 30);
		//transform.localRotation
		animator.Play("Virus Flying");
		transform.parent = null;
		spriteTransform.localScale = Vector3.one;
		transform.localEulerAngles = new Vector3 (0, 0, 0);
		DOTween.Kill (spriteTransform);
		spriteTransform.DOScale (new Vector3 (1.7f, 0.2f, 0), 0.2f).SetEase(Ease.InOutSine).SetLoops (-1, LoopType.Yoyo);
		currentHost = null;
		isFlying = true;
	}

	void Update()
	{
		if (body.velocity.magnitude < 5 && isFlying)
			SetIdleOutOfHost ();
	}
		
	void SetIdleOutOfHost()
	{		
		isFlying = false;
		animator.Play ("Virus Idle");
		spriteTransform.localScale = Vector3.one;
		DOTween.Kill (spriteTransform);
		spriteTransform.DOScale (new Vector3 (1.3f, 0.7f, 0), 0.25f).SetEase(Ease.InOutSine).SetLoops (-1, LoopType.Yoyo);
	}

	void SetHost(HostFigure newHost)
	{
		currentHost = newHost;
		DOTween.Kill (spriteTransform);
		transform.parent = newHost.transform;
		spriteTransform.localScale = Vector3.zero;
		transform.localPosition = Vector3.zero;
		body.velocity = Vector2.zero;
		isFlying = false;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (!isFlying)
			return;
		
		Debug.Log ("OntriggerEnter");
		HostFigure hostHit = other.GetComponent<HostFigure> ();
		if (hostHit != null) {
			hostHit.OnHit ();
			SetHost (hostHit);
		}
	}
}
