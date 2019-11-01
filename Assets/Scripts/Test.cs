using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject FirePos;

    private void Awake() 
    {
        Debug.Log("Gun position(world)" + this.gameObject.transform.position);
        Debug.Log("Gun position(local)" + this.gameObject.transform.localPosition);
        Debug.Log("FirePos position(world)" + FirePos.transform.position);
        Debug.Log("FirePos position(local)" + FirePos.transform.localPosition);

        Debug.Log("diff angle (world x and local x) " + Vector3.Angle(Vector3.right, this.transform.right));
        Debug.Log("diff angle (world x and local z) " + Vector3.Angle(Vector3.right, this.transform.forward));
    }
    

    private void Update() 
    {
        
    }
}
