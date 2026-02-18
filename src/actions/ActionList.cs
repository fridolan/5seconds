using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class ActionList
    {
        public List<AbilityAction> Actions = [];
        public int NextActionIndex = 0;
        public AbilityAction? NextAction => NextActionIndex >= Actions.Count ? null : Actions[NextActionIndex];
        public bool Finished => Actions.Count <= NextActionIndex;
        public long Timer = 0;
        public bool Waiting => NextAction is {Waiting: true};

        public long GetNextTiming()
        {
            if (Actions.Count > NextActionIndex)
                return Timer + Actions[NextActionIndex].NextActivationTime;
            else return long.MaxValue;
        }

        public void Act()
        {
            AbilityAction action = Actions[NextActionIndex];
            action.RoundTime = Timer;
            action.Execute();
            if (action.Finished)
            {
                NextActionIndex++;
                Timer += action.TimePassed;
            }
        }

        public void Reset()
        {
            Actions.Clear();
            NextActionIndex = 0;
            Timer = 0;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(NextActionIndex);
            writer.Put(Timer);
            writer.Put(Actions.Count);
            for(int i = 0; i < Actions.Count; i++)
            {
                AbilityAction action = Actions[i];
                action.Write(writer);
            }
        }

        public static ActionList FromReader(NetDataReader reader)
        {

            ActionList newList = new()
            {
                NextActionIndex = reader.GetInt(),
                Timer = reader.GetLong(),
            };

            Console.WriteLine($"ActionList from Reader, NextActionIndex {newList.NextActionIndex} Timer {newList.Timer}");

            List<AbilityAction> actions = [];
            int actionCount = reader.GetInt();
            for (int i = 0; i < actionCount; i++)
            {
                AbilityAction action = AbilityAction.FromReader(reader);
                actions.Add(action);
            }

            newList.Actions = actions;
            return newList;
        }

    }
}