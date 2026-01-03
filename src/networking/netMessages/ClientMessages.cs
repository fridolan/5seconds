using LiteNetLib.Utils;
using OpenTK.Mathematics;

// Enthält Funktionen zum Versenden und Verarbeiten von Client-Nachrichten.
// Normalerweise wird eine z.B. eine Funktion "Test" vom Client benutzt, und triggert auf Server-Seite die Funktion "rTest", welche die Nachricht von "Test" verarbeitet.
// Zusammengehörige Funktionspaare (Test u. rTest) sollten untereinander stehen.
//
// Lesende Funktionen bekommen einen reader übergeben, welcher immer zum Server gehört.

namespace fiveSeconds
{

    public enum CMessageType : byte
    {
        FullActionList
    }

    public static class ClientMessages
    {
        public static Dictionary<CMessageType, Action<NetDataReader, byte>> MessageHandlers = new()
        {
            { CMessageType.FullActionList, rFullActionList},
        };

        public static void rFullActionList(NetDataReader reader, byte playerByte)
        {
            ActionList actionList = ActionList.FromReader(reader);
            Player? player = Server.GetPlayerByByte(playerByte);
            if (player == null) return;

            if (Game.State == GameState.INPUT)
            {
                player.entity.ActionList = actionList;
            }
            else
            {
                Console.WriteLine("Ignored Client Input, as its not Input Phase anymore");
            }
        }

        public static void FullActionList(NetDataWriter writer, ActionList actionList)
        {
            writer.Put((byte)CMessageType.FullActionList);
            actionList.Write(writer);
        }
    }

}