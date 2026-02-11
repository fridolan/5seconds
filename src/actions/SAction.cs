using LiteNetLib.Utils;

namespace fiveSeconds
{
    public abstract class SAction
    {
        public float TimePassed = 0;
        public float NextActivationTime = 0;
        public Action<Game> NextActivation;
        public bool Finished = false;
        public bool Begun = false;
        public bool Waiting = false;
        public virtual int Icon => 0;

        public void Execute(Game game)
        {
            float now = NextActivationTime;
            if (Begun == false)
            {
                Begin(game);
                Begun = true;
            }
            else NextActivation(game);
            TimePassed = now;
        }

        public abstract void Begin(Game game);

        public static SAction FromReader(NetDataReader reader)
        {

            int typeIndex = reader.GetInt();
            SAction action = GetInstance[typeIndex]();

            action.TimePassed = reader.GetFloat();
            action.NextActivationTime = reader.GetFloat();
            action.Finished = reader.GetBool();
            action.Begun = reader.GetBool();

            Console.WriteLine($"SAction from reader, typeI {typeIndex}, TimePassed {action.TimePassed}, NextActivationTime {action.NextActivationTime}, Finished {action.Finished}, Begun {action.Begun}");

            if (action is IStartGoalInput sgi)
            {
                sgi.Start = (reader.GetInt(), reader.GetInt());
                sgi.Goal = (reader.GetInt(), reader.GetInt());
            }

            if (action is IEntityIDInput ei)
            {
                ei.EntityID = reader.GetInt();
            }

            if (action is ICancelOnDisplaceInput cod)
            {
                cod.CancelOnDisplace = reader.GetBool();
            }

            if (action is IRelativeInput r)
            {
                r.Relative = reader.GetBool();
            }

            if (action is IToEntityIDInput tei)
            {
                tei.ToEntityID = reader.GetInt();
            }

            if(action is IWaitInput w)
            {
                w.Wait = reader.GetBool();
            }

            return action;
        }

        public void Write(NetDataWriter writer)
        {
            SAction action = this;

            writer.Put(GetTypeIndex[action.GetType()]);

            writer.Put(TimePassed);
            writer.Put(NextActivationTime);
            writer.Put(Finished);
            writer.Put(Begun);

            if (action is IStartGoalInput sgi)
            {
                writer.Put(sgi.Start.X);
                writer.Put(sgi.Start.Y);
                writer.Put(sgi.Goal.X);
                writer.Put(sgi.Goal.Y);
            }

            if (action is IEntityIDInput ei)
            {
                writer.Put(ei.EntityID);
            }

            if (action is ICancelOnDisplaceInput cod)
            {
                writer.Put(cod.CancelOnDisplace);
            }

            if (action is IRelativeInput r)
            {
                writer.Put(r.Relative);
            }

            if (action is IToEntityIDInput tei)
            {
                writer.Put(tei.ToEntityID);
            }

            if(action is IWaitInput w)
            {
                writer.Put(w.Wait);
            }
        }

        public static Dictionary<Type, int> GetTypeIndex = new(){
          { typeof(MoveEntityAction), 0 },
          { typeof(CatchEntityAction), 1 },
          { typeof(MeleeAttackEntityAction), 2},
        };

        public static List<Func<SAction>> GetInstance = [
            () => new MoveEntityAction(),
            () => new CatchEntityAction(),
            () => new MeleeAttackEntityAction(),
        ];
    }
}