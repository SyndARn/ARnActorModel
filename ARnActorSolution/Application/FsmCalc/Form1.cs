using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace FsmCalc
{
    public partial class Form1 : Form
    {
        private CalcFormModel FormModel;
        public Form1()
        {
            InitializeComponent();
            FormModel = new CalcFormModel(this);
            foreach(var item in Controls.OfType<Button>())
            {
                item.Click += this.CalcEventClick;
            }
            foreach (var item in Controls.OfType<Label>())
            {
                item.Click += this.CalcEventClick;
            }
        }

        private async void CalcEventClick(object sender, EventArgs e)
        {
            var lbl = sender as Label;
            if (lbl != null)
            {
                await FormModel.Process(lbl.Text);
                return;
            }

            var btt = sender as Button;
            if (btt != null)
            {
                await FormModel.Process(btt.Text);
            }
        }

    }
}
