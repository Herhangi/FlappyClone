using UnityEngine;

public class Birdie : MonoBehaviour
{
    void OnTriggerEnter2D()
    {
        FlappyController.Instance.BirdieCrashed();
    }
}
