using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using System.Globalization;

namespace FsmCalc
{
    public enum CalcState
    {
        NoData,
        EnterDigit,
        EnterDigitNoZero
    }

    public enum CalcEvent
    {
        Digit,
        Clear,
        Plus,
        Moins,
        Mult,
        Div,
        Sign
    }

    public interface ICalcBehaviors
    {
        void Digit(string aDigit);
        void Clear();
        void Plus();
        void Moins();
        void Mult();
        void Div();
        void Sign();
    }

    public class CalcActor : FsmActor<CalcState, Tuple<CalcEvent, string>>, ICalcBehaviors
    {
        private List<IActor> observers = new List<IActor>();

        public CalcActor() : base()
        {
            Become(new CalcBehavior());
            AddBehavior(new Behavior<Accumulators>(DoAcc));
            AddBehavior(new Behavior<IActor>(DoRegister));
        }

        public void Register(IActor actor)
        {
            this.SendMessage(actor);
        }

        private void DoRegister(IActor actor)
        {
            if (observers.Contains(actor))
            {
                observers.Remove(actor);
            }
            else
            {
                observers.Add(actor);
            }
        }

        private void DoAcc(Accumulators acc)
        {
            foreach (var item in observers)
            {
                item.SendMessage(acc);
            }
        }

        public void Clear()
        {
            this.SendMessage(Tuple.Create(CalcEvent.Clear, ""));
        }

        public void Digit(string aDigit)
        {
            this.SendMessage(Tuple.Create(CalcEvent.Digit, aDigit));
        }

        public void Div()
        {
            this.SendMessage(Tuple.Create(CalcEvent.Div, ""));
        }

        public void Moins()
        {
            this.SendMessage(Tuple.Create(CalcEvent.Moins, ""));
        }

        public void Mult()
        {
            this.SendMessage(Tuple.Create(CalcEvent.Mult, ""));
        }

        public void Plus()
        {
            this.SendMessage(Tuple.Create(CalcEvent.Plus, ""));
        }

        public void Sign()
        {
            this.SendMessage(Tuple.Create(CalcEvent.Sign, ""));
        }
    }

    public class Accumulators
    {
        public string Data { get; private set; }
        public int Accumulator { get; private set; }
        public int PlusAcc { get; private set; }
        public int MoinsAcc { get; private set; }
        public int MultAcc { get; private set; }
        public int DivAcc { get; private set; }
        public Accumulators Clone()
        {
            return new Accumulators()
            {
                Data = this.Data,
                Accumulator = this.Accumulator,
                DivAcc = this.DivAcc,
                MoinsAcc = this.MoinsAcc,
                MultAcc = this.MultAcc,
                PlusAcc = this.PlusAcc
            };
        }

        public Accumulators Enter(string data)
        {
            var clone = Clone();
            clone.Data = Data + data;
            int value = int.Parse(clone.Data, CultureInfo.InvariantCulture);
            clone.PlusAcc = this.Accumulator + value;
            clone.MoinsAcc = this.Accumulator - value;
            clone.MultAcc = this.Accumulator * value;
            clone.DivAcc = this.Accumulator / value;
            return clone;
        }

        public Accumulators Plus()
        {
            var clone = new Accumulators()
            {
                Accumulator = this.PlusAcc
            };
            clone.PlusAcc = clone.Accumulator ;
            clone.MoinsAcc = clone.Accumulator ;
            clone.MultAcc = 0 ;
            clone.DivAcc = 0;
            return clone;
        }

        public Accumulators Moins()
        {
            var clone = new Accumulators()
            {
                Accumulator = this.MoinsAcc
            };
            clone.PlusAcc = clone.Accumulator;
            clone.MoinsAcc = clone.Accumulator;
            clone.MultAcc = 0;
            clone.DivAcc = 0;
            return clone;
        }

        public Accumulators Div()
        {
            var clone = new Accumulators()
            {
                Accumulator = this.DivAcc
            };
            clone.PlusAcc = clone.Accumulator;
            clone.MoinsAcc = clone.Accumulator;
            clone.MultAcc = 0;
            clone.DivAcc = 0;
            return clone;
        }

        public Accumulators Mult()
        {
            var clone = new Accumulators()
            {
                Accumulator = this.MultAcc
            };
            clone.PlusAcc = clone.Accumulator;
            clone.MoinsAcc = clone.Accumulator;
            clone.MultAcc = 0;
            clone.DivAcc = 0;
            return clone;
        }

        public Accumulators Sign()
        {
            var clone = Clone();
            clone.Data = Data.StartsWith("-",StringComparison.Ordinal) ? Data.Substring(1, Data.Length - 1) : "-" + Data;
            int value = int.Parse(clone.Data,CultureInfo.InvariantCulture);
            clone.PlusAcc = this.Accumulator + value;
            clone.MoinsAcc = this.Accumulator - value;
            clone.MultAcc = this.Accumulator * value;
            clone.DivAcc = this.Accumulator / value;
            return clone;
        }
    }

    public class CalcBehavior : FsmBehaviors<CalcState, Tuple<CalcEvent, string>>
    {
        private Accumulators accumulators = new Accumulators();

        public CalcBehavior() : base()
        {
            AddRule(CalcState.NoData,
                e => e.Item1 == CalcEvent.Digit && e.Item2 != "0",
                e => Update(e), CalcState.EnterDigit)
           .AddRule(CalcState.EnterDigitNoZero, e => e.Item1 == CalcEvent.Digit && e.Item2 != "0", e => Update(e), CalcState.EnterDigit)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Digit, e => Update(e), CalcState.EnterDigit)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Sign, e => Update(e), CalcState.EnterDigit)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Clear, e => Update(e), CalcState.NoData)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Plus, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Moins, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Mult, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigit, e => e.Item1 == CalcEvent.Div, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigitNoZero, e => e.Item1 == CalcEvent.Div, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigitNoZero, e => e.Item1 == CalcEvent.Plus, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigitNoZero, e => e.Item1 == CalcEvent.Moins, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigitNoZero, e => e.Item1 == CalcEvent.Mult, e => Update(e), CalcState.EnterDigitNoZero)
           .AddRule(CalcState.EnterDigitNoZero, e => e.Item1 == CalcEvent.Clear, e => Update(e), CalcState.NoData);
        }

        private void Update(Tuple<CalcEvent, string> evt)
        {
            string data = evt.Item2;
            switch (evt.Item1)
            {
                case CalcEvent.Clear: { accumulators = new Accumulators(); LinkedActor.SendMessage(accumulators); break; }
                case CalcEvent.Digit: { accumulators = accumulators.Enter(data); LinkedActor.SendMessage(accumulators); break; }
                case CalcEvent.Plus: { accumulators = accumulators.Plus(); LinkedActor.SendMessage(accumulators); break; }
                case CalcEvent.Moins: { accumulators = accumulators.Moins(); LinkedActor.SendMessage(accumulators); break; }
                case CalcEvent.Mult: { accumulators = accumulators.Mult(); LinkedActor.SendMessage(accumulators); break; }
                case CalcEvent.Div: { accumulators = accumulators.Div(); LinkedActor.SendMessage(accumulators); break; }
                case CalcEvent.Sign:
                    {
                        accumulators = accumulators.Sign();
                        LinkedActor.SendMessage(accumulators);
                        break;
                    }
                default: break;
            }
        }
    }
}
