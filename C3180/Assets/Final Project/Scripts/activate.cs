using UnityEngine;
using MyProject.Audio;

public class activate : MonoBehaviour
{
    private Mic mic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mic = FindObjectOfType<Mic>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hi!");
        if (other.CompareTag("VRHand"))
        {
            mic.setSnap();
        }
    }
}
