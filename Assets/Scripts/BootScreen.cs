using DG.Tweening;
using UnityEngine;

public class BootScreen : MonoBehaviour {
    public SpriteRenderer splash;
    public SpriteRenderer startBtn;

    void Awake() {
        DOVirtual.DelayedCall(0.5f, () => {
            startBtn.gameObject.SetActive(true);

            DOTween.Sequence().Append(
                    startBtn.DOFade(1, 0.2f)
            ).Append(
                    startBtn.DOFade(1, 0.2f)
            ).Append(
                    startBtn.DOFade(0, 0.2f)
            ).SetLoops(-1, LoopType.Restart);
        });
    }

    void Update(){
        if(Input.GetMouseButtonDown(0) && startBtn.gameObject.activeSelf)
            UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }
}
