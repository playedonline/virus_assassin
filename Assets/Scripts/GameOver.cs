using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    public Text scoreText;
    public Text tapToStart;

    private bool allowRestart = false;

    void Awake() {
        //


    }

    void Update(){
        if(Input.GetMouseButtonDown(0) && allowRestart)
            UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }

	public void Show(bool success)
	{
		gameObject.SetActive (true);

		transform.Find ("text_lose").gameObject.SetActive (!success);
		transform.Find ("text_win").gameObject.SetActive (success);	
		
		DOVirtual.DelayedCall(0.5f, () => {
			allowRestart = true;
			DOTween.Sequence().Append(
				tapToStart.DOFade(1, 0.2f)
			).Append(
				tapToStart.DOFade(1, 0.2f)
			).Append(
				tapToStart.DOFade(0, 0.2f)
			).SetLoops(-1, LoopType.Restart);
		});
	}
}
