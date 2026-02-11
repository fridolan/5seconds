using LiteNetLib;
using LiteNetLib.Utils;

namespace fiveSeconds
{

    public class Server : INetworkHandler
    {
        private INetworkTransport _net;

        public NetDataWriter bWriter = new();
        public Dictionary<int, NetDataWriter> cWriters = [];

        public Dictionary<byte, Player> players = [];
        public List<Player> PlayerList => players.Values.ToList(); 
        public Dictionary<byte, int> pIdToCId = [];
        public Dictionary<int, byte> CIdToPId = [];
        private byte idCounter = 0;
        public int playerCount = 0;

        public double writeTickTimer = 0;
        public double writeInterval = 1 / 60f;
        public double stateTickTimer = 0;
        public double stateTickInterval = 1 / 10f;

        public Player AddPlayer(int clientId, int x, int y)
        {
            byte id = idCounter++;
            pIdToCId[id] = clientId;
            CIdToPId[clientId] = id;
            playerCount += 1;
            Player player = new Player
            {
                ID = id,
                //position = new(x, y),
                ClientId = clientId
            };
            players[id] = player;

            //ServerMessages.PlayerM(Game.Server.bWriter, player);

            return player;
        }

        public Server()
        {
            _net = new LiteNetLibTransport();
            _net.SetHandler(this);
/*             Game = new Game();
            Game.OnLoad(); */
        }

        public void Start()
        {
            _net.Start();
        }

        public void Stop()
        {
            _net.Stop();
        }

        public void Tick(double time)
        {
            _net.Poll();
            writeTickTimer += time;
            stateTickTimer += time;

            if (stateTickTimer > stateTickInterval)
            {
                stateTickTimer -= stateTickInterval;
                //ServerState.Tick(stateTickInterval);
            }

            if (writeTickTimer > writeInterval)
            {
                //Console.WriteLine($"T: {time}, tt: {tickTimer} ,wI: {writeInterval}");
                writeTickTimer -= writeInterval;
                SendWriters();
            }

        }

        public void SendWriters()
        {
            if (bWriter.Length > 0)
            {
                _net.Broadcast(bWriter);
                bWriter.Reset();
            }

            int[] clientIds = cWriters.Keys.ToArray();
            for (int i = 0; i < clientIds.Length; i++)
            {
                int clientId = clientIds[i];
                var writer = cWriters[clientId];
                if (writer.Length > 0)
                {
                    _net.SendToClient(clientId, writer);
                    writer.Reset();
                }

            }
        }

        public void OnClientConnected(int clientId)
        {
            Console.WriteLine($"Client {clientId} connected.");

            Player newPlayer = AddPlayer(clientId, 50, 50);
 
            NetDataWriter writer = new();
            cWriters[clientId] = writer;
            ServerMessages.PlayerID(writer, newPlayer.ID);
            if(playerCount == 1)
            {
                Window.Client.playerId = newPlayer.ID;
            }
        }

        public void OnClientDisconnected(int clientId)
        {
            Console.WriteLine($"Client {clientId} disconnected.");
        }

        public void OnDataReceived(int senderId, NetPacketReader reader)
        {
            //Console.WriteLine($"From {senderId}, Remaining bytes: {reader.AvailableBytes}");
            byte playerId = CIdToPId[senderId];

            while (reader.AvailableBytes > 0)
            {
                byte type = reader.GetByte();

                if(ClientMessages.MessageHandlers.TryGetValue((CMessageType)type, out var handler))
                {
                    handler(reader, playerId);
                }
                else
                {
                    Console.WriteLine($"SERVER ERROR: Received unknown message type: {type}");
                }
            }
        }

        public NetDataWriter GetWriterByPlayerByte(byte playerByte)
        {
            return Window.Server.cWriters[pIdToCId[playerByte]];
        }

        public Player GetPlayerByClientId(int clientId)
        {
            return players[CIdToPId[clientId]];
        }

        public Player? GetPlayerByByte(byte playerByte)
        {
            players.TryGetValue(playerByte, out Player player);
            return player;
        }
    }
}