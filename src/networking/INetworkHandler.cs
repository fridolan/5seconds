using LiteNetLib;

namespace fiveSeconds
{

    public interface INetworkHandler
    {
        void OnClientConnected(int clientId);
        void OnClientDisconnected(int clientId);
        void OnDataReceived(int senderId, NetPacketReader reader);
    }

}
