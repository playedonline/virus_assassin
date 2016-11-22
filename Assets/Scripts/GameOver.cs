using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    public Text scoreText;
    public Text tapToStart;
    public Button button;

    private bool allowRestart = false;



    void Awake() {
        scoreText.text = string.Format("SCORE: {0}", GameManager.Instance.score);

        button.onClick.AddListener(() => {
            if(allowRestart)
                UnityEngine.SceneManagement.SceneManager.LoadScene ("main");
        });

		DOVirtual.DelayedCall(0.5f, () => {
            allowRestart = true;
            DOTween.Sequence().Append(
                    tapToStart.DOFade(1, 0.2f)
            ).Append(
                    tapToStart.DOFade(1, 2)
            ).Append(
                    tapToStart.DOFade(0, 0.2f)
            ).SetLoops(-1, LoopType.Restart);
        });
    }
}
