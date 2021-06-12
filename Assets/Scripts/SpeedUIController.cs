using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUIController : MonoBehaviour
{

    public Sprite fastSpeed;
    public Sprite fastSpeedSelect;
    public Sprite speed;
    public Sprite speedSelect;
    public Sprite stop;
    public Sprite stopSelect;

    public Sprite selection;

    public Image fastImage;
    public Image normalSpeedImage;
    public Image stopImage;
    public Transform selectTrans;

    int throttle;
    public Train train;

    private void Start()
    {
        train.OnThrottleChanged += SetThrottle;
    }

    public void SetThrottle(int newThrottle)
    {
        throttle = newThrottle;

        UnSelectAll();

        switch(throttle)
        {
            case 0:
                stopImage.sprite = stopSelect;
                StartCoroutine(MoveSelection(stopImage.transform.position));
                break;
            case 1:
                normalSpeedImage.sprite = speedSelect;
                StartCoroutine(MoveSelection(normalSpeedImage.transform.position));
                break;
            case 2:
                fastImage.sprite = fastSpeedSelect;
                StartCoroutine(MoveSelection(fastImage.transform.position));
                break;
        }
    }

    void UnSelectAll()
    {
        fastImage.sprite = fastSpeed;
        normalSpeedImage.sprite = speed;
        stopImage.sprite = stop;
    }

    IEnumerator MoveSelection(Vector2 position)
    {
        Vector2 startPos = selectTrans.position;
        int length = 21;
        for (int i = 0; i < length; i++)
        {
            selectTrans.position = Vector2.Lerp(startPos, position, (float)i / (length-1));
            yield return null;
        }
    }
}
