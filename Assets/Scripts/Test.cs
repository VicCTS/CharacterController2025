using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = transform.position = new Vector3(Mathf.Lerp(0, 50, 1f), 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position = new Vector3(Mathf.Lerp(0, 50, 1f * Time.deltaTime), 0, 0);
    }
}
