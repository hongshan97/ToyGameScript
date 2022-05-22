using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using proto.Enter;
using proto.Move;
using proto.Leave;
using proto.List;
using proto.Attack;

public class _Main : MonoBehaviour
{
    public Text text;
    public GameObject humanPrefab;

    public BaseHuman myHuman; // 玩家自己

    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>(); // 其他玩家


    void OnEnter(MsgBase msgBase) {
        Debug.Log("OnEnter：msgName是" + msgBase.msgName);
        Enter msg = (Enter)msgBase.msgBody;
        string desc = msg.desc;
        if(_NetManager.GetDesc() == desc)
            return;
        float x = msg.x;
        float y = msg.y;
        float z = msg.z;
        float e = msg.e;
        Debug.Log("desc" + desc +"x" + x +"y" + y +"z" + z +"e" + e);

        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, e, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
    }
    void OnMove(MsgBase msgBase) {
        Debug.Log("OnMove：msgName是" + msgBase.msgName);

        Move msg = (Move)msgBase.msgBody;
        string desc = msg.desc;
        if(desc == NetManager.GetDesc())
            return;
        float x = msg.x;
        float y = msg.y;
        float z = msg.z;
        otherHumans[desc].MoveTo(new Vector3(x, y, z));
    }
    void OnLeave(MsgBase msgBase) {
        Debug.Log("OnLeave：msgName是" + msgBase.msgName);
        
        Leave msg = (Leave)msgBase.msgBody;
        string desc = msg.desc;
        if(otherHumans.ContainsKey(desc)) {
            Destroy(otherHumans[desc].gameObject);
            otherHumans.Remove(desc);
        }
        else {
            Debug.Log("要求销毁的玩家不存在");
        }
    }
    void OnList(MsgBase msgBase) {
        Debug.Log("OnList：msgName是" + msgBase.msgName);

        List msg = (List)msgBase.msgBody;

        for(int i = 0; i < msg.element.Count; ++i) {
            var ele = msg.element[i];
            string desc = ele.desc;
            if(desc == NetManager.GetDesc())
                continue;
            float x = ele.x;
            float y = ele.y;
            float z = ele.z;
            float e = ele.e;
            GameObject obj = (GameObject)Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, e, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    void OnAttack(MsgBase msgBase) {
        Debug.Log("OnAttack：msgName是" + msgBase.msgName);
        Attack msg = (Attack)msgBase.msgBody;
        string desc = msg.desc;
        float e = msg.e;
        if(otherHumans.ContainsKey(desc)) {
            BaseHuman h = otherHumans[desc];
            h.transform.eulerAngles = new Vector3(0, e, 0);
            h.Attack();
        }
        else {
            Debug.Log("要求attack的玩家不存在");
        }
    }

    public void OnConnectSucc(MsgBase msg) {
        SelfEnter();
    }

    public void OnConnectFail(MsgBase msg) {

    }

    public void OnConnectClose(MsgBase msg) {

    }


    public void SelfEnter() {
        // 添加玩家自己
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(250, 750);
        float z = Random.Range(250, 750);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = _NetManager.GetDesc();

        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;

        MsgBase msgBase = new MsgBase("Enter");
        Enter msgBody = new Enter();
        msgBody.desc = _NetManager.GetDesc();
        msgBody.x = pos.x;
        msgBody.y = pos.y;
        msgBody.z = pos.z;
        msgBody.e = eul.y;
        msgBase.msgBody = msgBody;
        _NetManager.Send(msgBase);
    }

    
    // Start is called before the first frame update
    void Start()
    {
        _NetManager.AddMsgListener("ConnectSucc", OnConnectSucc);
        _NetManager.AddMsgListener("ConnectFail", OnConnectFail);
        _NetManager.AddMsgListener("ConnectClose", OnConnectClose);

        _NetManager.AddMsgListener("Enter", OnEnter);
        _NetManager.AddMsgListener("Move", OnMove);
        _NetManager.AddMsgListener("Leave", OnLeave);
        _NetManager.AddMsgListener("List", OnList);
        _NetManager.AddMsgListener("Attack", OnAttack);

        _NetManager.Connect("139.224.100.205", 8001);
    }

    // Update is called once per frame
    void Update()
    {
        _NetManager.MsgUpdate();
    }
}
