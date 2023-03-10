using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A class used to control a player in a Runner
    /// game. Includes logic for player movement as well as 
    /// other gameplay logic.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary> Returns the PlayerController. </summary>
        public static PlayerController Instance => s_Instance;
        static PlayerController s_Instance;

        [Header("速度参数")]
        [SerializeField] public float ZMoveAcceraltion;//竖直方向的
        [SerializeField] public float ZMoveSpeed = 10;//鱼稳定下来后往前移动的速度
        [SerializeField] public float ZFastMoveSpeed = 15;//按住上键时的加速的速度
        [SerializeField] public float XMoveAcceraltion;//水平方向的加速度
        [SerializeField] public float XMoveSpeed = 10;//水平方向移动的速度
        [SerializeField] public float dragRatio = 0.9f;//减速系数，数字越小减速越快
        [SerializeField] public bool disableContinueForce = true;//玩家初始便具有持续性的力使其具有速度，如果想暂时关闭这个持续向前的力则将这个bool关闭即可（可以使用协程实现暂时性的关闭）

        [Header("旋转参数")]
        public float turnSpeed = 100;
        public float getBackSpeed = 0.05f;//旋转回来的速度
        public float maxTurnAngle = 50f;//最大所能旋转的角度
        bool startDownSpeed = false;//用于标识玩家此时开始减速

        [Header("跳跃参数")]
        public float bounceForce = 30f;//鱼的跳跃力

        [Header("场景宽度")]
        public float sceneWidth = 50;

        Rigidbody rigidBody;

        public bool gameInput = true;//如果想要

        [SerializeField]
        Animator m_Animator;

        [SerializeField]
        SkinnedMeshRenderer m_SkinnedMeshRenderer;

        [SerializeField]
        PlayerSpeedPreset m_PlayerSpeed = PlayerSpeedPreset.Medium;

        [SerializeField]
        float m_CustomPlayerSpeed = 10.0f;

        [SerializeField]
        float m_AccelerationSpeed = 10.0f;

        [SerializeField]
        float m_DecelerationSpeed = 20.0f;

        [SerializeField]
        float m_HorizontalSpeedFactor = 0.5f;

        [SerializeField]
        float m_ScaleVelocity = 2.0f;

        [SerializeField]
        bool m_AutoMoveForward = true;

        Vector3 m_LastPosition;
        float m_StartHeight;

        const float k_MinimumScale = 0.1f;
        static readonly string s_Speed = "Speed";

        enum PlayerSpeedPreset
        {
            Slow,
            Medium,
            Fast,
            Custom
        }

        Transform m_Transform;
        Vector3 m_StartPosition;
        [SerializeField] public bool m_HasInput;
        float m_MaxXPosition;
        float m_XPos;
        float m_ZPos;
        float m_TargetPosition;
        float m_Speed;
        float m_TargetSpeed;
        Vector3 m_Scale;
        Vector3 m_TargetScale;
        Vector3 m_DefaultScale;

        const float k_HalfWidth = 0.5f;

        ///下面这里的写法，=>，是为了将这个变量保护起来，使之变为只读的模式
        /// <summary> The player's root Transform component. </summary>
        public Transform Transform => m_Transform;

        /// <summary> The player's current speed. </summary>
        public float Speed => m_Speed;

        /// <summary> The player's target speed. </summary>
        public float TargetSpeed => m_TargetSpeed;

        /// <summary> The player's minimum possible local scale. </summary>
        public float MinimumScale => k_MinimumScale;

        /// <summary> The player's current local scale. </summary>
        public Vector3 Scale => m_Scale;

        /// <summary> The player's target local scale. </summary>
        public Vector3 TargetScale => m_TargetScale;

        /// <summary> The player's default local scale. </summary>
        public Vector3 DefaultScale => m_DefaultScale;

        /// <summary> The player's default local height. </summary>
        public float StartHeight => m_StartHeight;

        /// <summary> The player's default local height. </summary>
        public float TargetPosition => m_TargetPosition;

        /// <summary> The player's maximum X position. </summary>
        public float MaxXPosition => m_MaxXPosition;

        void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;

            Initialize();
        }

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        /// <summary>
        /// Set up all necessary values for the PlayerController.
        /// </summary>
        public void Initialize()
        {
            m_Transform = transform;
            m_StartPosition = m_Transform.position;
            m_DefaultScale = m_Transform.localScale;
            m_Scale = m_DefaultScale;
            m_TargetScale = m_Scale;

            if (m_SkinnedMeshRenderer != null)
            {
                m_StartHeight = m_SkinnedMeshRenderer.bounds.size.y;
            }
            else
            {
                m_StartHeight = 1.0f;
            }

            ResetSpeed();
        }

        /// <summary>
        /// 根据当前玩家的速度模式变量来选择不同的速度
        /// </summary>
        public float GetDefaultSpeed()
        {
            switch (m_PlayerSpeed)
            {
                case PlayerSpeedPreset.Slow:
                    return 5.0f;

                case PlayerSpeedPreset.Medium:
                    return 10;

                case PlayerSpeedPreset.Fast:
                    return 15;
            }

            return m_CustomPlayerSpeed;
        }

        /// <summary>
        /// Adjust the player's current speed
        /// </summary>
        public void AdjustSpeed(float speed)
        {
            m_TargetSpeed += speed;
            m_TargetSpeed = Mathf.Max(0.0f, m_TargetSpeed);//设定速度最大为0，暂时没懂为什么要这样
        }

        /// <summary>
        /// Reset the player's current speed to their default speed
        /// </summary>
        public void ResetSpeed()
        {
            m_Speed = 0.0f;
            m_TargetSpeed = GetDefaultSpeed();
        }

        /// <summary>
        /// Adjust the player's current scale
        /// </summary>
        public void AdjustScale(float scale)
        {
            m_TargetScale += Vector3.one * scale;
            m_TargetScale = Vector3.Max(m_TargetScale, Vector3.one * k_MinimumScale);
        }

        /// <summary>
        /// Reset the player's current speed to their default speed
        /// </summary>
        public void ResetScale()
        {
            m_Scale = m_DefaultScale;
            m_TargetScale = m_DefaultScale;
        }

        /// <summary>
        /// 获取玩家顶部的向量，大概是为了手机端移动的操作所需要的变量
        /// </summary>
        public Vector3 GetPlayerTop()
        {
            return m_Transform.position + Vector3.up * (m_StartHeight * m_Scale.y - m_StartHeight);
        }

        /// <summary>
        /// Sets the target X position of the player
        /// </summary>
        public void SetDeltaPosition(float normalizedDeltaPosition)
        {
            if (m_MaxXPosition == 0.0f)
            {
                Debug.LogError("Player cannot move because SetMaxXPosition has never been called or Level Width is 0. If you are in the LevelEditor scene, ensure a level has been loaded in the LevelEditor Window!");
            }

            float fullWidth = m_MaxXPosition * 2.0f;
            m_TargetPosition = m_TargetPosition + fullWidth * normalizedDeltaPosition;
            m_TargetPosition = Mathf.Clamp(m_TargetPosition, -m_MaxXPosition, m_MaxXPosition);//限定范围不超过最大的X横向范围
            m_HasInput = true;
        }

        /// <summary>
        /// Stops player movement
        /// </summary>
        public void CancelMovement()
        {
            m_HasInput = false;
        }

        /// <summary>
        /// Set the level width to keep the player constrained
        /// </summary>
        public void SetMaxXPosition(float levelWidth)
        {
            // Level is centered at X = 0, so the maximum player
            // X position is half of the level width
            m_MaxXPosition = levelWidth * k_HalfWidth;//x轴的水平方向的位移就是最大关卡的宽度，如果宽度为1，那么水平方向的可行走范围为（-0.5，0.5）
        }

        /// <summary>
        /// 用来重置玩家的初始状态，包括设定初始速度、位置等
        /// </summary>
        public void ResetPlayer()
        {
            m_Transform.position = m_StartPosition;
            m_XPos = 0.0f;
            m_ZPos = m_StartPosition.z;
            m_TargetPosition = 0.0f;

            m_LastPosition = m_Transform.position;

            m_HasInput = false;

            ResetSpeed();
            ResetScale();
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;
            //Debug.Log(m_TargetSpeed);

            UpdateTargetScale(deltaTime);

            //UpdateTargetSpeed(deltaTime);

            LimitEdge();

            float speed = m_Speed * deltaTime;//写着是速度，其实是这一帧内的位移变化量


            // Update position

            //UpdatePos(speed); 因为原有的方式是修改位置实现位移，那样不好实现物理效果

            //SetAnimatorSpeed(deltaTime);
            //PlayerTurn(speed);

            m_LastPosition = m_Transform.position;//这一帧结束时更新上一帧的位置
        }


        private void FixedUpdate()
        {
            TurnAngle();//旋转头
            XMoveAndBounce();
            ZMove();
        }

        void Accelerate(float deltaTime, float targetSpeed)
        {
            m_Speed += deltaTime * m_AccelerationSpeed;
            m_Speed = Mathf.Min(m_Speed, targetSpeed);

        }

        void Decelerate(float deltaTime, float targetSpeed)
        {
            m_Speed -= deltaTime * m_DecelerationSpeed;
            m_Speed = Mathf.Max(m_Speed, targetSpeed);
        }

        /// <summary>
        /// 判断两个向量是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }


        void UpdateTargetSpeed(float deltaTime)
        {
            // Update Speed

            if (!m_AutoMoveForward && !m_HasInput)
            {
                Decelerate(deltaTime, 0.0f);//减速至0
            }
            else if (m_TargetSpeed < m_Speed)
            {
                Decelerate(deltaTime, m_TargetSpeed);//如果速度大于目标速度则减速至目标速度
            }
            else if (m_TargetSpeed > m_Speed)
            {
                Accelerate(deltaTime, m_TargetSpeed);
            }
        }

        /// <summary>
        /// 改变位置
        /// </summary>
        /// <param name="speed"></param>
        void UpdatePos(float speed)
        {
            //执行水平方向的位移
            if (m_HasInput)
            {
                float horizontalSpeed = speed * m_HorizontalSpeedFactor;

                float newPositionTarget = Mathf.Lerp(m_XPos, m_TargetPosition, horizontalSpeed);
                float newPositionDifference = newPositionTarget - m_XPos;//获取相差的位移向量

                newPositionDifference = Mathf.Clamp(newPositionDifference, -horizontalSpeed, horizontalSpeed);

                m_XPos += newPositionDifference;//改变X方向的位移
            }

            //执行Z方向的位移
            m_ZPos += speed;
            m_Transform.position = new Vector3(m_XPos, m_Transform.position.y, m_ZPos);



        }

        /// <summary>
        /// 设定动画机的速度
        /// </summary>
        void SetAnimatorSpeed(float deltaTime)
        {
            if (m_Animator != null && deltaTime > 0.0f)
            {
                float distanceTravelledSinceLastFrame = (m_Transform.position - m_LastPosition).magnitude;
                float distancePerSecond = distanceTravelledSinceLastFrame / deltaTime;//每秒所走的距离

                m_Animator.SetFloat(s_Speed, distancePerSecond);//设定速度
            }
        }

        /// <summary>
        /// 往目标规模的大小更新
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateTargetScale(float deltaTime)
        {
            if (!Approximately(m_Transform.localScale, m_TargetScale))//判断目标规模和当前规模是否一致
            {
                //如果不一致则让
                m_Scale = Vector3.Lerp(m_Scale, m_TargetScale, deltaTime * m_ScaleVelocity);
                m_Transform.localScale = m_Scale;
            }
        }

        /// <summary>
        /// 限制边界
        /// </summary>
        void LimitEdge()
        {
            float posX = transform.position.x;
            posX = Mathf.Clamp(posX, -sceneWidth, sceneWidth);//限定范围不超过最大的X横向范围
            transform.position = new Vector3(posX, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// 实现玩家的转向
        /// </summary>
        void PlayerTurn(float speed)
        {
            if (m_Transform.position != m_LastPosition)
            {
                //实现人物的转向
                m_Transform.forward = Vector3.Lerp(m_Transform.forward, (m_Transform.position - m_LastPosition).normalized, speed);
            }
        }

        /// <summary>
        /// 旋转角度
        /// </summary>
        void TurnAngle()
        {
            //Debug.Log(transform.localEulerAngles.y);
            float InputX = Input.GetAxis("Horizontal");
            if (Mathf.Abs(InputX) > 0.5f)
            {
                float YAngle = transform.localEulerAngles.y;
                if (YAngle >= 0 && YAngle <= maxTurnAngle || (YAngle >= 360 - maxTurnAngle && YAngle <= 360))
                    transform.Rotate(0, InputX * Time.deltaTime * turnSpeed, 0);
            }
            else
            {
                Quaternion mid = Quaternion.identity;//这个是前方
                transform.rotation = Quaternion.Slerp(transform.rotation, mid, getBackSpeed);//球面插值，第三个参数是速度
                                                                                             //当没按下按键时，物体将会旋转回正前方的方向
            }
            /*Vector3 yRot = transform.localEulerAngles;
            yRot.y = Mathf.Clamp(yRot.y, -50, 50);
            transform.localEulerAngles = yRot;*/
        }

        /// <summary>
        /// 处理前进方向的速度
        /// </summary>
        void ZMove()
        {
            //Debug.Log(rigidBody.velocity.z);
            if (disableContinueForce == true)
            {
                rigidBody.AddForce(new Vector3(0, 0, ZMoveAcceraltion), ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.W))
            {
                m_Animator.SetBool("SpeedUp", true);
                if (rigidBody.velocity.z > ZFastMoveSpeed)
                {
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, ZFastMoveSpeed);
                }
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                m_Animator.SetBool("SpeedUp", false);

            }
            /*else if (Input.GetKeyUp(KeyCode.W))
            {
                //如果玩家松开按键，代表此时需要让玩家减速到原来的速度，并且此时需要禁用持续向前的力
                DisableContinueForce(1);//禁用一秒，在这一秒内玩家将会减速至初始的速度
                StartSpeedDown(1);//接下来开始的一秒内玩家将开始减速
            }
            else if (startDownSpeed)
            {
                float oneSecondDownSpeed = ZFastMoveSpeed - ZMoveSpeed;//在一秒内需要减少的速度量

                rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, rigidBody.velocity.z - oneSecondDownSpeed * Time.deltaTime);
            }*/
            else
            {
                m_Animator.SetBool("SpeedUp", false);

                if (rigidBody.velocity.z > ZMoveSpeed)
                {
                    //Debug.Log("rigidSpeedZ>ZMoveSpeed");

                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, ZMoveSpeed);
                    Debug.Log(rigidBody.velocity);
                }
            }
        }

        IEnumerator StartSpeedDown(float duration)
        {
            startDownSpeed = true;
            yield return new WaitForSeconds(duration);
            startDownSpeed = false;
        }

        /// <summary>
        /// 如果想暂时禁用物体的持续向前的力则调用这个函数，参数为时间
        /// </summary>
        /// <param name="duration"></param>
        public void DisableContinueForceInterface(float duration)
        {
            StartCoroutine(DisableContinueForce(duration));
        }
        IEnumerator DisableContinueForce(float duration)
        {
            disableContinueForce = false;
            yield return new WaitForSeconds(duration);
            disableContinueForce = true;
        }

        /// <summary>
        /// 处理水平方向移动
        /// </summary>
        void XMoveAndBounce()
        {

            float InputX = Input.GetAxis("Horizontal");
            rigidBody.AddForce(new Vector3(XMoveAcceraltion * InputX, 0, 0), ForceMode.Impulse);
            if (rigidBody.velocity.x > XMoveSpeed)
            {
                rigidBody.velocity = new Vector3(XMoveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            }
            else if (rigidBody.velocity.x < -XMoveSpeed)
            {
                rigidBody.velocity = new Vector3(-XMoveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            }

            if (Mathf.Abs(InputX) < 0.4f)
                rigidBody.velocity = new Vector3(rigidBody.velocity.x * dragRatio, rigidBody.velocity.y, rigidBody.velocity.z);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidBody.AddForce(new Vector3(0, bounceForce, 0), ForceMode.Impulse);
            }
        }
    }
}