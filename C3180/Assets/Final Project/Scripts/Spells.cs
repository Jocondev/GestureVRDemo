using UnityEngine;

public class Spells : MonoBehaviour
{
    private ParticleSystem myParticleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spellComplete()
    {
        myParticleSystem.Play();
    }

}
