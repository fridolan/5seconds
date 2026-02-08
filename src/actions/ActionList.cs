using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class ActionList
    {
        public List<SAction> Actions = [];
        public int NextActionIndex = 0;
        public SAction? NextAction => NextActionIndex >= Actions.Count ? null : Actions[NextActionIndex];
        public bool Finished => Actions.Count <= NextActionIndex;
        public float Timer = 0;
        public bool Waiting => NextAction is {Waiting: true};

        public float GetNextTiming()
        {
            if (Actions.Count > NextActionIndex)
                return Timer + Actions[NextActionIndex].NextActivationTime;
            else return float.MaxValue;
        }

        public void Act(Game game)
        {
            SAction action = Actions[NextActionIndex];
            action.Execute(game);
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
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Write(writer);
            }
        }

        public static ActionList FromReader(NetDataReader reader)
        {

            ActionList newList = new()
            {
                NextActionIndex = reader.GetInt(),
                Timer = reader.GetFloat(),
            };

            Console.WriteLine($"ActionList from Reader, NextActionIndex {newList.NextActionIndex} Timer {newList.Timer}");

            List<SAction> actions = [];
            int actionCount = reader.GetInt();
            for (int i = 0; i < actionCount; i++)
            {
                SAction action = SAction.FromReader(reader);
                actions.Add(action);
            }

            newList.Actions = actions;
            return newList;
        }

    }
}