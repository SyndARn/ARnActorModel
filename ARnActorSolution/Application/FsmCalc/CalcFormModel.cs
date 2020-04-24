using Actor.Base;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsmCalc
{
    public class CalcFormModel
    {
        private CalcActor CalcActor;
        private readonly StateFullActor<Accumulators> _observer;
        private Form1 Form;

        public CalcFormModel(Form1 aForm)
        {
            Form = aForm;
            CalcActor = new CalcActor();
            _observer = new StateFullActor<Accumulators>();
            CalcActor.Register(_observer);
        }

        public string Enter { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public async Task<CalcState> GetCurrentState()
        {
            return await CalcActor.GetCurrentState().ResultAsync();
        }

        public async Task Process(string order)
        {
            switch(order)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    {
                        CalcActor.Digit(order);
                        break;
                    }
                case "+": CalcActor.Plus(); break;
                case "-": CalcActor.Moins(); break;
                case "/": CalcActor.Div(); break;
                case "*": CalcActor.Mult(); break;
                case "Clear": CalcActor.Clear(); break;
                case "+/-": CalcActor.Sign();break;
            }
            var accumulators = await _observer.GetStateAsync();
            Form.lblAccumulator.Text = accumulators.Accumulator.ToString();
            Form.lblPlusAcc.Text = accumulators.PlusAcc.ToString();
            Form.lblMoinsAcc.Text = accumulators.MoinsAcc.ToString();
            Form.lblMultAcc.Text = accumulators.MultAcc.ToString();
            Form.lblDivAcc.Text = accumulators.DivAcc.ToString();
            Form.lblEnter.Text = accumulators.Data;
        }
        
    }
}
