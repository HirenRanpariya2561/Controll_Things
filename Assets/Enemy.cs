using SensorToolkit;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private List<Transform> m_MoveTransforms = new List<Transform>();
    // Start is called before the first frame update
    [SerializeField] private Rigidbody m_ThisRigidBody;
    [SerializeField] private Animator m_ThisAnimator;
    [SerializeField] private GameObject Gun;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject ParticleOnDestroy;
    public SteeringRig Steering;
    [SerializeField] AttackMode m_EnemyAttackMode;
    public bool CanMove = false;
    private bool ascending = true;
    public float EnemyHealth;
    [SerializeField] Image Healthbar;
    [SerializeField] RectTransform m_rectbar;



    [Button]
    void SetRefs()
    {
        m_MoveTransforms.Clear();
        m_MoveTransforms.AddRange(transform.parent.parent.Find("MovePositions").GetComponentsInChildren<Transform>());
        m_MoveTransforms.RemoveAt(0);
        m_ThisRigidBody = GetComponent<Rigidbody>();
        m_ThisAnimator = GetComponent<Animator>();
    }
    void Start()
    {
        
        StartCoroutine(PatrolState());
        CanMove = true;
    }

    IEnumerator PatrolState()
    {
        var nextWaypoint = getNearestWaypointIndex();

    Start:

        if (CanMove)
        {

            
            Steering.DestinationTransform = m_MoveTransforms[nextWaypoint];
            m_ThisAnimator.SetBool("Run", true);
            Steering.FaceTowardsTransform = m_MoveTransforms[nextWaypoint];
            if ((transform.position - m_MoveTransforms[nextWaypoint].position).magnitude < 1f)
            {
                nextWaypoint = ascending ? nextWaypoint + 1 : nextWaypoint - 1;
                if (nextWaypoint >= m_MoveTransforms.Count - 1 || nextWaypoint < 0)
                {
                    if (nextWaypoint < 0) nextWaypoint = 0;
                    ascending = !ascending;
                 
                }
            }
        }

        yield return null;
        goto Start;
    }
    [Button]
    public void PauseWalking()
    {
        CanMove = false;
        m_ThisAnimator.SetBool("Run", false);
        StopCoroutine(PatrolState());
        Steering.MoveSpeed = 0;
        Steering.FaceTowardsTransform = null;
        Steering.DestinationTransform = null;
        m_ThisAnimator.SetBool("Run", false);
    }
    int getNearestWaypointIndex()
    {
        float nearestDist = 0f;
        int nearest = -1;
        for (int i = 0; i < m_MoveTransforms.Count; i++)
        {
            var dist = (transform.position - m_MoveTransforms[i].position).sqrMagnitude;
            if (dist < nearestDist || nearest == -1)
            {
                nearest = i;
                nearestDist = dist;
            }
        }
        Debug.Log(nearest);
        Debug.Log(m_MoveTransforms[nearest]);
        return nearest;
    }
    public void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.layer == 8 &&  collision.gameObject.GetComponent<Ref>().isref)
        {
            var otherobjControl = collision.gameObject.GetComponent<ObjectControls>();
            if (otherobjControl.m_PlayerInside)
            {
                PauseWalking();
                Steering.FaceTowardsTransform = collision.gameObject.transform;
                m_ThisAnimator.SetBool("GunAttack", true);
                //otherobjControl.OnDestroyThisThing();
                OnEnemyDie();
            }

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))//else if
        {
            PauseWalking();
            if (m_EnemyAttackMode == AttackMode.Gun)
            {
                Steering.FaceTowardsTransform = other.transform;
                m_ThisAnimator.SetBool("GunAttack", true);
            }
            //Player
        }

    }
    private void OnTriggerStay(Collider other)
    {  
       
    }
    private void OnEnemyDie()
    {        
        Destroy(gameObject);
        var a = Instantiate(ParticleOnDestroy, transform.position, Quaternion.identity);
        a.transform.eulerAngles = new Vector3(90.0f, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    [Button]
    public void FireBullet()
    {
        var localbullet = Instantiate(Bullet);
        localbullet.transform.position = Gun.transform.position;
        var bulletrigid = localbullet.GetComponent<Rigidbody>();
        bulletrigid.isKinematic = false;
        bulletrigid.rotation = Steering.transform.rotation;
        bulletrigid.AddForce(transform.forward * 10f, ForceMode.VelocityChange);
        Destroy(localbullet, 5f);
    }
}
public enum AttackMode
{
    Gun,
}
