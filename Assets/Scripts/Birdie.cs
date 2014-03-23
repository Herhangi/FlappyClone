using UnityEngine;

public class Birdie : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name.Equals("Scorer"))
        {
            col.collider2D.enabled = false;
            FlappyController.Instance.Score++;
        }
        else
            FlappyController.Instance.BirdieCrashed();
    }
}
