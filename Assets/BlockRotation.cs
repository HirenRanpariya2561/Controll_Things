using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotation : MonoBehaviour
{
    [SerializeField] Transform m_Target;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(m_Target.position.x,5,m_Target.transform.position.z);
        transform.eulerAngles = Vector3.zero;
    }
}
