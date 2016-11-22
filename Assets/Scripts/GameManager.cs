using UnityEngine;

public class GameManager : MonoBehaviour {
    public const float SCREEN_WIDTH = 10.8f;
    public const float SCREEN_HEIGHT = 19.2f;
    public const float SCREENS_WIDTH = 1;
    public const float SCREENS_HEIGHT = 1;

    public int regularFiguresAmount = 0;

    private Object m_hostFigurePrefab;

    public Vector3 TopLeft {get {
        return new Vector3(-SCREEN_WIDTH * (SCREENS_WIDTH - 0.5f), SCREEN_HEIGHT * (SCREENS_HEIGHT - 0.5f));
    }}

    public Vector3 BottomRight {get {
        return new Vector3(SCREEN_WIDTH * (SCREENS_WIDTH - 0.5f), -SCREEN_HEIGHT * (SCREENS_HEIGHT - 0.5f));
    }}

    public void Init(){
        Vector3 topLeft = TopLeft;
        Vector3 bottomRight = BottomRight;

        for(int i = 0 ; i < regularFiguresAmount ; i++){
            GameObject hostFigure = Instantiate(m_hostFigurePrefab) as GameObject;
            hostFigure.transform.localPosition = new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y), 0);
            hostFigure.GetComponent<HostFigure>().Init(topLeft, bottomRight);
        }
    }

    void Awake(){
        m_hostFigurePrefab = Resources.Load("SoldierOur");
		Application.targetFrameRate = 60;
        Init();
    }
}
