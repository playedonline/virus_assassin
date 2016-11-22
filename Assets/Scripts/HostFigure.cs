using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HostFigure : MonoBehaviour {
    private const float PATH_POINT_DIFF = 3;

    public string baseAssetsName;
    public float maxSpeed = 3;
	public HostFigureType hostType;

    private bool m_isInfected = false;
    private float m_infectedTime = 0;
    private SpriteRenderer m_spriteRenderer;
	public Transform visualContainer;
	private Animator m_animator;
    private List<Vector3> m_pathPoints;
    private int pointIndex = 0;
    private Tweener currentMoveTween;
    private Healthbar healthBar;


    public void Init(HostFigureType hostType, Vector3 topLeft, Vector3 bottomRight){
        this.hostType = hostType;
        UpdateAnimationState ("Walk Front");

        m_pathPoints = new List<Vector3>();
        m_pathPoints.Add(transform.localPosition);
        Vector3 lastPos;
        for(int i = 0 ; i < Random.Range(4,10) ; i++){
            lastPos = m_pathPoints[m_pathPoints.Count - 1];
            m_pathPoints.Add(new Vector2(
                    Random.Range(Mathf.Max(topLeft.x, lastPos.x - PATH_POINT_DIFF), Mathf.Min(bottomRight.x, lastPos.x + PATH_POINT_DIFF)),
                    Random.Range(Mathf.Max(bottomRight.y, lastPos.y - PATH_POINT_DIFF), Mathf.Min(topLeft.y, lastPos.y + PATH_POINT_DIFF))
            ));
        }

        MoveToNextPoint();
    }

    private void MoveToNextPoint(){
        pointIndex += 1;
        Vector3 nextPoint = m_pathPoints[pointIndex];

        Debug.Log("MoveToNextPoint");
        m_spriteRenderer.transform.localScale = new Vector3 (nextPoint.x - transform.localPosition.x < 0 ? 1 : -1, 1, 1);

        currentMoveTween = transform.DOLocalMove(nextPoint, 2).SetEase(Ease.Linear).OnComplete(() => {
            if(m_pathPoints.Count - 1 <= pointIndex){
                m_pathPoints.Reverse();
                pointIndex = 0;
            }

            MoveToNextPoint();
        });
    }

    public void Infect() {
        if(m_isInfected)
            return;

		healthBar.Init (5);

        m_isInfected = true;
        m_infectedTime = Time.realtimeSinceStartup;
		hostType = HostFigureType.Zombie;
		UpdateAnimationState ("Walk Front");
		m_spriteRenderer.transform.DOPunchScale (Vector3.one * 0.4f, 0.4f);
    }

    private void Die() {
        Destroy(gameObject);
    }

    void Awake() {
        visualContainer = transform.Find ("Container");
        m_spriteRenderer = visualContainer.Find("Sprite").GetComponent<SpriteRenderer>();
        m_animator = GetComponentInChildren<Animator> ();

        healthBar = GetComponentInChildren<Healthbar> ();
        healthBar.Disable ();
		hostType = HostFigureType.Soldier;
		UpdateAnimationState ("Walk Front");

//		// temp breath animation
//		m_spriteRenderer.transform.DOScale(Vector3.one * 0.07f, 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetRelative(true).SetDelay(Random.value);
    }

	private void UpdateAnimationState(string newState)
	{
		m_animator.Play (hostType.ToString() + " " + newState); // set to the correct skin for this character

	}

    void Update() {
        m_spriteRenderer.transform.rotation = Quaternion.identity;

        if (m_isInfected && healthBar.isEmpty)
            Die();
    }

	public void OnHit()
	{
		Infect ();
	}
}

public enum HostFigureType {
	Soldier,
	Zombie,
    Trump
}