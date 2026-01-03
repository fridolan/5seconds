using LiteNetLib;
using LiteNetLib.Utils;

namespace fiveSeconds
{

    public class Client : INetworkHandler
    {
        private INetworkTransport _net;

        public NetDataWriter writer;

        public byte playerId = 255;
        public int ControlledEntityID;

        public static Game Game;

        public Client()
        {
            _net = new LiteNetLibTransport();
            _net.SetHandler(this);
            writer = new NetDataWriter();
        }

        public void Start(string ip)
        {
            ((LiteNetLibTransport)_net).StartClient(ip);
        }

        public void Tick()
        {
            _net.Poll();
            Send(writer);
            writer = new NetDataWriter();
        }

        private void Send(NetDataWriter writer)
        {
            if (writer.Length != 0)
                _net.SendToServer(writer);
        }

        public void OnClientConnected(int clientId)
        {
        }

        public void OnClientDisconnected(int clientId)
        {
            Console.WriteLine("Disconnected from server.");
        }

        public void OnDataReceived(int senderId, NetPacketReader reader)
        {
            //Console.WriteLine($"CLIENT: From {senderId}, Remaining bytes: {reader.AvailableBytes}");
            while (reader.AvailableBytes > 0)
            {
                byte type = reader.GetByte();
                Console.WriteLine($"Byte: {type}");

                if(ServerMessages.MessageHandlers.TryGetValue((SMessageType)type, out var handler))
                {
                    handler(reader);
                }
                else
                {
                    Console.WriteLine($"CLIENT ERROR: Unknown message type: {type}");
                    break;
                }
            }
        }
    }
}