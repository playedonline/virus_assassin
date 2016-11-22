using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static float SCREEN_WIDTH;
	public static float SCREEN_HEIGHT;
	public static float HORIZONTAL_TILES = 8;
	public static float VERTICAL_TILES = 4;
	public const float comboActiveThreshold = 1.2f;
    public static bool startAnimationShown = true;

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
	private int comboCounter;
	private float comboStartTime;
    public GameOver gameOverScreen;
	public bool isGameOver;
	public Text comboText;
	public CanvasGroup comboCanvasGroup;

    void Awake(){
		GameManager.Instance = this;
		isGameOver = false;
		SCREEN_HEIGHT = Camera.main.orthographicSize * 2;    
		SCREEN_WIDTH = SCREEN_HEIGHT * Screen.width / Screen.height;

		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
         m_hostFigurePrefab = Resources.Load("Soldier");

        Application.targetFrameRate = 60;

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

            for(int i = 0 ; i < Random.Range(1, 2) ; i++)
                SpawnNewSoldier(new Vector3(Random.Range(x - bgSprite.bounds.extents.x, x + bgSprite.bounds.extents.x), Random.Range(y - bgSprite.bounds.extents.y, y + bgSprite.bounds.extents.y), 0));

            x += bgSprite.bounds.size.x;
            if(x > BottomRight.x){
                x = TopLeft.x + bgSprite.bounds.extents.x;
                y -= bgSprite.bounds.size.y;
            }
        }

        Time.timeScale = 0;

        comboText = GameObject.Find ("comboText").GetComponent<Text> ();
        comboCanvasGroup = GameObject.Find ("ComboMeter").GetComponent<CanvasGroup> ();

        if(startAnimationShown)
            StartGame();
        else {
            DisplayStartAnimation();
            startAnimationShown = true;
        }
    }

    void DisplayStartAnimation(){
        GameObject animationGO = Instantiate(Resources.Load("KJStartAnimation")) as GameObject;
        animationGO.transform.parent = canvas.transform;
        animationGO.transform.localPosition = Vector3.zero;
        animationGO.transform.localScale = Vector3.one;
        animationGO.GetComponent<StartAnimation>().Animation();
    }

    public void StartGame(){
        Time.timeScale = 1;
        player = (Instantiate(Resources.Load("Virus")) as GameObject).GetComponent<Virus> ();
		Camera.main.GetComponent<SmoothCameraFollow> ().target = player.transform;
        SpawnNewTarget ();
    }

	void Update()
	{
		if (isGameOver)
			return;
		
		scoreText.text = score.ToString ();

        if(hostFigures.Count < 14 && Random.value < 0.01)
            ReSpawnSoldier();

		if (Time.time - comboStartTime > comboActiveThreshold && comboCounter > 0) {
			// combo broken
			if (comboCounter > 1)
				ShowFloatingText (player.transform.position + Vector3.up * 2, "COMBO BROKEN" , 0.8f, false, true);
			score += comboCounter;
			comboCounter = 0;
		}
	}

    public void OnHostFigureDie(HostFigure hf){
        hostFigures.Remove(hf);
    }

    public void OnHostFigureInfected(HostFigure hf){
        if (hf.hostType == HostFigureType.Trump) {
            score += 10;
            SpawnNewTarget ();
        }

        UpdateComboCounter (hf.transform.position + Vector3.up * 3f);
        if (comboCounter % 5 == 0 && comboCounter > 0)
            ShowFloatingText (player.transform.position + Vector3.up * 3, "AWESOME! +" + 5, 0.8f, false, true);
    }

    private void UpdateComboCounter(Vector3 newPosition)
    {
        comboStartTime = Time.time;
        comboCounter += 1;
        comboText.text = comboCounter.ToString ();
        comboCanvasGroup.alpha = 1;
        DOTween.Kill (comboCanvasGroup);
        comboCanvasGroup.DOFade (0, 0.4f).SetDelay(0.8f);

        comboCanvasGroup.transform.DOPunchScale (Vector3.one * 0.3f, 0.5f);
        if (comboCounter == 1)
            comboCanvasGroup.transform.position = newPosition;
        else
            comboCanvasGroup.transform.DOMove (newPosition, 0.2f).SetEase (Ease.OutSine);
        //ShowFloatingText(hf.transform.position, comboCounter.ToString(), 1, true);

    }



    public void OnVirusDie(){
        targetPointer.Init (null, null);
		gameOverScreen.gameObject.SetActive(true);
		isGameOver = true;
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

		targetPointer = transform.GetComponentInChildren<OffscreenPointer>(true);
		targetPointer.Init (mainTarget.transform, player.transform);
	}

	public void ShowFloatingText(Vector3 origin, string text, float scaleFactor = 1, bool punch = false, bool rotate = false)
	{
		Text floatingLabel = Instantiate<GameObject> (Resources.Load<GameObject> ("FloatingLabel")).GetComponent<Text>();
		floatingLabel.text = text;
		floatingLabel.transform.position = origin;
		floatingLabel.transform.SetParent (canvas.transform, true);
		floatingLabel.transform.localScale = Vector3.one * scaleFactor;

		//floatingLabel.transform.localEulerAngles = new Vector3 (0, 0, 5);
		floatingLabel.transform.localEulerAngles = new Vector3 (0, 0, 0);

		if (rotate)
			floatingLabel.transform.DOLocalRotate (new Vector3 (0, 0, 5 * (Random.value < 0.5f ? 1 : -1)), 0.3f).SetEase(Ease.OutBack);
		
		if (punch)
			floatingLabel.transform.DOPunchScale (Vector3.one * 0.3f, 1);
		
		floatingLabel.DOFade (0f, 1.5f);
		floatingLabel.transform.DOLocalMoveY (80, 1).SetRelative(true);
		Destroy (floatingLabel.gameObject, 3);

	}
}
