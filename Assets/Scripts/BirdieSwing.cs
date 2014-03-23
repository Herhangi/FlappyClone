using UnityEngine;

public class BirdieSwing : MonoBehaviour
{
    public float Speed;
    public float Border;
    private bool isGoingTop;

    void Update ()
    {
	    if (isGoingTop)
	    {
	        transform.localPosition += Vector3.up*Speed*Time.deltaTime;

	        if (transform.localPosition.y > Border)
	            isGoingTop = false;
	    }
	    else
        {
            transform.localPosition -= Vector3.up * Speed * Time.deltaTime;

            if (transform.localPosition.y < -Border)
                isGoingTop = true;
	    }
	}
}
