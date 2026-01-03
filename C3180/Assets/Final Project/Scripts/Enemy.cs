using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void Highlight()
    {
        rend.material.color = Color.red;
    }

    public void Unhighlight()
    {
        rend.material.color = originalColor;
    }
}