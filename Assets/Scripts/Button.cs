using System.Collections;
using UnityEngine;

public abstract class Button : MonoBehaviour
{
    public float ScaleFactor = 1.1f;

    public abstract void OnClick();

    public void Touched()
    {
        StartCoroutine(Scale());
    }
    IEnumerator Scale()
    {
        for (float i = 0; i < 1; i += 0.1f)
        {
            transform.localScale = Vector3.one*Mathf.Lerp(1, ScaleFactor, i);
            yield return new WaitForEndOfFrame();
        }
    }
    protected virtual void OnEnable()
    {
        transform.localScale = Vector3.one;
    }
}
