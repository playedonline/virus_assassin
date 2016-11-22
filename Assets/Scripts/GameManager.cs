using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public const float SCREEN_WIDTH = 10.8f;
    public const float SCREEN_HEIGHT = 19.2f;
    public const float HORIZONTAL_TILES = 2;
    public const float VERTICAL_TILES = 4;

	public static GameManager Instance;
	public int score;

    private Object m_hostFigurePrefab;
    public Vector3 TopLeft {get {
        return new Vector3(-bgSprite.bounds.size.x * HORIZONTAL_TILES - 0.5f * SCREEN_WIDTH, VERTICAL_TILES * bgSprite.bounds.size.y - 0.5f * SCREEN_HEIGHT);
    }}

    public Vector3 BottomRight {get {
        return new Vector3(bgSprite.bounds.size.x * HORIZONTAL_TILES - 0.5f * SCREEN_WIDTH, -0.5f * SCREEN_HEIGHT);
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

            for(int i = 0 ; i < Random.Range(1,3) ; i++)
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

        if(hostFigures.Count < 10 && Random.value < 0.01)
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
            if(pos.x > Camera.main.transform.position.x - SCREEN_WIDTH * 0.5f && pos.x < Camera.main.transform.position.x + SCREEN_WIDTH * 0.5f && pos.y > Camera.main.transform.position.y - SCREEN_HEIGHT * 0.5f && pos.y < Camera.main.transform.position.y + SCREEN_HEIGHT * 0.5f)
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
		Debug.Log ("Spawn new target");
		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		mainTarget.transform.localPosition = new Vector3(Random.Range(-SCREEN_WIDTH, SCREEN_WIDTH), Random.Range(TopLeft.y * 0.66f, TopLeft.y), 0);
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
