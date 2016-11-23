using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class Healthbar : MonoBehaviour {
	public float healthLeft;
	public float healthTotal;
	private SpriteRenderer redSpriteSprite;
	private Image redSpriteImage;
	public bool isTimeBased;

	void Awake()
	{
		healthLeft = healthTotal = 1; // default values
	}

	public void Init(float health, bool isTimeBased = true)
	{
		this.isTimeBased = isTimeBased;
		gameObject.SetActive (true);
		healthLeft = healthTotal = health;
		if (isTimeBased) // oh my god uglyyyy			 
			redSpriteSprite = transform.Find ("Red").GetComponent<SpriteRenderer> ();
		else
			redSpriteImage = transform.Find ("Red").GetComponent<Image> ();
	}

	// Update is called once per frame
	void Update () {
		if (isTimeBased) {
			healthLeft -= Time.deltaTime;
		}

		healthLeft = Mathf.Max (0, healthLeft);
		if(redSpriteSprite != null)
			redSpriteSprite.transform.DOScaleX (healthLeft / healthTotal, 0.1f);
		else if (redSpriteImage != null)
			redSpriteImage.transform.DOScaleX (healthLeft / healthTotal, 0.1f);
		
	}

	public void Disable()
	{
		gameObject.SetActive (false);
	}
		
	public bool isEmpty { get { return healthLeft <= 0; } }
}
