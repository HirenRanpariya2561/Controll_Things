using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamFollow : MonoBehaviour
{
    public static SimpleCamFollow instance;
    [SerializeField] internal Transform m_Target;
    [SerializeField] private Vector3 m_Padding;
    [SerializeField] private float m_FollowSpeed;
    [SerializeField]float Temp_y;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject.Find("Canvas").gameObject.GetComponent<Gamemanager>().Start();
        SetcamerastartPos();
        //m_Target = GameObject.FindGameObjectWithTag("Player").transform.parent.parent;
        //transform.position = m_Target.position + m_Padding;
    }

    private void Update()
    {
        if (!m_Target) return;
        Vector3 pos = Vector3.MoveTowards(transform.position, m_Target.position + m_Padding, Time.deltaTime * m_FollowSpeed);                      
        pos.y = Temp_y;//comment Latest
        transform.position = pos;
    }
    public void SetcamerastartPos()
    {
        Debug.Log("Set camera start pos ");
        // m_Target = GameObject.FindGameObjectWithTag("Player").transform.parent.parent;
        m_Padding = Gamemanager.Instance.Level.gameObject.transform.GetChild(1).gameObject.transform.localPosition;
        m_Target = Gamemanager.Instance.Level.transform.Find("Player");
        transform.position = m_Target.position + m_Padding;
        Temp_y = transform.position.y;
    }
}
    