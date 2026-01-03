using UnityEngine;
public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject fireballObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireballObject.GetComponent<Renderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DebugStartGesture()
    {
        Debug.Log("Started");

        if (fireballObject != null)
        {
            fireballObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void DebugEndGesture()
    {
        Debug.Log("Ended");

        if (fireballObject != null)
        {
            fireballObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}
