using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading;

public class DTBrain : MonoBehaviour
{
    public Image logo;
    public Image blackImage;
    Sequence sequence;

    private void Start()
    {
        sequence = DOTween.Sequence();
        sequence.Append(logo.transform.DORotate(new Vector3(0, 0, 10), 2));
        sequence.Append(logo.transform.DORotate(new Vector3(0, 0, -10), 4));
        sequence.SetLoops(-1, LoopType.Yoyo);

        logo.DOFade(0.5f, 1.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void MoveToCenter(GameObject go)
    {
        go.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 2f);
    }
    public void MoveOut(GameObject go)
    {
        Sequence quence = DOTween.Sequence();
        
        quence.Append(go.transform.DOMove(new Vector3(0, Screen.height / 8, 0), 0.2f).SetRelative());
        quence.Append(go.transform.DOMove(new Vector3(0, -Screen.height, 0), 1f).SetRelative());
        quence.Join(go.transform.DORotate(new Vector3(0,180,0), 0.2f).SetLoops(10, LoopType.Incremental));

    }

    public void MoveUp()
    {
        Sequence quence = DOTween.Sequence();
        quence.Append(logo.transform.DOMove(new Vector3(0, -Screen.height / 8, 0), 0.2f).SetRelative());
        quence.Append(logo.transform.DOMove(new Vector3(0, Screen.height, 0), 1f).SetRelative());
    }

    public void BigAndDark(GameObject go)
    {
        Sequence quence = DOTween.Sequence();
        quence.Append(go.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f));
        quence.Append(go.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
        quence.Append(blackImage.DOFade(1f, 1f));
    }
}
