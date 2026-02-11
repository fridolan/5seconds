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
        LobbyInfo
    }

    public static class ServerMessages
    {
        public static Dictionary<SMessageType, Action<NetDataReader>> MessageHandlers = new()
        {
            { SMessageType.PlayerID, rPlayerID },
            { SMessageType.ActionLists, rActionlists },
            { SMessageType.SetGameState, rGameState },
            { SMessageType.Entities, rEntities },
            { SMessageType.LobbyInfo, rLobbyInfo },
        };

        public static void PlayerID(NetDataWriter writer, byte playerID, Entity entity)
        {
            writer.Put((byte)SMessageType.PlayerID);
            writer.Put(playerID);
            writer.Put(entity.ID);
        }

        public static void rPlayerID(NetDataReader reader)
        {
            Window.Client.playerId = reader.GetByte();
            Window.Client.ControlledEntityID = reader.GetInt();
            Console.WriteLine($"Client Received PlayerID & EntityID: {Window.Client.playerId} {Window.Client.ControlledEntityID}");
        }

        public static void ActionLists(NetDataWriter writer, List<ActionList> actionLists, int round)
        {
            writer.Put((byte)SMessageType.ActionLists);
            writer.Put(round);
            writer.Put(actionLists.Count);
            for (int i = 0; i < actionLists.Count; i++)
            {
                var list = actionLists[i];
                list.Write(writer);
            }
        }

        public static void rActionlists(NetDataReader reader)
        {
            Console.WriteLine("Client Read ActionLists");
            int round = reader.GetInt();
            int listCount = reader.GetInt();
            Console.WriteLine($"listCount {listCount}");
            List<ActionList> actionLists = [];
            for (int i = 0; i < listCount; i++)
            {
                ActionList list = ActionList.FromReader(reader);
                actionLists.Add(list);
            }

            // TODO:
            Client.Game.CurrentStage.FromServerActionlists = actionLists;
            Client.Game.CurrentStage.Round = round;
        }

        public static void SetGameState(NetDataWriter writer, GameState state, float time, int round)
        {
            writer.Put((byte)SMessageType.SetGameState);
            writer.Put((int)state);
            writer.Put(time);
            writer.Put(round);

        }

        public static void rGameState(NetDataReader reader)
        {
            
            GameState gameState =(GameState)reader.GetInt();
            Client.Game.SetState(gameState);
            float time = reader.GetFloat();
            int round = reader.GetInt();
            Console.WriteLine($"Client: Handle rGameState {gameState} {time} {round}");
            if(gameState == GameState.INPUT)
            {
                Client.Game.InputTimeLeft = time;
            }
            Client.Game.CurrentStage.Round = round;
        }

        public static void Entities(NetDataWriter writer, List<Entity> entities)
        {
            writer.Put((byte)SMessageType.Entities);
            writer.Put(entities.Count);
            for(int i = 0; i < entities.Count; i++)
            {
                entities[i].Write(writer);
            }
        }

        public static void rEntities(NetDataReader reader)
        {
            Stage stage = Client.Game.CurrentStage;
            stage.ClearEntities();

            int entityCount = reader.GetInt();
            for(int i = 0; i< entityCount; i++)
            {
                Entity entity = Entity.FromReader(reader);
                stage.AddEntity(entity);
            }
        }

        public static void LobbyInfo(NetDataWriter writer)
        {
            Console.WriteLine("Sending LobbyInfo..");
            writer.Put((byte)SMessageType.LobbyInfo);
            View.Lobby.Info.Write(writer);

            List<Player> players = Window.Server.players.Values.ToList();
            writer.Put(players.Count);

            players.ForEach((p) =>
            {
               p.Write(writer);
            });
        }

        public static void rLobbyInfo(NetDataReader reader)
        {
            Console.WriteLine("Receiving LobbyInfo..");
            LobbyView.LobbyInfo info = LobbyView.LobbyInfo.FromReader(reader);

            List<Player> players = [];

            int playerCount = reader.GetInt();
            for(int i = 0; i < playerCount; i++)
            {
                players.Add(Player.FromReader(reader));
            }

            View.Lobby.Info = info;
            Client.Game.SetState(GameState.LOBBY);
            LobbyView.Players = players;
        }


    }

}