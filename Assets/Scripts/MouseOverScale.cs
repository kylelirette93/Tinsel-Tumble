using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 scaleIncrease = new Vector3(1.2f, 1.2f, 1.2f); // Scale increase factor
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale; // Store the original scale
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Increase the scale when the mouse enters the object
        transform.localScale = originalScale + scaleIncrease;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset the scale when the mouse exits the object
        transform.localScale = originalScale;
    }
}