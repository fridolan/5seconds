using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class ActionList
    {
        public List<SAction> actions = [];
        public int NextActionIndex = 0;
        public bool Finished => actions.Count <= NextActionIndex;
        public float timer = 0;

        public float GetNextTiming()
        {
            if (actions.Count > NextActionIndex)
                return timer + actions[NextActionIndex].NextActivationTime;
            else return float.MaxValue;
        }

        public void Act(Game game)
        {
            SAction action = actions[NextActionIndex];
            action.Execute(game);
            if (action.Finished)
            {
                NextActionIndex++;
                timer += action.TimePassed;
            }
        }

        public void Reset()
        {
            actions.Clear();
            NextActionIndex = 0;
            timer = 0;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(NextActionIndex);
            writer.Put(timer);
            writer.Put(actions.Count);
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Write(writer);
            }
        }

        public static ActionList FromReader(NetDataReader reader)
        {

            ActionList newList = new()
            {
                NextActionIndex = reader.GetInt(),
                timer = reader.GetFloat(),
            };

            Console.WriteLine($"ActionList from Reader, NextActionIndex {newList.NextActionIndex} Timer {newList.timer}");

            List<SAction> actions = [];
            int actionCount = reader.GetInt();
            for (int i = 0; i < actionCount; i++)
            {
                SAction action = SAction.FromReader(reader);
                actions.Add(action);
            }

            newList.actions = actions;
            return newList;
        }

        public void AddActionClient(SAction action)
        {
            actions.Add(action);
            ClientMessages.FullActionList(Window.Client.writer, this);
        }


    }
}