using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour {
    public const float SCREEN_WIDTH = 10.8f;
    public const float SCREEN_HEIGHT = 19.2f;
    public const float HORIZONTAL_TILES = 2;
    public const float VERTICAL_TILES = 4;
    public int regularFiguresAmount = 0;

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

    void Awake(){
		GameManager.Instance = this;

		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        regularFiguresAmount = 45;
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
