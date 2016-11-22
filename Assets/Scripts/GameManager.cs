using UnityEngine;

public class GameManager : MonoBehaviour {
    public int regularFiguresAmount = 0;

	public HostFigure mainTarget;    
	private Transform targetPointer;
	private Virus player;

    public void Init(){
        for(int i = 0 ; i < regularFiguresAmount ; i++){
			GameObject wanderer = Instantiate(Resources.Load<GameObject>("Soldier"));
            wanderer.transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(-9.8f, 9.8f), 0);
			wanderer.GetComponent<HostFigure>().Init (HostFigureType.Trump);
        }

		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		mainTarget.Init (HostFigureType.Trump);
		mainTarget.transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(10f, 20.8f), 0);
    }

    void Awake(){        
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
