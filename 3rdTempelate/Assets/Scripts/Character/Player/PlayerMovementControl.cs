using Character.Base;
using Config;
using Input;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Character.Player
{
    
    public class PlayerMovementControl : CharacterMovementControlBase
    {
        private bool isAttacking = false;
        private Coroutine turnCoroutine;

        //预输入
        private float preInputTime_ATK = -1f;
        private float preInputWindowTime_ATK = 0.2f;

        //检测
        private float angle_ATK = 135f;
        private float distance_ATK = 1.5f;
        private int enemyLayerIndex;
        private LayerMask enemyLayer;

        private Collider[] detectedEnemy;
        private List<GameObject> enemyInRange = new List<GameObject>();
        private int maxCount = 10;

        //攻击设置
        [SerializeField] private int attackDamage = 1;

        //特效管理
        [System.Serializable]
        [SerializeField]private class PlayerVFXManage
        {
            public string effectName;
            public GameObject prefab;
        }
        [SerializeField] private List<PlayerVFXManage > vfxList;
        [SerializeField] private Transform vfxPoint;

        //脚步声管理
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private float volume;


        private float _rotationAngle;
        private float _angleVolocity;
        [FormerlySerializedAs("_rotationSmoothTime")] [SerializeField] private float rotationSmoothTime;

        private Transform _mainCamera;

        [FormerlySerializedAs("_isLock")] public bool isLock = false;
        //脚步声
        private float _nextStepTime;
        [FormerlySerializedAs("_slowFootTime")] [SerializeField] private float slowFootTime;
        [FormerlySerializedAs("_fastFootTime")] [SerializeField] private float fastFootTime;
        [FormerlySerializedAs("_parryFootTime")] [SerializeField] private float parryFootTime;
        
        [FormerlySerializedAs("_canTurnAndRun")] [SerializeField , Header("是否开启转身跑") , Space(10)] private bool canTurnAndRun;
        
        
        //目标朝向
        private Vector3 _characterTargetDirection;
        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
            enemyLayerIndex = LayerMask.NameToLayer("Enemy");
            enemyLayer = 1 << enemyLayerIndex;
            detectedEnemy = new Collider[maxCount];
            audioSource = GetComponent<AudioSource>();
        }

        protected override void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            base.Update();
            OnLockUpdateXY();
            PreInputATK();
        }

        private void LateUpdate()
        {

            CharacterRotationControl();

            UpdateAnimator();

        }

        private void PreInputATK()
        {
            preInputTime_ATK -= Time.unscaledDeltaTime;
            if (GameInputManager.MainInstance.LAttack)
            {
                preInputTime_ATK = preInputWindowTime_ATK;
            }
        }
        private void CharacterRotationControl()
        {
            
            if(!CharacterIsOnGround || isLock||isAttacking ) return;

            if (Animator.GetBool(AnimationID.HasInput))
            {
                _rotationAngle = 
                    Mathf.Atan2(GameInputManager.MainInstance.Movement.x , GameInputManager.MainInstance.Movement.y) * 
                    Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            }
            
            
            if (Animator.GetBool(AnimationID.HasInput) && Animator.AnimationAtTag("Motion"))
            {

                if(canTurnAndRun)
                    Animator.SetFloat(AnimationID.DeltaAngle , DevelopmentTools.GetDeltaAngle(transform, _characterTargetDirection.normalized));
                if (canTurnAndRun)
                {
                    if(Animator.GetFloat(AnimationID.DeltaAngle) < -135f && Animator.GetBool(AnimationID.Run)) return;
                    if(Animator.GetFloat(AnimationID.DeltaAngle) > 135f && Animator.GetBool(AnimationID.Run)) return;
                }
                
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle,
                    ref _angleVolocity, rotationSmoothTime);
            
                if (canTurnAndRun)
                {
                    //得到我们要转到的目标方向
                    _characterTargetDirection = Quaternion.Euler(0, _rotationAngle, 0) * Vector3.forward;
                   
                }
                
               
            }
            // if(_canTurnAndRun)
            //     _animator.SetFloat(AnimationID.DeltaAngle , DevelopmentToos.GetDeltaAngle(transform, _characterTargetDirection.normalized));
            
        }


        private void UpdateAnimator()
        {
            if(!CharacterIsOnGround) return;

            Animator.SetBool(AnimationID.HasInput, GameInputManager.MainInstance.Movement != Vector2.zero);
            Animator.SetBool(AnimationID.CanCombo, true);

            if (preInputTime_ATK >0)
            {
                Animator.SetTrigger(AnimationID.Combo);
                preInputTime_ATK = 0;
            }

            if (Animator.GetBool(AnimationID.HasInput))
            {
                
                Animator.SetBool(AnimationID.Run , GameInputManager.MainInstance.Run);

                
                Animator.SetFloat(AnimationID.Movement , (Animator.GetBool(AnimationID.Run) ? 2f : GameInputManager.MainInstance.Movement.sqrMagnitude ), 0.25f, Time.deltaTime);
            }
            else
            {
                Animator.SetFloat(AnimationID.Movement , 0f, 0.25f, Time.deltaTime);
                if (Animator.GetFloat(AnimationID.Movement) < 0.2f)
                {
                    Animator.SetBool(AnimationID.Run , false);

                }
            }
        }

        private void OnLockUpdateXY()
        {
            if (isLock)
            {
                Animator.SetFloat(AnimationID.Horizontal, GameInputManager.MainInstance.Movement.x , 0.25f, Time.deltaTime);
                Animator.SetFloat(AnimationID.Vertical, GameInputManager.MainInstance.Movement.y , 0.25f, Time.deltaTime);
            }
        }

        private List<GameObject> CheckEnemyWhenStart()
        {
            enemyInRange.Clear();
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position,distance_ATK ,detectedEnemy,enemyLayer);
            for (int i = 0; i < hitCount; i++)
            {
                GameObject enemy = detectedEnemy[i].gameObject;

                if (!enemyInRange.Contains(enemy))
                {
                    enemyInRange.Add(enemy);
                }
            }
            List<GameObject> validEnemy = new List<GameObject>();

            foreach (GameObject enemy in enemyInRange)
            {
                    validEnemy.Add(enemy);
            }

            return validEnemy;
        }
        private List<GameObject> CheckEnemyInATK()
        {
            enemyInRange.Clear();
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, distance_ATK, detectedEnemy, enemyLayer);
            for (int i = 0; i < hitCount; i++)
            {
                GameObject enemy = detectedEnemy[i].gameObject;

                if (!enemyInRange.Contains(enemy))
                {
                    enemyInRange.Add(enemy);
                }
            }
            List<GameObject> validEnemy = new List<GameObject>();

            foreach (GameObject enemy in enemyInRange)
            {
                if (IsEnemyInRange(enemy))
                {
                    validEnemy.Add(enemy);
                }
            }

            return validEnemy;
        }

        private GameObject GetClosestEnemy(List<GameObject> enemies)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (GameObject potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            return bestTarget;
        }

        private System.Collections.IEnumerator ForceSmoothTurn(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; 

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                float turnDuration = 0.15f; 
                float timeElapsed = 0f;

                while (timeElapsed < turnDuration)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timeElapsed / turnDuration);
                    timeElapsed += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                transform.rotation = targetRotation;
            }
        }

        private bool IsEnemyInRange(GameObject enemy)
        {
            if (enemy == null) return false;

            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
            if (angleToEnemy > angle_ATK / 2) return false;

            return true;
        }


        #region 连招动画事件
        public void OnAttackStart()
        {
            isAttacking = true;
            List<GameObject> checkedEnemy = CheckEnemyWhenStart();

            if (enemyInRange.Count > 0)
            {
                GameObject closestEnemy = GetClosestEnemy(enemyInRange);

                if (turnCoroutine != null)
                {
                    StopCoroutine(turnCoroutine);
                }
                turnCoroutine = StartCoroutine(ForceSmoothTurn(closestEnemy.transform.position));
            }
        }

        public void EnablePreInput()
        {
            
        }
        public void CancelAttackColdTime()
        {
            
        }

        public void DisableLinkCombo()
        {
            
        }
        public void EnableMoveInterrupt()
        {
            
        }

        public void ATK()
        {
            List<GameObject> checkedEnemy = CheckEnemyInATK();
            foreach (GameObject enemy in checkedEnemy)
            {
                EnemyBaseSystem enemySystem = enemy.GetComponent<EnemyBaseSystem>();

                if (enemySystem != null)
                {
                    enemySystem.TakeDamage(attackDamage);
                }

            }
            isAttacking = false;
        }

        #endregion

        #region 动画事件音效
        public void PlayVFX(string effectName)
        {
            foreach(var mapping in vfxList)
            {
                if(mapping.effectName==effectName)
                {
                    if(mapping .effectName!=null)
                    {
                        GameObject effect = Instantiate(mapping.prefab, vfxPoint.position, vfxPoint.rotation);

                        Destroy(effect, 2f);
                    }
                    return;
                }
            }
            //Debug.Log("找不到");
        }

        public void PlayFootSound()
        {
            int index = Random.Range(0, footstepSounds.Length);
            AudioClip clip = footstepSounds[index];
            audioSource.PlayOneShot(clip, volume);
        }
        public void PlayFootBackSound()
        {
            
        }

        public void PlayWeaponBackSound()
        {
            
        }

        public void PlayWeaponEndSound()
        {
            
        }

        #endregion
    
    }
}
