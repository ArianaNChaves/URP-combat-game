using UnityEngine;

public class HandleSword : MonoBehaviour
{
    [SerializeField] private Transform hand;
    [SerializeField] private float _pivotDiference;

    private void LateUpdate()
    {
        this.transform.position = hand.position - hand.up * _pivotDiference;
        this.transform.rotation = hand.rotation;
    }

}
