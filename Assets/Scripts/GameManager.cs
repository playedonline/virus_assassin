using UnityEngine;

public class GameManager : MonoBehaviour {
    public int regularFiguresAmount = 0;

    private Object m_wandererPrefab;

    public void Init(){
        for(int i = 0 ; i < regularFiguresAmount ; i++){
            GameObject wanderer = Instantiate(m_wandererPrefab) as GameObject;
            wanderer.transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(-9.8f, 9.8f), 0);
        }
    }

    void Awake(){
        m_wandererPrefab = Resources.Load("Wanderer");

        Init();
    }
}
