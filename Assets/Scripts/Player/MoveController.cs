using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace PlatformJump
{
    public class Force
    {
        public Force(Vector2 dir, float time)
        {
            Direction = dir;
            Time = time;

        }
        public Vector2 Direction;
        public float Time;

    }
    public class MoveController : MonoBehaviour
    {
        [SerializeField] private KeyCode LeftKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode RightKey = KeyCode.RightArrow;
        [SerializeField] private KeyCode JumpKey = KeyCode.C;
        [SerializeField] private KeyCode DashKey = KeyCode.X;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform playerSpriteTrans;
        [SerializeField] private float MaxMoveSpeed = 5;//��������ƶ��ٶ�
        private float currMoveSpeed = 0;//��ҵ�ǰͨ��������ȡ���ƶ��ٶ�
        private List<Force> forces;

        [SerializeField] private float MinJumpTime = 0.1f;
        [SerializeField] private float MaxJumpTime = 0.2f;
        [SerializeField] private float highJumoMultiplier = 2f;
        private float WallSlideSpeed = 1;
        [SerializeField] private float CollisionRadius = 0.25f;
        [SerializeField] private float DashDistance = 3f;
        [SerializeField] private Vector3 bottomOffset, wallOffset, leftOffset, rightOffset;

        [SerializeField] private int JumpFrame = 5;//���ǰ����֡������Ծ������غ��Զ���
        [SerializeField] private int FallFrame = 5;//���ն���֡��Ȼ������Ծ


        [SerializeField] private LayerMask GroundLayer;




        [Space]
        [Header("Boolֵ���")]
        //��ǰ�Ƿ�����ƶ�
        [SerializeField] private bool canMove = true;//��ǰ����Ƿ�����ƶ�
        public bool CanMove => canMove;

        [SerializeField] private bool onGround = true;//��ǰ����Ƿ��ڵ���
        public bool OnGround => onGround;

        [SerializeField] private bool onWall = false;//��ǰ����Ƿ�����ǽ��
        public bool OnWall => onWall;
        [SerializeField] private bool wallJump = false;
        public bool IsInputDisabled => inputDisableTimer > 0;
        public bool IsGravityDisabled => gravityDisableTimer > 0;
        public bool IsHorizontalDisabled => horizontalDisableTimer > 0;
        public bool IsForceVelocity => isForceVelocity;
        private bool isForceVelocity;


        private int moveDir;
        private Transform playerTransform;
        private float wallJunpTimer;
        private int jumpFrame;
        private int fallFrame;
        private int jumpCount;//��ǰ��Ծ�Ĵ���
        private int dashCount;

        private float inputDisableTimer;
        private float gravityDisableTimer;
        private float jumpTimer;
        private float horizontalDisableTimer;
        private float forceAffectTimer;
        void Start()
        {

            GroundLayer = 1<<6 | 1<<11;
            playerTransform = transform;
            Application.targetFrameRate = 60;
            forces = new List<Force>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsInputDisabled)
                InputUpdate();

            CollisionUpdate();

        }
        private void FixedUpdate()
        {
            if (canMove)
                Move();
            if (jumpFrame > 0)
            {
                --jumpFrame;
            }
            if (fallFrame > 0)
            {
                --fallFrame;
            }
            if (jumpTimer >= 0)
            {
                jumpTimer -= Time.fixedDeltaTime;
            }
            if (inputDisableTimer > 0)
            {
                inputDisableTimer -= Time.fixedDeltaTime;

            }
            if (gravityDisableTimer > 0)
            {
                gravityDisableTimer -= Time.fixedDeltaTime;
                if (gravityDisableTimer <= 0)
                {
                    EnableGravity();
                }
            }
            if (horizontalDisableTimer > 0)
            {
                horizontalDisableTimer -= Time.fixedDeltaTime;
            }
            if (forceAffectTimer > 0)
            {
                forceAffectTimer -= Time.fixedDeltaTime;
            }

            if (!isForceVelocity)
            {
                CalculateVelocity();
                JumpVelocityAdjust();
            }

        }

        private void CalculateVelocity()
        {
            Vector2 velocity = new Vector2(0, rb.velocity.y);
            if (IsHorizontalDisabled)
            {
                velocity.x = rb.velocity.x;
            }
            else
            {
                velocity.x = currMoveSpeed * moveDir;
            }

            for (int i = 0; i < forces.Count; ++i)
            {
                velocity += forces[i].Direction;
            }
            if (forceAffectTimer > 0)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, velocity, 0.1f);
            }
            else
            {
                rb.velocity = velocity;
            }

        }

        private void CollisionUpdate()
        {
            if (Physics2D.OverlapCircle(playerTransform.position + bottomOffset, CollisionRadius, GroundLayer))
            {
                if (!OnGround)
                {
                    OnTouchGround();
                }
                onGround = true;
            }
            else
            {
                if (OnGround)
                {
                    fallFrame = FallFrame;
                }
                onGround = false;
            }
            if (Physics2D.OverlapCircle(playerTransform.position + wallOffset, CollisionRadius, GroundLayer))
            {
                onWall = true;

            }
            else
            {
                onWall = false;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + bottomOffset, CollisionRadius);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position + leftOffset, CollisionRadius);
            Gizmos.DrawWireSphere(transform.position + rightOffset, CollisionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + wallOffset, CollisionRadius);

        }
        private void InputUpdate()
        {

            int xInput = (int)Input.GetAxisRaw("Horizontal");
            if (xInput != 0)
            {
                moveDir = xInput;
            }
            else
            {
                if (Input.GetKeyDown(RightKey)) moveDir = 1;
                else if (Input.GetKeyDown(LeftKey)) moveDir = -1;
                else if (!Input.GetKey(LeftKey) && !Input.GetKey(RightKey)) moveDir = 0;
            }


            if (Input.GetKeyDown(JumpKey))
            {
                if (!OnGround)
                {
                    jumpFrame = JumpFrame;
                    if (fallFrame > 0 && jumpCount == 0)
                    {
                        Jump();
                    }
                }
                else
                {
                    Jump();
                }


            }
            if (wallJump)
            {
                if (Time.time - wallJunpTimer > 0.5f) wallJump = false;
            }
            if (Input.GetKeyDown(DashKey))
            {
                Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Debug.Log(dir);
                Dash(dir.normalized);

            }

        }

        private void JumpVelocityAdjust()
        {
            if (rb.velocity.y < -0.1f)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * highJumoMultiplier * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space) && jumpTimer <= MaxJumpTime - MinJumpTime)//���û�г����ո����ٵ�MinJumpTime��ſ�ʼ��������
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * highJumoMultiplier * 2 * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && jumpTimer <= 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * highJumoMultiplier * 2 * Time.deltaTime;//������������Ծʱ�䣬������ʼ��������
            }
        }
        /// <summary>
        /// ��Ҹ��ݵ�ǰ��������ƶ�
        /// </summary>
        private void Move()
        {


            if (moveDir != 0 && currMoveSpeed < MaxMoveSpeed)
            {
                currMoveSpeed += (MaxMoveSpeed / 6);
            }
            else if (moveDir == 0 && currMoveSpeed > 0)
            {
                currMoveSpeed -= (MaxMoveSpeed / 3);
            }
            currMoveSpeed = Math.Clamp(currMoveSpeed, 0, MaxMoveSpeed);


            if (moveDir < 0)
            {
                Vector3 oldScale = playerSpriteTrans.localScale;
                oldScale.x = -1;
                playerSpriteTrans.localScale = oldScale;
                wallOffset = leftOffset;
            }
            else if (moveDir > 0)
            {
                Vector3 oldScale = playerSpriteTrans.localScale;
                oldScale.x = 1;
                playerSpriteTrans.localScale = oldScale;
                wallOffset = rightOffset;
            }
        }
        private void Jump()
        {
            ++jumpCount;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * 10;
            jumpTimer = MaxJumpTime;
        }
        public void Dash(Vector2 dir)
        {
            DisableGravity(0.5f);
            if (dir == Vector2.zero)
            {
                dir.x = playerSpriteTrans.localScale.x;
            }
            horizontalDisableTimer = 0.5f;
            rb.DOMove(rb.position + dir * DashDistance, 0.5f);
            ++dashCount;
        }

        /// <summary>
        /// ��������ʱ����
        /// </summary>
        private void OnTouchGround()
        {
            jumpCount = 0;

            wallJump = false;
            if (jumpFrame > 0)
            {
                jumpFrame = 0;
                Jump();
            }

        }

        /// <summary>
        /// ����һ��˲�����
        /// </summary>
        /// <param name="force"></param>
        /// <param name="resetVelocity">��������ǰ���ٶȹ���</param>
        /// <param name="time">������Һ������������</param>
        ///  <param name="forceEffectTime">�������õ�ʱ��</param>
        public void AddVelocity(Vector2 force, float time = 0f, float velocityEffectTime = 1f, bool resetVelocity = true)
        {
            if (resetVelocity)
            {
                rb.velocity = Vector3.zero;
            }
            horizontalDisableTimer = time;
            forceAffectTimer = velocityEffectTime;
            rb.velocity += force;
        }
        public Force AddConstantVelocity(Vector2 force, float time)
        {
            Force Force = new Force(force, time);
            forces.Add(Force);
            if (time > 0)
            {
                DOVirtual.DelayedCall(time, () => { RemoveForce(Force); });
            }
            return Force;
        }
        /// <summary>
        /// ǿ���趨�ٶȣ�����ʱ�����
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="time"></param>
        public void ConstrainVelocity(Vector2 velocity)
        {
            rb.velocity = velocity;
            isForceVelocity = true;
            rb.gravityScale = 0;

        }
        public void EndConstrainVelocity()
        {
            isForceVelocity = false;
            rb.gravityScale = 1;
        }
        public bool RemoveForce(Force force)
        {
            if (forces.Contains(force))
            {
                forces.Remove(force);
                return true;
            }
            return false;
        }
        public void SetPosition(Vector2 pos)
        {
            rb.MovePosition(pos);
        }
        public void DisableGravity(float time)
        {
            gravityDisableTimer = time;
            rb.gravityScale = 0;
        }
        public void EnableGravity()
        {
            gravityDisableTimer = 0;
            rb.gravityScale = 1;

        }
        public void DisableInput(float time)
        {
            inputDisableTimer = time;
        }
        public void EnableInput()
        {
            inputDisableTimer = 0;
        }

        public void BeHigher(){
            Vector3 higherScale = transform.localScale;
            higherScale.y = 3f;
            transform.localScale = higherScale;
            bottomOffset = new Vector3(0,-1.4f,0);
        }

        public void BeNormal(){
            Vector3 normalScale = transform.localScale;
            normalScale.y = 1f;
            transform.localScale = normalScale;
            bottomOffset = new Vector3(0,-0.33f,0);
        }

        public void BeLower(){
            Vector3 lowerScale = new Vector3(1f,0.5f,1f);
            transform.localScale = lowerScale;
            bottomOffset = new Vector3(0,-0.33f,0);
        }
    }
}