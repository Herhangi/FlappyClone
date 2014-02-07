public class RestartButton : Button
{
    public override void OnClick()
    {
        FlappyController.Instance.Reset();
    }
}
