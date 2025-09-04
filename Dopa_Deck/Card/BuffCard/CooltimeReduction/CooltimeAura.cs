using UnityEngine;

public class CooltimeAura : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.position = InGameManager.Instance.GetPlayerObject().transform.position;
    }
}
