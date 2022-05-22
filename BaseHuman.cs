using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    public float speed = 1.2f; // 移动速度
    public string desc = ""; // 描述

    protected bool isMoving = false;

    private Animator animator; // 动画
    private Vector3 targetPosition; // 移动目标


    internal bool isAttacking = false;

    internal float attackTime = float.MinValue;

    // 攻击
    public void Attack() {
        isAttacking = true;
        attackTime = Time.time;
        animator.SetBool("isAttack", true);
    }

    public void AttackUpdate() {
        if(!isAttacking)
            return;
        if(Time.time - attackTime < 1.2f)
            return;
        isAttacking = false;
        animator.SetBool("isAttack", false);
    }

    // 移动
    public void MoveTo(Vector3 pos) {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);

        transform.LookAt(targetPosition);
    }

    public void MoveUpdate() {
        if(isMoving == false) {
            return;
        }

        Vector3 pos = transform.position;

        if(Vector3.Distance(pos, targetPosition) < 2f) {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
        else {
            transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime); // 朝target移动
            transform.LookAt(targetPosition);
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }
}
