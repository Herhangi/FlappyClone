using UnityEngine;

public class BirdieSwing : MonoBehaviour
{
    public float Speed;
    public float Border;
    private bool _isGoingTop;

    #region Unity Methods
    void Update ()
    {
	    if (_isGoingTop)
	    {
	        transform.localPosition += Vector3.up*Speed*Time.deltaTime;

	        if (transform.localPosition.y > Border)
	            _isGoingTop = false;
	    }
	    else
        {
            transform.localPosition -= Vector3.up * Speed * Time.deltaTime;

            if (transform.localPosition.y < -Border)
                _isGoingTop = true;
	    }
    }
    #endregion
}