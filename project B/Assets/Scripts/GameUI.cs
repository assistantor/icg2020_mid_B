using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Text m_MessageText;
    [SerializeField] Text m_ScoreCount;
    int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(Color.white);
        ShowMessage("Game start!");
    }

    // Update is called once per frame
    public void ShowMessage(string msg)
    {
        m_MessageText.text = msg;
    }
    public void ScoreCal(int s)
    {
        CancelInvoke();
        score += s;
        UpdateScore(Color.green);
        Invoke("ResetScoreColor", 0.5f);
    }
    public void ScoreOut()
    {
        CancelInvoke();
        m_ScoreCount.text = string.Format("<color=red>{0}</color>", score);
        Invoke("ResetScoreColor", 0.5f);
    }
    void UpdateScore(Color c)
    {
        if (c == Color.red) m_ScoreCount.text = string.Format("<color=red>{0}</color>", score);
        if (c == Color.green) m_ScoreCount.text = string.Format("<color=green>{0}</color>", score);
        if (c == Color.white) m_ScoreCount.text = string.Format("<color=white>{0}</color>", score);
    }
    void ResetScoreColor()
    {
        m_ScoreCount.text = string.Format("<color=white>{0}</color>", score);
    }
}
