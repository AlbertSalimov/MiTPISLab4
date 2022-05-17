using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InputBox;
using System.IO;

namespace Лаб.работа__4
{
    public partial class Form1 : Form
    {
        List<Criterion> experts = new List<Criterion>();
        List<Criterion> alternatives = new List<Criterion>();
        List<double> expCoefs = new List<double>();
        List<double> altCoefs = new List<double>();

        public void ShowData(List<Criterion> c, List<double> coefs, DataGridView t)
        {
            t.Rows.Clear();
            t.Columns.Clear();
            int crCount = c.Count;
            int varCount = c[0].VariantCount;
            for (int i = 0; i < varCount; i++)
            {
                t.Columns.Add("", string.Format("Вариант {0}", i + 1));
            }
            for (int i = 0; i < crCount; i++)
            {
                t.Rows[t.Rows.Add(1)].HeaderCell.Value = string.Format("{0} ({1})", c[i].Name, c[i].Eval);
            }
            for (int i = 0; i < crCount; i++)
            {
                for (int j = 0; j < varCount; j++)
                {
                    t[j, i].Value = c[i].VariantEvals[j];
                }
            }
            if (coefs.Count != 0)
            {
                int index = t.Rows.Add(1);
                for (int i = 0; i < coefs.Count; i++)
                {
                    t[i, index].Value = coefs[i];
                }
            }
        }

        public void Normalize(List<Criterion> c)
        {
            double sum = c.Sum(e => e.Eval);
            for (int i = 0; i < c.Count; i++)
            {
                //c[i].Eval /= sum;
                c[i].Eval = Math.Round(c[i].Eval / sum, 2);
                double varSum = c[i].VariantEvals.Sum();
                for (int j = 0; j < c[i].VariantEvals.Count; j++)
                {
                    //c[i].VariantEvals[j] /= varSum;
                    c[i].VariantEvals[j] = Math.Round(c[i].VariantEvals[j] / varSum, 2);
                }
            }
        }

        public void CalcCouplingCoefs(List<Criterion> c, List<double> coefs)
        {
            coefs.Clear();
            int crCnt = c.Count;
            int varCnt = c[0].VariantCount;
            for (int i = 0; i < varCnt; i++)
            {
                double coef = 0;
                for (int j = 0; j < crCnt; j++)
                {
                    coef += c[j].Eval * c[j].VariantEvals[i];
                }
                coefs.Add(coef);
            }
        }

        public void LoadFromFile(List<Criterion> c, string filename)
        {
            c.Clear();
            StreamReader sr = new StreamReader(string.Format("{0}.txt", filename));
            int crCnt = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < crCnt; i++)
            {
                Criterion cr = new Criterion();
                cr.Load(sr);
                c.Add(cr);
            }
            sr.Close();
        }

        public void SaveToFile(List<Criterion> c, string filename)
        {
            StreamWriter sw = new StreamWriter(string.Format("{0}.txt", filename));
            sw.WriteLine(c.Count);
            foreach (Criterion cr in c)
            {
                cr.Save(sw);
            }
            sw.Close();
        }

        public void CalcAndShowResults(List<double> eC, List<double> aC, Label l)
        {
            l.Text = string.Empty;
            int ecCnt = eC.Count;
            int aCCnt = aC.Count;
            for (int i = 0; i < ecCnt; i++)
            {
                for (int j = 0; j < aCCnt; j++)
                {
                    l.Text += string.Format("({0}.{1}): {2}*{3} = {4}\n\n", i + 1, j + 1, eC[i], aC[j], eC[i] * aC[j]);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            experts.Clear();
            frmInputBox ib = new frmInputBox();
            int crCnt = Convert.ToInt32(ib.InputBox("", "Введите количество критериев", "0"));
            int expCnt = Convert.ToInt32(ib.InputBox("", "Введите количество экспертов", "0"));
            for (int i = 0; i < crCnt; i++)
            {
                string crName = ib.InputBox("", string.Format("Введите имя {0}-го критерия", i + 1), "");
                double crEval = Convert.ToDouble(ib.InputBox("", string.Format("Введите оценку {0}-го критерия", i + 1), ""));
                List<double> varEvals = new List<double>();
                for (int j = 0; j < expCnt; j++)
                {
                    double ev = Convert.ToDouble(ib.InputBox("", string.Format("Введите оценку для {0}-го эксперта по критерию \"{1}\"", j + 1, crName), ""));
                    varEvals.Add(ev);
                }
                Criterion cr = new Criterion(crName, crEval, varEvals);
                experts.Add(cr);
            }
            ShowData(experts, expCoefs, tExp);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            alternatives.Clear();
            frmInputBox ib = new frmInputBox();
            int crCnt = Convert.ToInt32(ib.InputBox("", "Введите количество критериев", "0"));
            int altCnt = Convert.ToInt32(ib.InputBox("", "Введите количество альтернатив", "0"));
            for (int i = 0; i < crCnt; i++)
            {
                string crName = ib.InputBox("", string.Format("Введите имя {0}-го критерия", i + 1), "");
                double crEval = Convert.ToDouble(ib.InputBox("", string.Format("Введите оценку {0}-го критерия", i + 1), ""));
                List<double> varEvals = new List<double>();
                for (int j = 0; j < altCnt; j++)
                {
                    double ev = Convert.ToDouble(ib.InputBox("", string.Format("Введите оценку для {0}-й альтернативы по критерию \"{1}\"", j + 1, crName), ""));
                    varEvals.Add(ev);
                }
                Criterion cr = new Criterion(crName, crEval, varEvals);
                alternatives.Add(cr);
            }
            ShowData(alternatives, altCoefs, tAlternatives);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Normalize(experts);
            Normalize(alternatives);
            CalcCouplingCoefs(experts, expCoefs);
            CalcCouplingCoefs(alternatives, altCoefs);
            ShowData(experts, expCoefs, tExp);
            ShowData(alternatives, altCoefs, tAlternatives);
            CalcAndShowResults(expCoefs, altCoefs, label3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveToFile(experts, "experts");
            SaveToFile(alternatives, "alternatives");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expCoefs.Clear();
            altCoefs.Clear();
            LoadFromFile(experts, "experts");
            ShowData(experts, expCoefs, tExp);
            LoadFromFile(alternatives, "alternatives");
            ShowData(alternatives, altCoefs, tAlternatives);
        }
    }

    public class Criterion
    {
        public string Name;
        public double Eval;
        public List<double> VariantEvals;
        public int VariantCount;

        public Criterion()
        {
            Name = string.Empty;
            Eval = 0.0;
            VariantEvals = new List<double>();
            VariantCount = 0;
        }

        public Criterion(string n, double e, List<double> vE)
        {
            Name = n;
            Eval = e;
            VariantEvals = vE;
            VariantCount = vE.Count();
        }

        public void Load(StreamReader sr)
        {
            Name = sr.ReadLine();
            Eval = Convert.ToDouble(sr.ReadLine());
            VariantCount = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < VariantCount; i++)
            {
                VariantEvals.Add(Convert.ToDouble(sr.ReadLine()));
            }
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine(Name);
            sw.WriteLine(Eval);
            sw.WriteLine(VariantCount);
            foreach (double e in VariantEvals)
            {
                sw.WriteLine(e);
            }
        }
    }
}
