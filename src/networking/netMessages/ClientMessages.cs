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
    }

    public static class ClientMessages
    {
        public static Dictionary<CMessageType, Action<NetDataReader>> MessageHandlers = new()
        {
            
        };
    }

}