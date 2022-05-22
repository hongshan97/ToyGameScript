using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using proto.Enter;

public class Main : MonoBehaviour
{
    public GameObject humanPrefab;

    public BaseHuman myHuman; // 玩家自己

    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>(); // 其他玩家


    void OnEnter(string msg) {
        Debug.Log("Enter" + msg);

        string[] split = msg.Split(',');
        string desc = split[0];
        if(NetManager.GetDesc() == desc)
            return;
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float e = float.Parse(split[4]);        

        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, e, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
    }
    void OnMove(string msg) {
        Debug.Log("Move" + msg);

        string[] split = msg.Split(',');
        string desc = split[0];
        if(desc == NetManager.GetDesc())
            return;
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        otherHumans[desc].MoveTo(new Vector3(x, y, z));
    }
    void OnLeave(string msg) {
        Debug.Log("Leave1     " + msg);
        
        string[] split = msg.Split(',');
        string desc = split[0];
        if(otherHumans.ContainsKey(desc)) {
            Destroy(otherHumans[desc].gameObject);
            otherHumans.Remove(desc);
        }
        else {
            Debug.Log("要求销毁的玩家不存在");
        }
    }
    void OnList(string msg) {
        Debug.Log("List" + msg);

        string[] split = msg.Split(',');
        int cnt = split.Length / 5;
        for(int i = 0; i < cnt; ++i) {
            string desc = split[i * 5];
            if(desc == NetManager.GetDesc())
                continue;
            float x = float.Parse(split[i * 5 + 1]);
            float y = float.Parse(split[i * 5 + 2]);
            float z = float.Parse(split[i * 5 + 3]);
            float e = float.Parse(split[i * 5 + 4]);
            GameObject obj = (GameObject)Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, e, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    void OnAttack(string msg) {
        Debug.Log("OnAttack" + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        float e = float.Parse(split[1]);
        if(otherHumans.ContainsKey(desc)) {
            BaseHuman h = otherHumans[desc];
            h.transform.eulerAngles = new Vector3(0, e, 0);
            h.Attack();
        }
        else {
            Debug.Log("要求attack的玩家不存在");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Attack", OnAttack);
// NetManager.Connect("139.224.100.205", 8001);

        // 添加玩家自己
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(250, 750);
        float z = Random.Range(250, 750);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        // 玩家自己Enter
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr = sendStr + NetManager.GetDesc() + "," + pos.x + "," + pos.y + "," + pos.z + "," + eul.y + "\n";
// Debug.Log(sendStr);
// NetManager.Send(sendStr);







        _NetManager.Connect("139.224.100.205", 8001);
    }

    public void Enter() {
        // 添加玩家自己
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(250, 750);
        float z = Random.Range(250, 750);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;

        MsgBase msgBase = new MsgBase();
        msgBase.msgName = "Enter";
        Enter msgBody = new Enter();
        msgBody.desc = _NetManager.GetDesc();
        msgBody.x = pos.x;
        msgBody.y = pos.y;
        msgBody.z = pos.z;
        msgBody.e = eul.y;
        msgBase.msgBody = msgBody;
        _NetManager.Send(msgBase);
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
}
