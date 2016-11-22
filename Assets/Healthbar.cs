using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {
	public float healthLeft;
	public float healthTotal;
	private SpriteRenderer redSprite;

	void Awake()
	{
		redSprite = transform.Find ("Red").GetComponent<SpriteRenderer> ();
		healthLeft = healthTotal = 1; // default values
	}

	public void Init(float health)
	{
		gameObject.SetActive (true);
		healthLeft = healthTotal = health;
	}

	// Update is called once per frame
	void Update () {
		healthLeft -= Time.deltaTime;
		healthLeft = Mathf.Max (0, healthLeft);

		redSprite.transform.localScale = new Vector3 (healthLeft / healthTotal, 1, 1);
	}

	public void Disable()
	{
		gameObject.SetActive (false);
	}

	public bool isEmpty { get { return healthLeft <= 0; } }
}
