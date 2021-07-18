using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : MonoBehaviour
{
    public static Chopper Instance;
    public bool IsPlayerCollied = false;
    private float ChopperFanSpeed = 5f;
    [SerializeField] GameObject ConfetiEffect;
    [SerializeField] GameObject m_Camera;

    public void Awake()
    {
        Instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerCollied) return;
        /*if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            return;
        }  */      
        if (other.gameObject.CompareTag("Player"))
        {
            IsPlayerCollied = true;
            var Confettieffect = Instantiate(ConfetiEffect, transform);
            Confettieffect.transform.parent = GameObject.Find("Main Camera").gameObject.transform;
            //Confettieffect.transform.parent = m_Camera.transform;//Latest Camera
            Confettieffect.transform.localPosition = new Vector3(0f, 0.2f, 2f);
            DOVirtual.DelayedCall(0.5f, () => ReadyToFly());
            DOVirtual.DelayedCall(0.2f, () => ChopperFanSpeed = 15f);            
        }
    }
    private void Update()
    {
        if (!IsPlayerCollied) return;
        ChopperFanOn();
    }

    [Button]
    void ChopperFanOn()
    {
        transform.GetChild(1).Rotate(Vector3.down * ChopperFanSpeed);
    }

    [Button]
    void ReadyToFly()
    {
        Player.Instance.IsInOtherObject = true;
        Player.Instance.transform.SetParent(transform.GetChild(2));
        Player.Instance.transform.localPosition = Vector3.zero;
        Player.Instance.transform.DOLocalMove(Vector3.zero, 0.1f).OnComplete(() =>
        {
            Player.Instance.m_Animator.SetBool("Seatdown", true);
            Player.Instance.m_Animator.SetBool("Jump", false);
            Player.Instance.m_Animator.SetBool("Run", false);
            Player.Instance.transform.localEulerAngles = new Vector3(0, 180, 0);
            Player.Instance.GetComponent<Rigidbody>().useGravity = false;
            Player.Instance.GetComponent<Rigidbody>().isKinematic = true;
            //transform.GetChild(2).gameObject.transform.SetParent(Player.Instance.transform);
            //yield return new WaitForSeconds(1f);
            transform.DORotate(Vector3.down * -22f, 5f);
            transform.DOMoveY(10f, 5f);
        });
        Gamemanager.Instance.WinGame();
    }
}
