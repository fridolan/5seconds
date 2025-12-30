using LiteNetLib.Utils;

namespace fiveSeconds
{

    public interface INetworkTransport
    {
        void Start();
        void Stop();
        void Poll(); // regelmäßig im Spiel-Loop aufrufen
        void SendToClient(int clientId, NetDataWriter writer);
        void SendToServer(NetDataWriter writer);
        void Broadcast(NetDataWriter writer);
        void SetHandler(INetworkHandler handler);
    }

}