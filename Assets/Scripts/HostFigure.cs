using UnityEngine;
using UnitySteer2D.Behaviors;
using DG.Tweening;
public class HostFigure : MonoBehaviour {
    public string baseAssetsName;
    public float maxSpeed = 3;
    public float timeToLive = 30f;

    private bool m_isInfected = false;
    private float m_infectedTime = 0;
    private AutonomousVehicle2D m_autonomousVehicle2d;
    private SpriteRenderer m_spriteRenderer;
	private Animator m_animator;
	public HostFigureType hostType;
	

    public void Infect() {
        if(m_isInfected)
            return;

        Debug.Log("Infected!");
        m_isInfected = true;
        m_infectedTime = Time.realtimeSinceStartup;
		hostType = HostFigureType.Zombie;
		UpdateAnimationState ("Walk Front");
		m_spriteRenderer.transform.DOPunchScale (Vector3.one * 0.4f, 0.4f);
    }

    private float TimeLeftToLive() {
        return timeToLive - (Time.realtimeSinceStartup - m_infectedTime);
    }

    private void die() {
        Destroy(gameObject);
    }

    void Awake() {
        m_autonomousVehicle2d = GetComponent<AutonomousVehicle2D>();
        m_autonomousVehicle2d.MaxSpeed = maxSpeed;

        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		m_animator = GetComponentInChildren<Animator> ();
		hostType = HostFigureType.Soldier;
		UpdateAnimationState ("Walk Front");

		// temp breath animation
		m_spriteRenderer.transform.DOScale(Vector3.one * 0.07f, 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetRelative(true).SetDelay(Random.value);
    }

	private void UpdateAnimationState(string newState)
	{
		m_animator.Play (hostType.ToString() + " " + newState); // set to the correct skin for this character

	}

    void Update() {
        m_spriteRenderer.transform.rotation = Quaternion.identity;
		if (m_autonomousVehicle2d.Velocity.x > 0)
			m_spriteRenderer.transform.localScale = new Vector3 (-1, 1, 1);
		else
			m_spriteRenderer.transform.localScale = new Vector3 (1, 1, 1);


        if (m_isInfected) {
        //Debug.Log("TimeLeftToLive: " + TimeLeftToLive());
            if(TimeLeftToLive() < 0)
                die();
        }
    }
	public void OnHit()
	{
		Infect ();
	}
}

public enum HostFigureType {
	Soldier,
	Zombie
}