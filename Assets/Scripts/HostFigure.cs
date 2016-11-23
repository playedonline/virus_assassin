using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HostFigure : MonoBehaviour {
    private const float PATH_POINT_DIFF = 3;

    public float speed = 3;
	public HostFigureType hostType;

    private bool m_isInfected = false;
    private SpriteRenderer m_spriteRenderer;
	public Transform visualContainer;
	private Animator m_animator;
    private List<Vector3> m_pathPoints;
    private int pointIndex = 0;
    private Tweener currentMoveTween;
    private Healthbar healthBar;

	public void Init(HostFigureType hostType){
        this.hostType = hostType;
        UpdateAnimationState ("Walk Front");

        m_pathPoints = new List<Vector3>();
        m_pathPoints.Add(transform.localPosition);
        Vector3 lastPos;
        for(int i = 0 ; i < Random.Range(4,10) ; i++){
            lastPos = m_pathPoints[m_pathPoints.Count - 1];
            m_pathPoints.Add(new Vector2(
				Random.Range(Mathf.Max(GameManager.Instance.spawnableArea.min.x, lastPos.x - PATH_POINT_DIFF), Mathf.Min(GameManager.Instance.spawnableArea.max.x, lastPos.x + PATH_POINT_DIFF)),
				Random.Range(Mathf.Max(GameManager.Instance.spawnableArea.min.y, lastPos.y - PATH_POINT_DIFF), Mathf.Min(GameManager.Instance.spawnableArea.max.y, lastPos.y + PATH_POINT_DIFF))
            ));
        }

        MoveToNextPoint();
    }

    private void MoveToNextPoint(){
        pointIndex += 1;
        Vector3 nextPoint = m_pathPoints[pointIndex];
        m_spriteRenderer.transform.localScale = new Vector3 (nextPoint.x - transform.localPosition.x < 0 ? 1 : -1, 1, 1);

        float duration = Vector3.Distance(transform.localPosition, nextPoint) / speed;
        currentMoveTween = transform.DOLocalMove(nextPoint, duration).SetEase(Ease.Linear).OnComplete(() => {
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

		healthBar.Init (3);
		GameObject splatPS = Instantiate (Resources.Load ("SplatPS") as GameObject);
		splatPS.transform.position = transform.position + Vector3.up;
		Destroy (splatPS, 2);
		GameManager.Instance.OnHostFigureInfected (this);
		//DOVirtual.DelayedCall (0.5f, TurnToZombie);		
		TurnToZombie();
        m_isInfected = true;        
		m_spriteRenderer.transform.DOPunchScale (Vector3.one * 0.4f, 0.4f);

    }

    private void Die() {

		if (GetComponentInChildren<Virus> () != null)
			GetComponentInChildren<Virus> ().Die ();

        GameManager.Instance.OnHostFigureDie(this);
        Destroy(gameObject);
    }

	void TurnToZombie()
	{
		hostType = HostFigureType.Zombie;
		UpdateAnimationState ("Walk Front");		
	}

    void Awake() {
        visualContainer = transform.Find ("Container");
        m_spriteRenderer = visualContainer.Find("Sprite").GetComponent<SpriteRenderer>();
        m_animator = GetComponentInChildren<Animator> ();

        healthBar = GetComponentInChildren<Healthbar> ();
        healthBar.Disable ();
		hostType = HostFigureType.Soldier;
		UpdateAnimationState ("Walk Front");
    }

	private void UpdateAnimationState(string newState)
	{
		m_animator.Play (hostType.ToString() + " " + newState); // set to the correct skin for this character
	}

    public void SetHostType(HostFigureType newHostType){
        hostType = newHostType;
        UpdateAnimationState("Walk Front");
    }

    void Update() {
        m_spriteRenderer.transform.rotation = Quaternion.identity;
        m_spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.localPosition.y * 1000);

        if (m_isInfected && healthBar.isEmpty)
            Die();
    }

	public void OnHit(Vector3 knockbackForce)
	{
		currentMoveTween.Kill ();
		--pointIndex;
		transform.DOMove (knockbackForce, 0.5f).SetRelative (true).OnComplete(MoveToNextPoint);
		Infect ();
	}
}

public enum HostFigureType {
	Soldier,
	Zombie,
    Trump,
    TrumpMAC
}
