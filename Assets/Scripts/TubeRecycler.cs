using UnityEngine;

public class TubeRecycler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        FlappyController.Instance.RecycleTube(collision.transform.parent);
    }
}
