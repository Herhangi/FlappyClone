using UnityEngine;

public class SortingOrderSetter : MonoBehaviour
{
    public int OrderInLayer;
    public string SortingLayer;

    #region Unity Methods
    void Start ()
	{
        renderer.sortingLayerName = SortingLayer;
        renderer.sortingOrder = OrderInLayer;
        Destroy(this);
    }
    #endregion
}