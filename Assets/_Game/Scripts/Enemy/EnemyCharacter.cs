    using System.Collections;
    using System.Collections.Generic;
    using Scriptable;
    using UnityEngine;
    using UnityEngine.AI;

    public class EnemyCharacter : BaseMovement
    {
        [Header("State")]
        [SerializeField] private IState<EnemyCharacter> currentState;
        [Header("Enemy AI")]
        [SerializeField] NavMeshAgent agent;
        [Header("Speed")]
        [Header("Block control")]
        [SerializeField] float timer = 0f;
        [SerializeField] float randomTargetIndex ;
        [SerializeField] float sphereRadius;
        [SerializeField] public List<GameObject> targetObjects;
        [SerializeField] GameObject target;
        [SerializeField] public int blockCount;
        [SerializeField] public int blockIndex = 0;
        [SerializeField] Material material;
        [Header("Check Point")]
        [SerializeField] GameObject checkPoint;
        [Header("Finish")]
        [SerializeField] GameObject FinishPoint;
        private void Start()
        {
            randomTargetIndex = Random.Range(3,8);
            ChangeState(new IdleState());
        }

        // Update is called once per frame
        void Update()
        {
            if (isWin || currentState == null) return;
            {
               
                currentState.OnExecute(this);
            }   
            
        }
        //Change State

        public void ChangeState(IState<EnemyCharacter> state)
        {
            if (currentState != null)
            {
                currentState.OnExit(this);
            }

            currentState = state;

            if (currentState != null)
            {
                currentState.OnEnter(this);
            }
        }
        // Collect block
        public void SetTarget()
        {
            Vector3 center = transform.position; 
            float radius = 30f; 
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            targetObjects = new List<GameObject>(); 

            foreach (var hitCollider in hitColliders)
            {
                MeshRenderer meshRenderer = hitCollider.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.material != null)
                {
                    Color hitColliderColor = meshRenderer.material.color;
                    // Check if the GameObject has the tag "Block" and the colors match
                    if (hitCollider.gameObject.CompareTag("Block") && material.color == hitColliderColor)
                    {
                       
                        targetObjects.Add(hitCollider.gameObject);
                    }
                }
            }


        }
        public void MoveToTarget()
        {
            timer += Time.deltaTime;
            if(timer > 10f)
            {
                randomTargetIndex = Random.Range(3,15);
                timer = 0;
            }
            blockCount = GetComponent<BrickControl>().blockCount;
            if (targetObjects.Count == 0)
            {
                SetTarget();
                if(blockCount > 0)
                {
                    ChangeState(new BuildState());
                }
                else
                {
                    ChangeState(new IdleState());
                }
            }
            if(targetObjects.Count > 0)
            {
             
                target = targetObjects[0];
            if (target != null)
            {
                agent.SetDestination(target.transform.position);

                if (agent.remainingDistance < 1.5f)
                {
                    targetObjects.RemoveAt(0);
                }
                if (blockCount >= randomTargetIndex)
                {
                    ChangeState(new BuildState());
                }
            }
            }
            
        }
        // Build brick
        public void MoveToFinishPoint()
        {
            agent.SetDestination(FinishPoint.transform.position);
        }
        public void CheckBuildingState()
        {
            blockCount =  blockCount = GetComponent<BrickControl>().blockCount;
            if(blockCount == 0)
            {
                ChangeState(new CollectState());
            }
            
        }
        // Win
        private void Win()
        {
            ChangeAnim("dancing");
            rb.velocity = Vector3.zero;
            isWin = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "CheckPoint")
            {
                checkPoint = other.gameObject;
                checkPoint.GetComponent<StageFinish>().IsPassed = true;
                SetTarget();
                while (targetObjects.Count > 0)
                {
                    GameObject objectToDestroy = targetObjects[0];
                    Destroy(objectToDestroy);
                    targetObjects.RemoveAt(0);
                }
                ChangeState(new CollectState());
            
            }
        }

        

    }
