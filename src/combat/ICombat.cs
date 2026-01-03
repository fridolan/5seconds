using LiteNetLib.Utils;

namespace fiveSeconds
{
    public interface ICombat
    {
        public Stats BaseStats {get; set;}
        public Stats Stats { get; set; }

        public void Write(NetDataWriter writer)
        {
            BaseStats.Write(writer);
            Stats.Write(writer);
        }

        public void Read(NetDataReader reader)
        {
            BaseStats = Stats.FromReader(reader);
            Stats = Stats.FromReader(reader);
        }
    }


}