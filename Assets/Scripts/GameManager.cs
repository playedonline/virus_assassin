using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour {
	public static float SCREEN_WIDTH ;
	public static float SCREEN_HEIGHT ;
	public static float HORIZONTAL_TILES = 6;
	public static float VERTICAL_TILES = 4;
    public int regularFiguresAmount = 0;

	public static GameManager Instance;
	public int score;

    private Object m_hostFigurePrefab;
    public Vector3 TopLeft {get {
        return new Vector3(-bgSprite.bounds.size.x * HORIZONTAL_TILES / 2, VERTICAL_TILES * bgSprite.bounds.size.y - 0.5f * SCREEN_HEIGHT);
    }}

    public Vector3 BottomRight {get {
		return new Vector3(bgSprite.bounds.size.x * HORIZONTAL_TILES / 2, -0.5f * SCREEN_HEIGHT);
    }}

    public HostFigure mainTarget;
    private OffscreenPointer targetPointer;
    private Virus player;
	public Text scoreText;
	private Canvas canvas;
    public Sprite bgSprite;

    void Awake(){
		GameManager.Instance = this;

		SCREEN_HEIGHT = Camera.main.orthographicSize;    
		SCREEN_WIDTH = SCREEN_HEIGHT * Screen.width / Screen.height;

		Debug.Log (SCREEN_HEIGHT + "," + SCREEN_WIDTH +TopLeft + BottomRight);
		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        regularFiguresAmount = 30;
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

            x += bgSprite.bounds.size.x;
            if(x > BottomRight.x){
                x = TopLeft.x + bgSprite.bounds.extents.x;
                y -= bgSprite.bounds.size.y;
            }
        }

        for(int i = 0 ; i < regularFiguresAmount ; i++){
            GameObject hostFigure = Instantiate(m_hostFigurePrefab) as GameObject;
			hostFigure.transform.localPosition = new Vector3(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y), 0);
			hostFigure.GetComponent<HostFigure>().Init(HostFigureType.Soldier, TopLeft, BottomRight);
        }

		SpawnNewTarget ();
        
    }

	void Update()
	{
		scoreText.text = score.ToString ();
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
