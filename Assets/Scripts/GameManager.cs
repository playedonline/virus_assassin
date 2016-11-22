using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static float SCREEN_WIDTH ;
	public static float SCREEN_HEIGHT ;
	public static float HORIZONTAL_TILES = 8;
	public static float VERTICAL_TILES = 4;

	public static GameManager Instance;
	public int score;

    private Object m_hostFigurePrefab;
    public Vector3 TopLeft {get {
			return new Vector3(-bgSprite.bounds.size.x * HORIZONTAL_TILES / 2, VERTICAL_TILES * bgSprite.bounds.size.y + 0.5f * SCREEN_HEIGHT);
    }}

    public Vector3 BottomRight {get {
			return new Vector3(bgSprite.bounds.size.x * HORIZONTAL_TILES / 2, -SCREEN_HEIGHT / 2);
    }}

    public HostFigure mainTarget;
    private OffscreenPointer targetPointer;
    private Virus player;
	public Text scoreText;
	private Canvas canvas;
    public Sprite bgSprite;
    private List<HostFigure> hostFigures = new List<HostFigure>();

    void Awake(){
		GameManager.Instance = this;

		SCREEN_HEIGHT = Camera.main.orthographicSize * 2;    
		SCREEN_WIDTH = SCREEN_HEIGHT * Screen.width / Screen.height;

		Debug.Log (SCREEN_HEIGHT + "," + SCREEN_WIDTH +TopLeft + BottomRight);
		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
         m_hostFigurePrefab = Resources.Load("Soldier");

        Application.targetFrameRate = 60;
        player = GameObject.Find ("Virus").GetComponent<Virus> ();

        float x = TopLeft.x + bgSprite.bounds.extents.x;
        float y = TopLeft.y - bgSprite.bounds.extents.y;
        GameObject bg = new GameObject("bg");
        while(y > BottomRight.y){
            SpriteRenderer bgsr = new GameObject("bg").AddComponent<SpriteRenderer>();
            bgsr.transform.parent = bg.transform;
            bgsr.transform.localPosition = new Vector3(x, y);
            bgsr.sprite = bgSprite;
            bgsr.sortingLayerName = "Background";
            bgsr.sortingOrder = -1;

            for(int i = 0 ; i < Random.Range(0,0) ; i++)
                SpawnNewSoldier(new Vector3(Random.Range(x - bgSprite.bounds.extents.x, x + bgSprite.bounds.extents.x), Random.Range(y - bgSprite.bounds.extents.y, y + bgSprite.bounds.extents.y), 0));

            x += bgSprite.bounds.size.x;
            if(x > BottomRight.x){
                x = TopLeft.x + bgSprite.bounds.extents.x;
                y -= bgSprite.bounds.size.y;
            }
        }

		SpawnNewTarget ();

    }

	void Update()
	{
		scoreText.text = score.ToString ();

        if(hostFigures.Count < 14 && Random.value < 0.01)
            ReSpawnSoldier();
	}

    public void OnHostFigureDie(HostFigure hf){
        hostFigures.Remove(hf);
    }

    public void ReSpawnSoldier(){
        int retries = 100;
        for(int i = 0 ; i < retries ; i++){
            Vector3 pos = new Vector3(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y), 0);

            // is in camera
			Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint (pos);
			if (targetViewportPosition.x > 0 && targetViewportPosition.x < 1 && targetViewportPosition.y > 0 && targetViewportPosition.y < 1)
                continue;

            bool isNearToSoldier = false;
            foreach (HostFigure hf in hostFigures)
                if(Vector3.Distance(pos, hf.transform.localPosition) < 10) {
                    isNearToSoldier = true;
                    break;
                }

            if(isNearToSoldier)
                continue;

            SpawnNewSoldier(pos);
            break;
        }
    }

    public void SpawnNewSoldier(Vector3 position){
        GameObject hostFigureGO = Instantiate(m_hostFigurePrefab) as GameObject;
        hostFigureGO.transform.localPosition = position;
        HostFigure hostFigure = hostFigureGO.GetComponent<HostFigure>();
        hostFigure.Init(HostFigureType.Soldier, TopLeft, BottomRight);
        hostFigures.Add(hostFigure);
    }

	public void SpawnNewTarget()
	{		
		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		Vector3 randomPos = new Vector3 (Random.Range (TopLeft.x, BottomRight.x), Random.Range (BottomRight.y, TopLeft.y), 0);
		int tries = 0;
		while ((randomPos - player.transform.position).sqrMagnitude < 1300 && tries < 100) {
			++tries;
			randomPos = new Vector3 (Random.Range (TopLeft.x, BottomRight.x), Random.Range (BottomRight.y, TopLeft.y), 0);
		}
		if (tries == 100)
			Debug.LogError ("Oh hell");
		
		mainTarget.transform.localPosition = randomPos;
		mainTarget.Init (HostFigureType.Trump, TopLeft, BottomRight);

		targetPointer = transform.GetComponentInChildren<OffscreenPointer>();
		targetPointer.Init (mainTarget.transform, player.transform);
	}

	public void ShowFloatingText(Vector3 origin, string text)
	{
		Text floatingLabel = Instantiate<GameObject> (Resources.Load<GameObject> ("FloatingLabel")).GetComponent<Text>();
		floatingLabel.text = text;
		floatingLabel.rectTransform.position = Camera.main.WorldToScreenPoint (origin);
		floatingLabel.rectTransform.SetParent (canvas.transform, true);
		floatingLabel.rectTransform.localScale = Vector3.one;

		floatingLabel.DOFade (0, 2);
		floatingLabel.rectTransform.DOLocalMoveY (200, 1).SetRelative(true);
		Destroy (floatingLabel.gameObject, 3);

	}
}
