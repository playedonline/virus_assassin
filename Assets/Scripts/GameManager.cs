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
    private bool leaderAnimationShown = false;
    private List<GameObject> floatingLabels = new List<GameObject>();

	public static GameManager Instance;
	public int score;

    private Object m_hostFigurePrefab;
	public Bounds spawnableArea;
	public HostFigure mainTarget;
    public OffscreenPointer targetPointer;
    public Virus player;
	public Text scoreText;
	private Canvas canvas;
    public Sprite bgSprite;
    private List<HostFigure> hostFigures = new List<HostFigure>();
	public int comboCounter;
	private float comboStartTime;
    public GameOver gameOverScreen;
	public bool isGameOver;
	public Text comboText;
	public CanvasGroup comboCanvasGroup;
	public const int powerMax = 5;
    void Awake(){
		GameManager.Instance = this;
		isGameOver = false;
		SCREEN_HEIGHT = Camera.main.orthographicSize * 2;    
		SCREEN_WIDTH = SCREEN_HEIGHT * Screen.width / Screen.height;

		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        m_hostFigurePrefab = Resources.Load("Soldier");
		        
        Application.targetFrameRate = 60;

		spawnableArea.min = new Vector3 (-38, -6, 0);
		spawnableArea.max = new Vector3 (38, 44, 0);
		Bounds spawnTileBounds = new Bounds (Vector3.zero, new Vector3 (10.8f, 10.8f, 0));
		float x = spawnableArea.min.x; 
		float y = spawnableArea.min.y;
		Debug.Log (spawnableArea.min +","+ spawnableArea.max +","+ spawnTileBounds.min +","+ spawnTileBounds.max);
		int tries = 0;
		while(y < spawnableArea.max.y){
			++tries;
            for(int i = 0 ; i < Random.Range(1, 2) ; i++) {
                Vector3 pos;
                for(int j = 0 ; i < 100 ; i++){
                    pos = new Vector3(Random.Range(x, x + spawnTileBounds.size.x), Random.Range(y, y + spawnTileBounds.size.y));

                    // if far enough from player place
                    if(Vector3.Distance(pos, Vector3.zero) > 3) {
                        SpawnNewSoldier(pos);
                        break;
                    }
                }
            }

			x += spawnTileBounds.size.x;
			if(x > spawnableArea.max.x){
				x = spawnableArea.min.x;
				y += spawnTileBounds.size.y;
            }
        }

        if(startAnimationShown)
            StartGame();
        else {
            DisplayStartSequence();
            startAnimationShown = true;
        }
    }

    void DisplayStartSequence(){
        Time.timeScale = 0;
        GameObject animationGO = Instantiate(Resources.Load("KJStartAnimation")) as GameObject;
        animationGO.transform.parent = canvas.transform;
        animationGO.transform.localPosition = Vector3.zero;
        animationGO.transform.localScale = Vector3.one;
        animationGO.GetComponent<StartSequence>().Animation();
    }

    void DisplayLeaderSequence(){
        GameObject animationGO = Instantiate(Resources.Load("LeaderSequence")) as GameObject;
        animationGO.transform.parent = canvas.transform;
        animationGO.transform.localPosition = Vector3.zero;
        animationGO.transform.localScale = Vector3.one;
		animationGO.GetComponent<LeaderSequence>().Animation(mainTarget, ShowBossUI);

    }

	void ShowBossUI()
	{
		GameObject bossBar = GameObject.Find ("BossBar");
		bossBar.GetComponent<CanvasGroup> ().DOFade (1, 0.5f);
		mainTarget.SetToBossMode (bossBar.GetComponentInChildren<Healthbar> ());
	}

	void BossPhaseEnd()
	{
		GameObject bossBar = GameObject.Find ("BossBar");
		bossBar.GetComponent<CanvasGroup> ().DOFade (0, 0.5f);
		mainTarget.RevertBossMode ();
	}

    public void StartGame(){
        player = (Instantiate(Resources.Load("Virus")) as GameObject).GetComponent<Virus> ();
		Camera.main.GetComponent<SmoothCameraFollow> ().target = player.transform;
        player.transform.localPosition = new Vector3(-1.7f, -1.2f);
        //SpawnNewTarget ();
    }

	void Update()
	{
		if (isGameOver)
			return;
		
		scoreText.text = score.ToString () + " / " + powerMax;

		if (hostFigures.Count < 30)
            ReSpawnSoldier();

//		if (Time.time - comboStartTime > comboActiveThreshold && comboCounter > 0) {
//			// combo broken
//			if (comboCounter > 1)
//				ShowFloatingText (player.transform.position + Vector3.up * 2, "COMBO BROKEN" , 0.8f, false, true);
//			//score += comboCounter;
//			comboCounter = 0;
//		}

//        if(player != null && mainTarget != null && !leaderAnimationShown && Vector3.Distance(player.transform.localPosition, mainTarget.transform.localPosition) < 9){
//            leaderAnimationShown = true;
//            DisplayLeaderSequence();
//        }
	}

    public void OnHostFigureDie(HostFigure hf){
        hostFigures.Remove(hf);
		if (hf.wasTrump) {			
			DOVirtual.DelayedCall (1.5f, () => {
				gameOverScreen.Show (true);
			});
		}
		if (hf.isBoss)
			BossPhaseEnd ();		
    }
		
    public void OnHostFigureInfected(HostFigure hf){
		if (hf.hostType == HostFigureType.Trump) {		
			Destroy (player.GetComponent<Flingable> ());	
		} else {
			score += 1;
			if (score <= powerMax)
				ShowFloatingPowerText (hf.transform.position + Vector3.up * 2, score.ToString(), 0.8f, true, true);

			if (score == powerMax && targetPointer == null) {
				foreach (GameObject floatingLabel in floatingLabels)
                    Destroy(floatingLabel);
				ShowFloatingText(player.transform.position + Vector3.up * 3, "MAX POWER, ATTACK!", 0.8f, false, true, 1, true);
				SpawnNewTarget ();
			} else if (score == powerMax) {
				ShowFloatingText (player.transform.position + Vector3.up * 3,  "MAX POWER, ATTACK!", 0.8f, false,true, 1);
			}

			score = Mathf.Min (score, powerMax);
		}
		
        //UpdateComboCounter (hf.transform.position + Vector3.up * 3f);
//        if (comboCounter % 5 == 0 && comboCounter > 0)
//            ShowFloatingText (player.transform.position + Vector3.up * 3, "AWESOME! +" + 5, 0.8f, false, true);
    }

    private void UpdateComboCounter(Vector3 newPosition)
    {
        comboStartTime = Time.time;
        comboCounter += 1;
        //comboText.text = comboCounter.ToString ();
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
        if(targetPointer != null)
            targetPointer.Init (null, null);
		gameOverScreen.Show (false);
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

		targetPointer = (Instantiate (Resources.Load ("TargetPointer")) as GameObject).GetComponent<OffscreenPointer> ();
		targetPointer.transform.parent = transform;
		
		targetPointer.Init (mainTarget.transform, player.transform);

		leaderAnimationShown = true;
		DisplayLeaderSequence();
	}

	public void ShowFloatingText(Vector3 origin, string text, float scaleFactor = 1, bool punch = false, bool rotate = false, float fadeDelay = 0.3f, bool preventTimeScale = false)
	{
		GameObject floatingLabel = Instantiate<GameObject> (Resources.Load<GameObject> ("FloatingLabel"));
		FloatLabel (floatingLabel,origin,text,scaleFactor,punch,rotate, 0.3f, preventTimeScale);
	}

	public void ShowFloatingPowerText(Vector3 origin, string text, float scaleFactor = 1, bool punch = false, bool rotate = false, bool preventTimeScale = false)
	{
		GameObject floatingLabel = Instantiate<GameObject> (Resources.Load<GameObject> ("FloatingPowerLabel"));
		FloatLabel (floatingLabel,origin,text,scaleFactor,punch,rotate, 0.3f, preventTimeScale);
	}

	void FloatLabel(GameObject floatingLabel, Vector3 origin, string text, float scaleFactor = 1, bool punch = false, bool rotate = false, float fadeDelay = 0.3f, bool preventTimeScale = false)
	{
        floatingLabels.Add(floatingLabel);
		floatingLabel.GetComponentInChildren<Text>().text = text;
		floatingLabel.transform.position = origin;
		floatingLabel.transform.SetParent (canvas.transform, true);
		floatingLabel.transform.localScale = Vector3.one * scaleFactor;

		//floatingLabel.transform.localEulerAngles = new Vector3 (0, 0, 5);
		floatingLabel.transform.localEulerAngles = new Vector3 (0, 0, 0);

		if (rotate)
			floatingLabel.transform.DOLocalRotate (new Vector3 (0, 0, 5 * (Random.value < 0.5f ? 1 : -1)), 0.3f).SetEase(Ease.OutBack).SetUpdate(UpdateType.Normal, preventTimeScale);

		if (punch)
			floatingLabel.transform.DOPunchScale (Vector3.one * 0.3f, 1).SetUpdate(UpdateType.Normal, preventTimeScale);

		floatingLabel.GetComponent<CanvasGroup>().DOFade (0f, 1.5f).SetDelay(fadeDelay).SetUpdate(UpdateType.Normal, preventTimeScale);
		floatingLabel.transform.DOLocalMoveY (80, 1).SetRelative(true).SetUpdate(UpdateType.Normal, preventTimeScale);
		Destroy (floatingLabel.gameObject, 3 + fadeDelay);
        DOVirtual.DelayedCall(3, () => {
            floatingLabels.Remove(floatingLabel);
        }).SetUpdate(UpdateType.Normal, preventTimeScale);;
	}
}
