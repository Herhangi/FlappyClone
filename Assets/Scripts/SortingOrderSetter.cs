using UnityEngine;

public class SortingOrderSetter : MonoBehaviour
{
    public string SortingLayer;
    public int OrderInLayer;

	void Start ()
	{
        renderer.sortingLayerName = SortingLayer;
        renderer.sortingOrder = OrderInLayer;
        Destroy(this);
	}
}
