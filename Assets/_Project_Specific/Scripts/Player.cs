using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public static Player Instance;

    public bool IsInOtherObject;
    public Transform Fridge;
    public GameObject Visual;

    [Header("Movement Settings")]
    private float movementSpeed = 4.8f;
    private float rotationSpeed = 45;
    float horizontal;
    float vertical;
    bool isInput;
    Vector3 movementDir;
    // Input Controls
    Joystick m_joystick;
    private bool canMove = true;
    [SerializeField] internal Animator m_Animator;
    public float Health = 20f;
    [SerializeField] Image Helthbar;
    [SerializeField] RectTransform m_rectbar;
    [SerializeField] private GameObject ParticleOnDestroy;
    private void Awake()
    {
        /* if (!UIManager.instance)
         {
             SceneManager.LoadScene(0);
         }*/
        Instance = this;
    }
    void Start()
    {
        //m_rectbar.gameObject.SetActive(true);//tempory comment
        m_joystick = FindObjectOfType<Joystick>();
    }
#if UNITY_EDITOR
    private void Update()
    {        
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(Vector3.back * Time.deltaTime);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(Vector3.up, -10);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(Vector3.up, 10);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }
    }
#endif
    void FixedUpdate()
    {
        if (!IsInOtherObject)
        {
            Movement();
        }

    }
    private void OnTriggerEnter(Collider other)
    {               
        if (other.gameObject.layer == 8 && other.gameObject.GetComponent<Ref>().isref)
        {
            var OtherObj = other.GetComponent<ObjectControls>();
            if (OtherObj.m_PlayerInside == this) return;//Comment tempory
            OtherObj.enabled = true;
            OtherObj.m_PlayerInside = this;
            other.GetComponent<MeshRenderer>().material.color = Color.blue;
            other.GetComponent<Collider>().isTrigger = false;
            other.GetComponent<Rigidbody>().useGravity = true;
            Transform ase = other.gameObject.transform;
            
            OnMoveIntoOther(ase, true, false);
            
        }
        if (other.CompareTag("Bullet") && Health > 1)
        {
            Destroy(other.gameObject);
            Health -= 10.0f;           //Helthbar.fillAmount = Health;
            if (Health <= 0)
            {
                /*var otheranimator = other.gameObject.GetComponent<Animator>();
                if (otheranimator)
                {
                    otheranimator.SetBool("GunAttack", false);
                    otheranimator.SetBool("HandAttack", false);
                }*/
                OnPlayerDie();
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && Health > 0)
        {
            if (Vector3.Distance(transform.position, other.transform.position) < 1.5f)
            {
                m_Animator.SetBool("HandAttack", true);
                transform.LookAt(other.transform);
                Health -= Time.deltaTime * 2;
                //Helthbar.fillAmount = Health;
            }
            if (Health <= 0)
            {
                var otheranimator = other.gameObject.GetComponent<Animator>();
                if (otheranimator)
                {
                    otheranimator.SetBool("GunAttack", false);
                    otheranimator.SetBool("HandAttack", false);
                }
                OnPlayerDie();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        m_Animator.SetBool("HandAttack", false);
    }
    public void OnMoveIntoOther(Transform m_CamTargetTransform, bool IsPlayerInsideOther, bool PlayerSetActive)
    {
        transform.gameObject.SetActive(PlayerSetActive);
        if (PlayerSetActive)
        {
            GetComponent<Rigidbody>().isKinematic = true;

            m_Animator.SetBool("Run", false);
            m_Animator.SetBool("Jump", true);
            transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            DOVirtual.DelayedCall(0.25f, () =>
            {
                GetComponent<Rigidbody>().isKinematic = false;
                IsInOtherObject = IsPlayerInsideOther;
            });
        }
        else
        {
            IsInOtherObject = IsPlayerInsideOther;
        }
        
        SimpleCamFollow.instance.m_Target = m_CamTargetTransform.transform;
        
        

    }   
    void Movement()
    {
        if (!canMove) return;

        if (m_joystick == null)
            m_joystick = FindObjectOfType<FixedJoystick>();
        if (m_joystick == null)
            return;
        // Pass input value
        horizontal = m_joystick.Horizontal;
        vertical = m_joystick.Vertical;
        movementDir = new Vector3(horizontal, 0f, vertical);
        isInput = movementDir != Vector3.zero;

        if (!isInput)
        {            
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", false);
            return;
        }
        else
        {
            m_Animator.SetBool("HandAttack", false);
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }
        transform.localPosition = new Vector3(transform.localPosition.x + movementDir.x * movementSpeed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z + movementDir.z * movementSpeed * Time.deltaTime);//old is -0.633f
        Rotate(movementDir);

    }
    public void OnPlayerDie()
    {
        Health = 0;
        Instantiate(ParticleOnDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Gamemanager.Instance.Gameover();
    }
    public void CancelMove()
    {
        canMove = false;
        /*j^oystick = null;*/
    }
    void Rotate(Vector3 moveDir)
    {
        if (!GetInputValue())
            return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotationSpeed);
    }
    public bool GetInputValue()
    {
        return isInput;
    }
}       