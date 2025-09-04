using UnityEngine;

public class Shield : MonoBehaviour
{
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = InGameManager.Instance.GetPlayerObject().transform.position;
    }
}
