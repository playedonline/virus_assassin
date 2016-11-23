using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LeaderSequence : MonoBehaviour {
    public Image topBG;
    public Image bottomBG;
    public Image image;
    public Text text;
    public SpriteRenderer mac;
    public GameObject smokePS;

    private Vector3 topBGPos = new Vector3(0, 271, 0);
    private Vector3 bottomBGPos = new Vector3(0, -263, 0);
    private Vector3 imagePos = new Vector3(419, 278, 0);
    private Vector3 textPos = new Vector3(0, -259, 0);
    private Vector3 topBGHiddenPos;
    private Vector3 bottomBGHiddenPos;
    private Vector3 imageHiddenPos;
    private Vector3 textHiddenPos;

    private float typeDelay = 0.05f;
    private float skullDelay = 1f;

	public void Animation(HostFigure hostFigure, System.Action callback) {
        topBGHiddenPos = topBG.transform.localPosition;
        bottomBGHiddenPos = bottomBG.transform.localPosition;
        imageHiddenPos = image.transform.localPosition;
        textHiddenPos = text.transform.localPosition;

        DOTween.Kill(Camera.main, false);

        GameManager.Instance.scoreText.gameObject.SetActive(false);
        GameManager.Instance.comboText.transform.parent.gameObject.SetActive(false);
        GameManager.Instance.targetPointer.gameObject.SetActive(false);

        mac.transform.parent = null;
        mac.transform.localScale = Vector3.one;
        mac.transform.localPosition = new Vector3(hostFigure.transform.localPosition.x, hostFigure.transform.localPosition.y + GameManager.SCREEN_HEIGHT);

        smokePS.transform.parent = null;
        smokePS.transform.localPosition = new Vector3(hostFigure.transform.localPosition.x, hostFigure.transform.localPosition.y - 3);


        DOTween.Sequence().Insert(0f,
                DOTween.To(value => Time.timeScale = value, 1, 0, 0.3f).SetEase(Ease.InCubic)
        ).InsertCallback(0.4f, () => {
            DOTween.Kill(Camera.main.transform, false);
            Camera.main.transform.DOLocalMove(new Vector3(hostFigure.transform.localPosition.x, hostFigure.transform.localPosition.y, Camera.main.transform.localPosition.z), 0.3f).SetUpdate(UpdateType.Normal, true);
        }).InsertCallback(0.8f, () => {
            DOTween.Kill(Camera.main, false);
            Camera.main.DOOrthoSize(6, 0.6f).SetEase(Ease.OutBack).SetUpdate(UpdateType.Normal, true);
        }).Insert(1.1f,
                topBG.transform.DOLocalMove(topBGPos, 0.3f).SetEase(Ease.InExpo)
        ).Insert(1.1f,
                bottomBG.transform.DOLocalMove(bottomBGPos, 0.3f).SetEase(Ease.InExpo)
        ).Insert(1.3f,
                image.transform.DOLocalMove(imagePos, 0.3f).SetEase(Ease.InExpo)
        ).Insert(1.3f,
                text.transform.DOLocalMove(textPos, 0.3f).SetEase(Ease.InExpo)
        ).Insert(2.1f,
                mac.transform.DOLocalMoveY(hostFigure.transform.localPosition.y, 0.3f).SetEase(Ease.InExpo).OnComplete(() => {
                    Camera.main.DOShakePosition(0.5f, Vector3.down).SetUpdate(UpdateType.Normal, true);
                    hostFigure.SetHostType(HostFigureType.TrumpMAC);
                    smokePS.gameObject.SetActive(true);
                    Destroy(mac.gameObject);
                    Destroy(smokePS.gameObject, 1);
                })
        ).Insert(2.2f,
                Camera.main.DOOrthoSize(10, 0.4f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack)
        ).InsertCallback(3.8f, () => {
            topBG.transform.DOLocalMove(topBGHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            bottomBG.transform.DOLocalMove(bottomBGHiddenPos, 0.4f).SetUpdate(UpdateType.Normal, true);
            image.transform.DOLocalMove(imageHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            text.transform.DOLocalMove(textHiddenPos, 0.4f).SetUpdate(UpdateType.Normal, true);
        }).InsertCallback(4.3f, () => {
            DOTween.To(value => Time.timeScale = value, 0, 1, 0.3f).SetEase(Ease.InCubic).SetUpdate(UpdateType.Normal, true);
            GameManager.Instance.scoreText.gameObject.SetActive(true);
            GameManager.Instance.comboText.transform.parent.gameObject.SetActive(true);
            GameManager.Instance.targetPointer.gameObject.SetActive(true);
			if (callback != null);
				callback();

            Destroy(gameObject);
        }).SetUpdate(UpdateType.Normal, true);
    }

    void Update(){
        if(smokePS.gameObject.activeSelf)
            smokePS.GetComponent<ParticleSystem>().Simulate(Time.unscaledDeltaTime, true, false);
    }
}
