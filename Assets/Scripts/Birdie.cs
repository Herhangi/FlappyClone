using UnityEngine;

public class Birdie : MonoBehaviour
{
    public AudioClip ScoreClip;
    public AudioClip DeathClip;

    #region Unity Methods
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name.Equals("Scorer"))
        {
            audio.clip = ScoreClip;
            col.collider2D.enabled = false;
            FlappyController.Instance.Score++;

            audio.Play();
        }
        else if(FlappyController.Instance.BirdieCrashed())
        {
            audio.clip = DeathClip;
            audio.Play();
        }
    }
    #endregion
}