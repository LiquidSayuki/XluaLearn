using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    Image progressValue;
    [SerializeField]
    TMP_Text progressText;
    [SerializeField]
    GameObject progressBar;
    [SerializeField]
    TMP_Text progressDesc;

    float m_Max;

    public void InitProgress(float max, string desc)
    {
        m_Max = max;
        progressBar.SetActive(true);
        progressDesc.text = desc;
        progressValue.fillAmount = max > 0 ? 0 : 100;
        progressText.gameObject.SetActive(max > 0);
    }

    public void UpdateProgress(float progress)
    {
        progressValue.fillAmount = progress / m_Max;
        progressText.text = string.Format("{0:0}%", progressValue.fillAmount * 100);
    }
}
