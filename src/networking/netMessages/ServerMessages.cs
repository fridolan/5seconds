using LiteNetLib.Utils;
// Enthält Funktionen zum Versenden und Verarbeiten von Server-Nachrichten.
// Normalerweise wird z.B. eine Funktion "Test" vom Server benutzt, und triggert auf Client-Seite die Funktion "rTest", welche die Nachricht von "Test" verarbeitet.
// Zusammengehörige Funktionspaare (Test u. rTest) sollten untereinander stehen.
//
// Schreibende Funktionen bekommen normalerweise einen writer übergeben, und sind daher im Normalfall unabhängig vom Empfänger.
// Lesende Funktionen bekommen einen reader übergeben, welcher immer zum eigenen Client gehört.

namespace fiveSeconds
{

    [Flags]
    public enum ADFlags : byte
    {
        None = 0,
        position = 1 << 0,
        name = 1 << 1,
        dialogue = 1 << 2,
    }

    public enum SMessageType : byte
    {
        PlayerID,
        ActionLists,
        SetGameState,
        Entities,
        SetLobbyInfo,
        GameStart
    }

    public static class ServerMessages
    {
        private static bool debug = false;

        public static Dictionary<SMessageType, Action<NetDataReader>> MessageHandlers = new()
        {
            { SMessageType.PlayerID, rPlayerID },
            { SMessageType.ActionLists, rActionLists },
            { SMessageType.SetGameState, rGameState },
            { SMessageType.Entities, rEntities },
            { SMessageType.SetLobbyInfo, rSetLobbyInfo },
            { SMessageType.GameStart, rGameStart},
        };

        public static void PlayerID(NetDataWriter writer, byte playerID)
        {
            writer.Put((byte)SMessageType.PlayerID);
            writer.Put(playerID);
        }

        public static void rPlayerID(NetDataReader reader)
        {
            Window.Client.playerId = reader.GetByte();
            Console.WriteLine($"Client Received PlayerID: {Window.Client.playerId}");
        }

        public static void ActionLists(NetDataWriter writer, List<ActionList> entityActionLists, int round)
        {
            writer.Put((byte)SMessageType.ActionLists);
            writer.Put(round);
            writer.Put(entityActionLists.Count);
            for (int i = 0; i < entityActionLists.Count; i++)
            {
                writer.Put(Client.Game.CurrentStage.EntityList[i].ID);
                var list = entityActionLists[i];
                list.Write(writer);
            }
        }

        public static void rActionLists(NetDataReader reader)
        {
            Console.WriteLine("Client Read ActionLists");
            int round = reader.GetInt();
            int listCount = reader.GetInt();
            Console.WriteLine($"listCount {listCount}");

            for (int i = 0; i < listCount; i++)
            {
                int entityID = reader.GetInt();
                ActionList list = ActionList.FromReader(reader);
                Entity entity = Client.Game.CurrentStage.EntityList.Find(e => e.ID == entityID);
                if(entity == null) throw new Exception("rActionLists: Entity not found by ID. Async.");
                entity.ActionList = list;
            }

            Client.Game.CurrentStage.Round = round;
        }

        public static void SetGameState(NetDataWriter writer, GameState state, long time, int round)
        {
            writer.Put((byte)SMessageType.SetGameState);
            writer.Put((int)state);
            writer.Put(time);
            writer.Put(round);

        }

        public static void rGameState(NetDataReader reader)
        {

            GameState gameState = (GameState)reader.GetInt();
            Client.Game.SetState(gameState);
            long time = reader.GetLong();
            int round = reader.GetInt();
            Console.WriteLine($"Client: Handle rGameState {gameState} {time} {round}");
            if (gameState == GameState.INPUT)
            {
                Client.Game.InputTimeLeft = time;
            }
            Client.Game.CurrentStage.Round = round;
        }

        public static void Entities(NetDataWriter writer, List<Entity> entities)
        {
            writer.Put((byte)SMessageType.Entities);
            writer.Put(entities.Count);
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Write(writer);
            }
        }

        public static void rEntities(NetDataReader reader)
        {
            Stage stage = Client.Game.CurrentStage;
            stage.ClearEntities();

            int entityCount = reader.GetInt();
            for (int i = 0; i < entityCount; i++)
            {
                Entity entity = Entity.FromReader(reader);
                stage.AddEntity(entity);
            }
        }

        private static int LobbyInfoVersion = 0;

        public static void SetLobbyInfo(NetDataWriter writer)
        {
            LobbyInfoVersion++;
            writer.Put((byte)SMessageType.SetLobbyInfo);
            View.Lobby.Info.Write(writer);

            List<Player> players = Window.Server.players.Values.ToList();
            writer.Put(players.Count);

            players.ForEach((p) =>
            {
                p.Write(writer);
            });

            int version = LobbyInfoVersion;
            Console.WriteLine($"Sending LobbyInfo.. v{version}");
            writer.Put(version);
        }

        private static int rLobbyInfoVersion = -2;

        public static void rSetLobbyInfo(NetDataReader reader)
        {

            LobbyInfo info = LobbyInfo.FromReader(reader);

            List<Player> players = [];

            int playerCount = reader.GetInt();
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(Player.FromReader(reader));
            }

            rLobbyInfoVersion = reader.GetInt();

            Console.WriteLine($"Receiving LobbyInfo.. v{rLobbyInfoVersion}");
            View.Lobby.Info = info;
            Client.Game.SetState(GameState.LOBBY);
            LobbyView.Players = players;
        }

        public static void GameStart(NetDataWriter writer)
        {
            writer.Put((byte)SMessageType.GameStart);
            writer.Put(LobbyInfoVersion);
            writer.Put(Window.Server.PlayerList.Count);
            Window.Server.PlayerList.ForEach((p) =>
            {
                writer.Put(p.ID);
                writer.Put(p.Entity?.ID ?? -1);
            });
        }

        public static void rGameStart(NetDataReader reader)
        {
            int lobbyVersion = reader.GetInt();
            if (lobbyVersion != rLobbyInfoVersion)
            {
                throw new Exception($"Client has outdated LobbyInfo version {rLobbyInfoVersion}. How. Should be {lobbyVersion}");
            }
            int playerCount = reader.GetInt();
            // TODO: Let Client remember players to display names on entities etc.
            Window.Client.Players = [];
            for (int i = 0; i < playerCount; i++)
            {
                byte id = reader.GetByte();
                int entityId = reader.GetInt();
                if (id == Window.Client.playerId)
                {
                    Window.Client.ControlledEntityID = entityId;
                }
                Player player = new()
                {
                  ID = id,
                };
                Window.Client.Players.Add(player);
            }

            Stage stage = Stage.GetStage(View.Lobby.Info, Client.Game);
            Client.Game.CurrentStage = stage;
            View.CurrentView = View.GameView;
            Client.Game.SetState(GameState.GAMESTART);
            ClientMessages.ConfirmGameStart(Window.Client.writer);
        }


    }

}