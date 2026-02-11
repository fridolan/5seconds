using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class Player
    {
        // Only Serverside
        public int ClientId;
        // Both
        public byte ID;
        public Entity Entity;


        public static Player FromReader(NetDataReader reader)
        {
            Player player = new()
            {
                ClientId = reader.GetInt(),
                ID = reader.GetByte()
            };

            return player;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(ClientId);
            writer.Put(ID);
        }
    }
}