using UnityEngine;

public class GameManager : MonoBehaviour {
 	public const float SCREEN_WIDTH = 10.8f;
    public const float SCREEN_HEIGHT = 19.2f;
    public const float HORIZONTAL_SCREENS = 3;
    public const float VERTICAL_SCREENS = 3;
    public int regularFiguresAmount = 0;

	private Object m_hostFigurePrefab;
	public Vector3 TopLeft {get {
			return new Vector3(-SCREEN_WIDTH * (HORIZONTAL_SCREENS - 0.5f), (VERTICAL_SCREENS - 0.5f) * SCREEN_HEIGHT);
    }}

    public Vector3 BottomRight {get {
			return new Vector3(SCREEN_WIDTH * (HORIZONTAL_SCREENS - 0.5f), -0.5f * SCREEN_HEIGHT);
    }}	

	public HostFigure mainTarget;    
	private OffscreenPointer targetPointer;
	private Virus player;
	    
    void Awake(){        	
		regularFiguresAmount = 45;
		m_hostFigurePrefab = Resources.Load("Soldier");	
		Vector3 topLeft = TopLeft;
        Vector3 bottomRight = BottomRight;

		Application.targetFrameRate = 60;
		player = GameObject.Find ("Virus").GetComponent<Virus> ();

		
		for(int i = 0 ; i < regularFiguresAmount ; i++){
            GameObject hostFigure = Instantiate(m_hostFigurePrefab) as GameObject;
            hostFigure.transform.localPosition = new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y), 0);
            hostFigure.GetComponent<HostFigure>().Init(HostFigureType.Soldier, topLeft, bottomRight);
        }
					
		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		mainTarget.transform.localPosition = new Vector3(Random.Range(-SCREEN_WIDTH, SCREEN_WIDTH), Random.Range(SCREEN_HEIGHT * 2, SCREEN_HEIGHT * 3), 0);
		mainTarget.Init (HostFigureType.Trump, topLeft, bottomRight);

		targetPointer = transform.GetComponentInChildren<OffscreenPointer>();
		targetPointer.Init (mainTarget.transform, player.transform);
    }
}
