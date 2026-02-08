using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;

namespace fiveSeconds
{

    public class LiteNetLibTransport : INetworkTransport, INetEventListener
    {
        private NetManager _net;
        private INetworkHandler _handler;
        private bool _isServer;
        private Dictionary<int, NetPeer> _peers = new();
        private NetPeer _serverPeer;
        private int port = 8888;

        public void Start()
        {
            _net = new NetManager(this)
            {
                AutoRecycle = true
            };
            _isServer = true;
            _net.Start(port);
        }

        public void StartClient(string address = "localhost")
        {
            _net = new NetManager(this);
            _isServer = false;
            _net.Start();
            _net.Connect(address, port, "secret");
        }

        public void Stop()
        {
            _net?.Stop();
            _peers.Clear();
        }

        public void Poll() => _net?.PollEvents();

        public void SetHandler(INetworkHandler handler) => _handler = handler;

        public void SendToClient(int clientId, NetDataWriter writer)
        {
            if (_peers.TryGetValue(clientId, out var peer))
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void SendToServer(NetDataWriter writer)
        {
            _serverPeer?.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void SendToServerUnreliable(NetDataWriter writer)
        {
            _serverPeer?.Send(writer, DeliveryMethod.Unreliable);
        }

        public void Broadcast(NetDataWriter writer)
        {
            //Console.WriteLine("Server broadcast");
            foreach (var peer in _peers.Values)
            {
               // Console.WriteLine($"Send to {peer.Id}");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            if (_isServer)
            {
                _peers[peer.Id] = peer;
                _handler?.OnClientConnected(peer.Id);
            }
            else
            {
                _serverPeer = peer;
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (_isServer)
            {
                _peers.Remove(peer.Id);
                _handler?.OnClientDisconnected(peer.Id);
            }
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            OnNetworkReceive(peer, reader, deliveryMethod);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _handler?.OnDataReceived(peer.Id, reader);
            reader.Recycle();
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("secret");
        }

        public void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }






        ////////////////////////////////////////////////////////////

        /** Für ChannelNumber falls gebraucht */
       /*  void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            throw new NotImplementedException();
        } */

        /** Für Discovery, also Pakete von nicht verbundenen Clients */
        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            throw new NotImplementedException();
        }
    }

}