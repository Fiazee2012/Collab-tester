using UnityEngine;

public class groundScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
        Debug.Log("Platform Y Position: " + transform.position.y);
    }
}
