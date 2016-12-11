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
        private StateFullActor<Accumulators> Observer;
        private Form1 Form;

        public CalcFormModel(Form1 aForm)
        {
            Form = aForm;
            CalcActor = new CalcActor();
            Observer = new StateFullActor<Accumulators>();
            CalcActor.Register(Observer);
        }

        public string Enter { get; set; }

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
            var accumulators = await Observer.GetStateAsync();
            Form.lblAccumulator.Text = accumulators.Accumulator.ToString();
            Form.lblPlusAcc.Text = accumulators.PlusAcc.ToString();
            Form.lblMoinsAcc.Text = accumulators.MoinsAcc.ToString();
            Form.lblMultAcc.Text = accumulators.MultAcc.ToString();
            Form.lblDivAcc.Text = accumulators.DivAcc.ToString();
            Form.lblEnter.Text = accumulators.Data;
        }
        
    }
}
