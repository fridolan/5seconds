using LiteNetLib.Utils;
// Enthält Funktionen zum Versenden und Verarbeiten von Server-Nachrichten.
// Normalerweise wird eine z.B. eine Funktion "Test" vom Server benutzt, und triggert auf Client-Seite die Funktion "rTest", welche die Nachricht von "Test" verarbeitet.
// Zusammengehörige Funktionspaare (Test u. rTest) sollten untereinander stehen.
//
// Schreibende Funktionen bekommen normalerweise einen writer übergeben, und sind daher im normalfall unabhängig vom Empfänger.
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
    }

    public static class ServerMessages
    {
        public static Dictionary<SMessageType, Action<NetDataReader>> MessageHandlers = new()
        {
            { SMessageType.PlayerID,(reader) =>
            {
                Window.Client.playerId = reader.GetByte();
                Window.Client.ControlledEntityID = reader.GetInt();
                Console.WriteLine("Client Received Byte");
            } },
        };

        public static void PlayerID(NetDataWriter writer, byte playerID, Entity entity)
        {
            writer.Put((byte)SMessageType.PlayerID);
            writer.Put(playerID);
            writer.Put(entity.ID);
        }
    }

}