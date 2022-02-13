using System.Collections;
using System.Collections.Generic;
using Protocol;
using Protocol.Code;
using UnityEngine;

public class NetManager : ManagerBase
{
    public static NetManager Instance = null;

    void Start()
    {
        client.Connect();
    }

    private ClientPeer client = new ClientPeer("127.0.0.1", 6666);


    void Update()
    {
        while (client.socketMsgQueue.Count > 0)
        {
            SocketMsg msg = client.socketMsgQueue.Dequeue();
            //TODO 操作这个msg
            processSocketMsg(msg);
        }
    }

    #region 处理客户端内部 给服务器发消息的事件
    void Awake()
    {
        Instance = this;
        //0就是发送 Add就是Bind的上层
        Add(0, this);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case 0:
                client.Send(message as SocketMsg);
                break;
            default:
                break;
        }
    }
    #endregion

    #region 处理接收到的服务器发来的消息
    HandlerBase accountHandler = new AccountHandler();
    HandlerBase userHandler = new UserHandler();
    HandlerBase matchHandler = new MatchHandler();
    HandlerBase chatHandler = new ChatHandler();
    HandlerBase fightHandler = new FightHandler();
    /// <summary>
    /// 处理网络的消息
    /// </summary>
    private void processSocketMsg(SocketMsg msg)
    {
        switch (msg.OpCode)
        {
            case OpCode.ACCOUNT:
                accountHandler.OnReceive(msg.SubCode, msg.Value);
                break;
            case OpCode.USER:
                userHandler.OnReceive(msg.SubCode, msg.Value);
                break;
            case OpCode.MATCH:
                matchHandler.OnReceive(msg.SubCode, msg.Value);
                break;
            case OpCode.CHAT:
                chatHandler.OnReceive(msg.SubCode, msg.Value);
                break;
            case OpCode.FIGHT:
                fightHandler.OnReceive(msg.SubCode, msg.Value);
                break;
            default:
                break;
        }
    }
    #endregion
}
