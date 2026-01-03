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
        SetGameState
    }

    public static class ServerMessages
    {
        public static Dictionary<SMessageType, Action<NetDataReader>> MessageHandlers = new()
        {
            { SMessageType.PlayerID, rPlayerID },
            { SMessageType.ActionLists, rActionlists },
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
            Console.WriteLine("Client Received Byte");
        }

        public static void ActionLists(NetDataWriter writer, List<ActionList> actionLists)
        {
            writer.Put((byte)SMessageType.ActionLists);
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
            int listCount = reader.GetInt();
            Console.WriteLine($"listCount {listCount}");
            List<ActionList> actionLists = [];
            for (int i = 0; i < listCount; i++)
            {
                ActionList list = ActionList.FromReader(reader);
                actionLists.Add(list);
            }

            Client.Game.CurrentStage.actionListsFromServer = actionLists;
        }

        public static void SetGameState(NetDataWriter writer, GameState state, float time)
        {
            writer.Put((byte)SMessageType.SetGameState);
            writer.Put((int)state);
            writer.Put(time);
        }

        public static void rGameState(NetDataReader reader)
        {
            Client.Game.State = (GameState)reader.GetInt();
            float time = reader.GetFloat();
            if(Client.Game.State == GameState.INPUT)
            {
                Client.Game.InputTimeLeft = time;
            }
        }
    }

}