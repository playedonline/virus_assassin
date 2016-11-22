using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LeaderSequence : MonoBehaviour{
    public Image topBG;
    public Image bottomBG;
    public Image image;
    public Text text;

    private Vector3 topBGPos = new Vector3(0, 238, 0);
    private Vector3 bottomBGPos = new Vector3(0, -238, 0);
    private Vector3 imagePos = new Vector3(423, 239, 0);
    private Vector3 textPos = new Vector3(0, -242, 0);
    private Vector3 topBGHiddenPos;
    private Vector3 bottomBGHiddenPos;
    private Vector3 imageHiddenPos;
    private Vector3 textHiddenPos;

    private float typeDelay = 0.05f;
    private float skullDelay = 1f;

    public void Animation(HostFigure hostFigure) {
        topBGHiddenPos = topBG.transform.localPosition;
        bottomBGHiddenPos = bottomBG.transform.localPosition;
        imageHiddenPos = image.transform.localPosition;
        textHiddenPos = text.transform.localPosition;

        DOTween.Sequence().Insert(0,
                Camera.main.DOOrthoSize (4, 0.6f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack)
        ).Insert(0.5f,
                topBG.transform.DOLocalMove(topBGPos, 0.5f)
        ).Insert(0.5f,
                bottomBG.transform.DOLocalMove(bottomBGPos, 0.5f)
        ).Insert(0.9f,
                image.transform.DOLocalMove(imagePos, 0.2f)
        ).Insert(0.9f,
                text.transform.DOLocalMove(textPos, 0.2f)
        ).InsertCallback(2f, () => {
            Camera.main.DOOrthoSize (10, 0.6f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack);
            topBG.transform.DOLocalMove(topBGHiddenPos, 0.5f);
            bottomBG.transform.DOLocalMove(bottomBGHiddenPos, 0.5f);
            image.transform.DOLocalMove(imageHiddenPos, 0.2f);
            text.transform.DOLocalMove(textHiddenPos, 0.2f);
        }).InsertCallback(2.6f, () => {
            Time.timeScale = 1;
        }).SetUpdate(UpdateType.Normal, true);
    }
}
