using UnityEngine;

public class GameManager : MonoBehaviour {
    public int regularFiguresAmount = 0;

	public HostFigure mainTarget;    
	private OffscreenPointer targetPointer;
	private Virus player;
	    
    void Awake(){        		
		Application.targetFrameRate = 60;
		player = GameObject.Find ("Virus").GetComponent<Virus> ();

		
		for(int i = 0 ; i < regularFiguresAmount ; i++){
			GameObject wanderer = Instantiate(Resources.Load<GameObject>("Soldier"));
			wanderer.transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(-9.8f, 9.8f), 0);
			wanderer.GetComponent<HostFigure>().Init (HostFigureType.Soldier);
		}
		
		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		mainTarget.Init (HostFigureType.Trump);
		mainTarget.transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(20f, 30.8f), 0);

		targetPointer = transform.GetComponentInChildren<OffscreenPointer>();
		targetPointer.Init (mainTarget.transform, player.transform);
    }


}
