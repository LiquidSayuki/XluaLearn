using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    public DTBrain animateController;
    public GameObject dungeonMode;
    public GameObject unlimitedMode;

    private int gameMode;
    private void Update()
    {

    }

    public void OnDungeonModeClicked()
    {
        gameMode = 0;
        StartCoroutine("IOnGameModeSelect");
    }

    public void OnUnlimitedModeClicked()
    {
        gameMode = 1;
        StartCoroutine("IOnGameModeSelect");
    }

    IEnumerator IOnGameModeSelect()
    {
        if (this.gameMode == 0)
        {
            animateController.MoveToCenter(dungeonMode);
            yield return null;
            animateController.MoveOut(unlimitedMode);
            animateController.MoveUp();
            yield return new WaitForSeconds(2);
            animateController.BigAndDark(dungeonMode);
            yield return new WaitForSeconds(1);
            GameManager.Instance.LoadMainGameScene(0);
        }
        else if(this.gameMode == 1)
        {
            animateController.MoveToCenter(unlimitedMode);
            yield return null;
            animateController.MoveOut(dungeonMode);
            animateController.MoveUp();
            yield return new WaitForSeconds(2);
            animateController.BigAndDark(unlimitedMode);
            yield return new WaitForSeconds(1);
            GameManager.Instance.LoadMainGameScene(1);
        }
        
    }
}
