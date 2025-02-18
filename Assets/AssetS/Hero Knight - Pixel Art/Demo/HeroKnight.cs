﻿using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    [Header("Block Settings")]
    [SerializeField] private Transform deflectRange;
    [SerializeField] private float deflectRadius = 1.5f;


    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private bool                isBlocking = false;
    public int                  m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private AudioSource _attackAudioSource;
    private int jumpCount = 0;
    [SerializeField] private int maxJumps = 1;


    [Header("Audio Clips")]
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip rollSound;


    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        if (PlayerPrefs.GetInt("DoubleJumpUnlocked", 0) == 1)
        {
            maxJumps = 2;
        }
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        _attackAudioSource = GetComponent<AudioSource>();
    }

    void Update ()
    {
        m_timeSinceAttack += Time.deltaTime;

        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
            jumpCount = 0;
        }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        float inputX = Input.GetAxis("Horizontal");

        if (!isBlocking) // Solo permite el movimiento si no está bloqueando
        {
            if (inputX > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1;
            }
            else if (inputX < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1;
            }

            if (!m_rolling)
                m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }

        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

       
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Attack
        if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            _attackAudioSource.Play();
            m_currentAttack++;

            if (m_currentAttack > 3)
                m_currentAttack = 1;

            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);

            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            _attackAudioSource.PlayOneShot(blockSound);
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            isBlocking = true;
            DeflectProjectiles();
        }

        else if (Input.GetMouseButtonUp(1))
        {
            m_animator.SetBool("IdleBlock", false);
            isBlocking = false;
        }
        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            _attackAudioSource.PlayOneShot(rollSound);
            m_rolling = true;
            m_rollCurrentTime = 0.0f;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }

        // Double Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_animator.SetTrigger("Jump");
            jumpCount++;
        }

        if (m_body2d.velocity.y == 0)
        {
            jumpCount = 0; 
        }

        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_animator.SetInteger("AnimState", 0);
        }
    }
    private void DeflectProjectiles()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(deflectRange.position, deflectRadius);
        foreach (Collider2D hit in hits)
        {
            IDeflectable deflectable = hit.GetComponent<IDeflectable>();
            if (deflectable != null)
            {
                Vector2 deflectDirection = (transform.position - hit.transform.position).normalized;
                deflectable.Deflect(deflectDirection);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (deflectRange != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(deflectRange.position, deflectRadius);
        }
    }


    // Animation Events
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
