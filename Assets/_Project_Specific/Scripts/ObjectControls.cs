using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ObjectControls : MonoBehaviour
{
    Joystick m_joystick;
    float horizontal;
    float vertical;
    Vector3 movementDir;
    internal Player m_PlayerInside;
    [SerializeField] Image m_HpImage;
    private bool IsInDistroyCount;
    [SerializeField] private GameObject ParticleOnDestroy;
    [SerializeField] private Collider m_AttachedCollider;
    [SerializeField] private RectTransform m_CanvasRec;
    private Vector3 HealthBarOffset;
    public Rigidbody m_Rigidbody;

    void Start()
    {
        m_CanvasRec.gameObject.SetActive(true);
        m_joystick = FindObjectOfType<Joystick>();
        HealthBarOffset = Vector3.up * m_AttachedCollider.bounds.max.y;
    }
    [Button]
    void SetCanPos()
    {
        HealthBarOffset = Vector3.up * m_AttachedCollider.bounds.max.y;
        Debug.Log(HealthBarOffset);
        HealthBarOffset.y += 1f;
        m_CanvasRec.position = HealthBarOffset + m_AttachedCollider.bounds.center;
    }

    void Update()
    {
        //Debug.Log("Is destory count is " + IsInDistroyCount);
        if (IsInDistroyCount) return;
        if (true)
        {
            m_CanvasRec.position = HealthBarOffset + m_AttachedCollider.bounds.center;
            // m_CanvasRec.eulerAngles = localrot;
        }
        horizontal = m_joystick.Horizontal;
        vertical = m_joystick.Vertical;

        movementDir = new Vector3(horizontal, 0f, vertical);

        if (movementDir.magnitude > 0)
        {
            m_Rigidbody.AddForce(movementDir * 5, ForceMode.Impulse);
            //m_Rigidbody.AddTorque(movementDir*50, ForceMode.Acceleration);
        }
        m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, 3);

        if (m_PlayerInside != null)
        {
            m_HpImage.fillAmount -= Time.deltaTime * 0.1f;
            m_PlayerInside.transform.position = m_AttachedCollider.bounds.center;
            if (m_HpImage.fillAmount <= 0.02f && !IsInDistroyCount)
            {
                OnDestroyThisThing();
            }
        }
    }
    public void OnDestroyThisThing()
    {   //Die fridge
        if (IsInDistroyCount) return;
        IsInDistroyCount = true;
        m_PlayerInside.OnMoveIntoOther(m_PlayerInside.transform, false, true);
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Instantiate(ParticleOnDestroy, transform.position, Quaternion.identity);
            Destroy(m_CanvasRec.gameObject);
            Destroy(gameObject);
        });
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && m_PlayerInside != null)
        {
            Debug.Log("Player special is ");
            OnDestroyThisThing();                    
        }
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            m_HpImage.fillAmount -= 0.3f;
            if (m_HpImage.fillAmount <= 0.02f && !IsInDistroyCount)
            {
                OnDestroyThisThing();
            }
        }
        if (other.CompareTag("Cylinder") && m_PlayerInside != null)
        {
            OnDestroyThisThing();
        }
        //New added
        //if (other.CompareTag("Boundry") && m_PlayerInside != null)
        //{
        //    //Destory ref object whenever reach boundry
        //    OnDestroyThisThing();
        //}
    }      
}