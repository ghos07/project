using UnityEngine;

public class CreateInstance : MonoBehaviour
{
    public GameObject prefab;
    public void Create()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}