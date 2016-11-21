using UnityEngine;
using UnitySteer2D.Behaviors;

public class Figure : MonoBehaviour {
    public string baseAssetsName;
    public float maxSpeed = 3;
    public float timeToLive = 10f;

    private bool m_isInfected = false;
    private float m_infectedTime = 0;
    private AutonomousVehicle2D m_autonomousVehicle2d;
    private SpriteRenderer m_spriteRenderer;

    public void Infect() {
        if(m_isInfected)
            return;

        Debug.LogError("Infected!");
        m_isInfected = true;
        m_infectedTime = Time.realtimeSinceStartup;
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
    }

    void Update() {
        m_spriteRenderer.transform.rotation = Quaternion.identity;

        if(transform.localPosition.y >= 4)
            Infect();

        if (m_isInfected) {
            Debug.Log("TimeLeftToLive: " + TimeLeftToLive());
            if(TimeLeftToLive() < 0)
                die();
        }
    }
}
