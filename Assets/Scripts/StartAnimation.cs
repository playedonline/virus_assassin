using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StartAnimation : MonoBehaviour{
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

    private float typeDelay = 0.05f;
    private float skullDelay = 1f;

    public void Animation() {
        text = bubbleText.text;
        bubbleText.text = "";
        bubble.transform.localScale = new Vector3(0, 0, 1);

        blackBGHiddenPos = blackBG.transform.localPosition;
        kjBigHiddenPos = kjBig.transform.localPosition;
        bubbleHiddenPos = bubble.transform.localPosition;

        GameObject kjFigureGO = Instantiate(Resources.Load("KJUtemp")) as GameObject;

        DOTween.Sequence().Insert(
            1f, Camera.main.DOOrthoSize (4, 0.6f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack)
        ).Insert(
            1.9f, blackBG.transform.DOLocalMove(blackBGPos, 0.6f).SetUpdate(UpdateType.Normal, true)
        ).Insert(
            1.9f, kjBig.transform.DOLocalMove(kjBigPos, 0.6f).SetUpdate(UpdateType.Normal, true)
        ).Insert(
            2.4f, bubble.transform.DOLocalMove(bubblePos, 0.3f).SetUpdate(UpdateType.Normal, true)
        ).Insert(
            2.6f, bubble.transform.DOScale(Vector3.one, 0.1f).SetUpdate(UpdateType.Normal, true)
        ).InsertCallback(2.7f,
            TypeAnimation
        ).InsertCallback(4f + TypeAnimationDuration(), () => {
            Camera.main.DOOrthoSize (10, 1f).SetUpdate(UpdateType.Normal, true);
            blackBG.transform.DOLocalMove(blackBGHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            kjBig.transform.DOLocalMove(kjBigHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            bubble.transform.DOLocalMove(bubbleHiddenPos, 0.5f).SetUpdate(UpdateType.Normal, true);
            bubble.transform.DOScale(Vector3.zero, 0.4f).SetUpdate(UpdateType.Normal, true);
        }).InsertCallback(4.5f + TypeAnimationDuration(), () => {
            Destroy(kjFigureGO);
            Destroy(this.gameObject);
            GameManager.Instance.StartGame();
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
                typeSequence.InsertCallback(typeDelay * tweensAmount, () => {
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
