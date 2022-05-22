using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using proto.Attack;
using proto.Move;

public class CtrlHuman : BaseHuman
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {   
        base.Update();

        if(Input.GetMouseButtonDown(0)) { // 0左，1右
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if(hit.collider.tag == "Land") {
                MoveTo(hit.point);

                MsgBase msgBase = new MsgBase("Move");
                Move msgBody = new Move();
                msgBody.desc = _NetManager.GetDesc();
                msgBody.x = hit.point.x;
                msgBody.y = hit.point.y;
                msgBody.z = hit.point.z;
                msgBase.msgBody = msgBody;
                _NetManager.Send(msgBase);
            }
        }

        if(Input.GetMouseButtonDown(1) && !(isAttacking || isMoving)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            transform.LookAt(hit.point);
            Attack();

            MsgBase msgBase = new MsgBase("Attack");
            Attack msgBody = new Attack();
            msgBody.desc = _NetManager.GetDesc();
            msgBody.e = transform.eulerAngles.y;
            msgBase.msgBody = msgBody;
            _NetManager.Send(msgBase);
        }
    }
}
