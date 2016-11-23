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
	private static List<HostFigureType> hostFigureTypesShown = new List<HostFigureType>();

	public static GameManager Instance;
	public int score;

    private Object m_hostFigurePrefab;
	public Bounds spawnableArea;
	public HostFigure mainTarget;
    private OffscreenPointer targetPointer;
    public Virus player;
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

        targetPointer = transform.GetComponentInChildren<OffscreenPointer>(true);

        Application.targetFrameRate = 60;

		spawnableArea.min = new Vector3 (-38, -6, 0);
		spawnableArea.max = new Vector3 (38, 44, 0);
		Bounds spawnTileBounds = new Bounds (Vector3.zero, new Vector3 (10.8f, 10.8f, 0));
		float x = spawnableArea.min.x; 
		float y = spawnableArea.min.y; 
        GameObject bg = new GameObject("bg");
		Debug.Log (spawnableArea.min +","+ spawnableArea.max +","+ spawnTileBounds.min +","+ spawnTileBounds.max);
		int tries = 0;
		while(y < spawnableArea.max.y){
			++tries;
			Debug.Log ("spawn!");
            for(int i = 0 ; i < Random.Range(1, 2) ; i++)
				SpawnNewSoldier(new Vector3(Random.Range(x, x + spawnTileBounds.size.x), Random.Range(y, y + spawnTileBounds.size.y), 0));

			x += spawnTileBounds.size.x;
			if(x > spawnableArea.max.x){
				x = spawnableArea.min.x;
				y += spawnTileBounds.size.y;
            }
        }

        if(hostFigureTypesShown.Count == 0)
            hostFigureTypesShown.Add(HostFigureType.Soldier);

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
        Time.timeScale = 0;
        GameObject animationGO = Instantiate(Resources.Load("KJStartAnimation")) as GameObject;
        animationGO.transform.parent = canvas.transform;
        animationGO.transform.localPosition = Vector3.zero;
        animationGO.transform.localScale = Vector3.one;
        animationGO.GetComponent<StartAnimation>().Animation();
    }

    void DisplayLeaderSequence(){
        GameObject animationGO = Instantiate(Resources.Load("LeaderSequence")) as GameObject;
        animationGO.transform.parent = canvas.transform;
        animationGO.transform.localPosition = Vector3.zero;
        animationGO.transform.localScale = Vector3.one;
        animationGO.GetComponent<LeaderSequence>().Animation(mainTarget);
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

        if(player != null && mainTarget != null && hostFigureTypesShown.IndexOf(mainTarget.hostType) == -1 && Vector3.Distance(player.transform.localPosition, mainTarget.transform.localPosition) < 9){
            hostFigureTypesShown.Add(mainTarget.hostType);
            DisplayLeaderSequence();
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
			Vector3 pos = new Vector3(Random.Range(-spawnableArea.extents.x, spawnableArea.extents.x), Random.Range(-spawnableArea.extents.y, spawnableArea.extents.y), 0);

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
		Debug.Log (position);
        GameObject hostFigureGO = Instantiate(m_hostFigurePrefab) as GameObject;
        hostFigureGO.transform.localPosition = position;
        HostFigure hostFigure = hostFigureGO.GetComponent<HostFigure>();
        hostFigure.Init(HostFigureType.Soldier);
        hostFigures.Add(hostFigure);
    }

	public void SpawnNewTarget()
	{		
		mainTarget = Instantiate<GameObject>(Resources.Load<GameObject>("Soldier")).GetComponent<HostFigure>();
		mainTarget.name = "Trump";
		Vector3 randomPos = new Vector3 (Random.Range (spawnableArea.min.x, spawnableArea.max.x), Random.Range (spawnableArea.min.y, spawnableArea.min.y), 0);
		int tries = 0;
		while ((randomPos - player.transform.position).sqrMagnitude < 1300 && tries < 100) {
			++tries;
			randomPos = new Vector3 (Random.Range (spawnableArea.min.x, spawnableArea.max.x), Random.Range (spawnableArea.min.y, spawnableArea.min.y), 0);
		}
		if (tries == 100)
			Debug.LogError ("Oh hell");
		
		mainTarget.transform.localPosition = randomPos;
		mainTarget.Init (HostFigureType.Trump);

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
