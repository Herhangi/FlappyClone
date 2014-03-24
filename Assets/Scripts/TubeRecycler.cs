using UnityEngine;

public class TubeRecycler : MonoBehaviour
{
    #region Unity Methods
    void OnTriggerEnter2D(Collider2D collision)
    {
        FlappyController.Instance.RecycleTube(collision.transform.parent);
    }
    #endregion
}