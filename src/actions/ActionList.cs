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

        public void Act()
        {
            SAction action = actions[NextActionIndex];
            action.Execute();
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
    }
}