using System.ComponentModel;
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
        FullActionList,
        UpdateDone,
    }

    public static class ClientMessages
    {
        public static Dictionary<CMessageType, Action<NetDataReader, byte>> MessageHandlers = new()
        {
            { CMessageType.FullActionList, rFullActionList},
            { CMessageType.UpdateDone, rUpdateDone},
        };

        public static void FullActionList(NetDataWriter writer, ActionList actionList)
        {
            writer.Put((byte)CMessageType.FullActionList);
            Console.WriteLine("Client sends ActionList");
            actionList.Write(writer);
        }

        public static void rFullActionList(NetDataReader reader, byte playerByte)
        {
            Console.WriteLine("Server receives Client ActionList");
            ActionList actionList = ActionList.FromReader(reader);
            Player? player = Server.GetPlayerByByte(playerByte);
            if (player == null) return;

            if (Client.Game.State == GameState.INPUT)
            {
                player.Entity.ActionList = actionList;
            }
            else
            {
                Console.WriteLine("Ignored Client Input, as its not Input Phase anymore");
            }
        }

        public static void UpdateDone(NetDataWriter writer, int round)
        {
            writer.Put((byte)CMessageType.UpdateDone);
            Console.WriteLine("Own Client update is done, informing Server..");
            writer.Put(round);
        }

        public static void rUpdateDone(NetDataReader reader, byte playerByte)
        {
            Console.WriteLine("Server receives Client UpdateDoneConfirmation");
            int round = reader.GetInt();
            if(round + 1 < Client.Game.CurrentStage.Round) throw new Exception($"Desync Client: {playerByte} with round {round} instead of {Client.Game.CurrentStage.Round}");
            Client.Game.ClientsFinishedUpdate[playerByte] = true;
        }
    }

}