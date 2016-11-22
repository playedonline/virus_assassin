using UnityEngine;

public class GameManager : MonoBehaviour {
    public const float SCREEN_WIDTH = 10.8f;
    public const float SCREEN_HEIGHT = 19.2f;
    public const float SCREENS_WIDTH = 3;
    public const float SCREENS_HEIGHT = 3;

    public int regularFiguresAmount = 0;

    private Object m_hostFigurePrefab;

    public Vector3 TopLeft {get {
        return new Vector3(-SCREEN_WIDTH * (SCREENS_WIDTH - 0.5f), SCREEN_HEIGHT * (SCREENS_HEIGHT - 0.5f));
    }}

    public Vector3 BottomRight {get {
        return new Vector3(SCREEN_WIDTH * (SCREENS_WIDTH - 0.5f), -SCREEN_HEIGHT * (SCREENS_HEIGHT - 0.5f));
    }}
	public HostFigure mainTarget;
	private Transform targetPointer;
	private Virus player;

    public void Init(){
        Vector3 topLeft = TopLeft;
        Vector3 bottomRight = BottomRight;

        for(int i = 0 ; i < regularFiguresAmount ; i++){
            GameObject hostFigure = Instantiate(m_hostFigurePrefab) as GameObject;
            hostFigure.transform.localPosition = new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y), 0);
            hostFigure.GetComponent<HostFigure>().Init(HostFigureType.Soldier, topLeft, bottomRight);
        }

		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		mainTarget.transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(10f, 20.8f), 0);
		mainTarget.Init (HostFigureType.Trump, topLeft, bottomRight);
    }

    void Awake(){
        m_hostFigurePrefab = Resources.Load("Soldier");
		Application.targetFrameRate = 60;
		targetPointer = transform.Find ("TargetPointer");
		player = GameObject.Find ("Virus").GetComponent<Virus> ();
        Init();
    }

	void Update()
	{
		Vector3 directionToTarget = mainTarget.transform.position - player.transform.position;
		directionToTarget.Normalize ();
		//targetPointer.transform.position =
		float pointerAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
		if(pointerAngle < 0) pointerAngle += 360;
		targetPointer.transform.localEulerAngles = new Vector3 (0, 0, pointerAngle);
	}
}
