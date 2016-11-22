using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOver : MonoBehaviour {
    public Text scoreText;
    public Text tapToStart;
    public Button button;

    public GameOver Instance;

    void Awake() {
        scoreText.text = string.Format("SCORE: {0}", GameManager.Instance.score);

        DOTween.Sequence().Append(
                tapToStart.DOFade(1, 2)
        ).Append(
                tapToStart.DOFade(0, 0.2f)
        ).Append(
                tapToStart.DOFade(1, 0.2f)
        ).SetLoops(-1, LoopType.Restart);

        button.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene ("main");
        });
    }
}
