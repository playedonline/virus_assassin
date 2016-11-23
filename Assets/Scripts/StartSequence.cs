using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartSequence : MonoBehaviour{
    public Image blackBG;
    public Image kjBig;
    public Image bubble;
    public Text bubbleText;
    public Image[] skulls;

    private Vector3 blackBGPos = new Vector3(0, -238, 0);
    private Vector3 kjBigPos = new Vector3(-415, -217, 0);
    private Vector3 bubblePos = new Vector3(-129, -225, 0);
    private Vector3 blackBGHiddenPos;
    private Vector3 kjBigHiddenPos;
    private Vector3 bubbleHiddenPos;
    private string text;

    private Dictionary<string, Sprite> KJShikuiSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> KJSpeakSprites = new Dictionary<string, Sprite>();

    private float typeDelay = 0.02f;
    private float skullDelay = 0.6f;

    public void Animation() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("KJShikuiSprites");
        for (int i = 0; i < sprites.Length; i++) {
            Sprite s = sprites[i];
            KJShikuiSprites.Add(s.name, s);
        }

        sprites = Resources.LoadAll<Sprite>("KJSpeakSprites");
        for (int i = 0; i < sprites.Length; i++) {
            Sprite s = sprites[i];
            KJSpeakSprites.Add(s.name, s);
        }

        text = bubbleText.text;
        bubbleText.text = "";
        bubble.transform.localScale = new Vector3(0, 0, 1);

        blackBGHiddenPos = blackBG.transform.localPosition;
        kjBigHiddenPos = kjBig.transform.localPosition;
        bubbleHiddenPos = bubble.transform.localPosition;

        GameObject kjFigureGO = Instantiate(Resources.Load("KJUtemp")) as GameObject;
        SpriteRenderer whiteGrad = kjFigureGO.transform.Find("WhiteGradient").GetComponent<SpriteRenderer>();
        SpriteRenderer kjImage = kjFigureGO.transform.Find("Shikui").GetComponent<SpriteRenderer>();
        Vector3 whiteGradHiddenPos = whiteGrad.transform.localPosition;
        Vector3 kjImageHiddenPos = kjImage.transform.localPosition;

        whiteGrad.DOFade(0, 0).SetUpdate(UpdateType.Normal, true);

        DOTween.Sequence().Insert(0,
                whiteGrad.transform.DOLocalMoveY(5.57f, 1).SetEase(Ease.Linear)
        ).Insert(0,
                whiteGrad.DOFade(1, 1.4f)
        ).Insert(0,
                whiteGrad.transform.DOLocalMoveY(5.57f, 1).SetEase(Ease.Linear)
        ).Insert(1f,
                whiteGrad.transform.DOScaleX(60, 0.5f).SetEase(Ease.OutBack)
        ).Insert(1.4f,
                kjImage.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear)
        ).Insert(1.6f,
                kjImage.transform.DOScaleX(1, 0.5f).SetEase(Ease.OutBack)
        ).Insert(2f,
                Camera.main.DOOrthoSize (4, 0.6f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack)
        ).Insert(2f,
                whiteGrad.DOFade(0, 0.4f).OnComplete(() => {
                    whiteGrad.transform.localPosition = whiteGradHiddenPos;
                    whiteGrad.transform.DOScaleX(10, 0).SetUpdate(UpdateType.Normal, true);
                })
        ).Insert(2.6f,
                blackBG.transform.DOLocalMove(blackBGPos, 0.4f).SetEase(Ease.InExpo)
        ).Insert(2.8f,
            kjBig.transform.DOLocalMove(kjBigPos, 0.6f).SetEase(Ease.InExpo)
        ).Insert(3.3f,
                bubble.transform.DOLocalMove(bubblePos, 0.3f).SetEase(Ease.InExpo)
        ).Insert(3.4f,
                bubble.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InExpo)
        ).InsertCallback(3.7f,
            TypeAnimation
        ).InsertCallback(5f, () => {
            kjImage.sprite = KJShikuiSprites["shikui01"];
        }).InsertCallback(5.5f, () => {
            kjImage.sprite = KJShikuiSprites["shikui02"];
        }).InsertCallback(6f, () => {
            kjImage.sprite = KJShikuiSprites["shikui03"];
        }).InsertCallback(6.5f, () => {
            kjImage.sprite = KJShikuiSprites["shikui04"];
        }).InsertCallback(7f, () => {
            kjImage.sprite = KJShikuiSprites["shikui05"];
        }).InsertCallback(7.5f, () => {
            GameManager.Instance.StartGame();
        }).InsertCallback(8f, () => {
            kjImage.sprite = KJShikuiSprites["shikui01"];
        }).InsertCallback(5f + TypeAnimationDuration(), () => {
            Camera.main.DOOrthoSize (10, 0.6f).SetUpdate(UpdateType.Normal, true);
            blackBG.transform.DOLocalMove(blackBGHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            kjBig.transform.DOLocalMove(kjBigHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            bubble.transform.DOLocalMove(bubbleHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            bubble.transform.DOScale(Vector3.zero, 0.4f).SetUpdate(UpdateType.Normal, true);
        }).Insert(5.3f + TypeAnimationDuration(),
                whiteGrad.transform.DOLocalMoveY(5.57f, 0.25f).SetEase(Ease.Linear)
        ).Insert(5.3f,
                whiteGrad.DOFade(1, 0.35f)
        ).Insert(5.55f + TypeAnimationDuration(),
                whiteGrad.transform.DOScaleX(60, 0.125f).SetEase(Ease.Linear)
        ).Insert(5.65f + TypeAnimationDuration(),
                kjImage.transform.DOScaleX(0.1f, 0.125f).SetEase(Ease.OutBack)
        ).Insert(5.75f + TypeAnimationDuration(),
                kjImage.transform.DOLocalMoveY(kjImageHiddenPos.y, 0.125f).SetEase(Ease.OutBack)
        ).Insert(5.85f + TypeAnimationDuration(),
                whiteGrad.DOFade(0, 0.1f)
        ).InsertCallback(6.4f + TypeAnimationDuration(), () => {
            Destroy(kjFigureGO);
            Destroy(this.gameObject);
            Time.timeScale = 1;
        }).SetUpdate(UpdateType.Normal, true);
    }

    private float TypeAnimationDuration(){
        return typeDelay * text.Replace(" ", "").Length + skullDelay * (skulls.Length + 1);
    }

    private void TypeAnimation(){
        Sequence typeSequence = DOTween.Sequence();
        int tweensAmount = 0;
        for(int i = 0 ; i < text.Length - 1 ; i++){
            string currentText = text.Substring(0, i + 1);
            if(!currentText.EndsWith(" ")) {
                int frameIndex = tweensAmount % 3 + 1;
                typeSequence.InsertCallback(typeDelay * tweensAmount, () => {
                    kjBig.sprite = KJSpeakSprites["pe0" + frameIndex.ToString()];
                    bubbleText.text = currentText;
                });
                tweensAmount += 1;
            }
        }

        int skullsAmount = 0;
        foreach(Image skull in skulls){
            Image skullTemp = skull;
            typeSequence.InsertCallback(typeDelay * tweensAmount + skullDelay * (skullsAmount + 1), () => {
                skullTemp.gameObject.SetActive(true);
            });
            skullsAmount += 1;
        }

        typeSequence.SetUpdate(UpdateType.Normal, true);
    }
}
