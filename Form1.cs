using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace OPR_5
{
    public partial class Form1 : Form
    {

        List<double> f1 = new List<double>(5) { 1.6, 1.8, 1.9, 2, 2.3 };
        List<double> f2 = new List<double>(5) { 1.5, 1.9, 2.3, 2.4, 2.7 };
        List<double> f3 = new List<double>(5) { 1.9, 2.3, 2.5, 2.6, 2.7 };
        List<double> f4 = new List<double>(5) { 1.8, 1.9, 2.1, 2.3, 2.5 };
                                                                                       
        List<List<double>> list = new List<List<double>>(4);
                                                                                     
        List<double> maxesStep1 = new List<double>();
        List<int> indexesStep1 = new List<int>();
        List<double> maxesStep2 = new List<double>();
        List<int> indexesStep2 = new List<int>();
        List<double> maxesStep3 = new List<double>();
        List<int> indexesStep3 = new List<int>();

        public Form1()
        {
            InitializeComponent();
            DGV.ColumnCount = 5;
            DGV.RowCount = 4;
            DGV.Rows[0].HeaderCell.Value = "f1";
            DGV.Rows[1].HeaderCell.Value = "f2";
            DGV.Rows[2].HeaderCell.Value = "f3";
            DGV.Rows[3].HeaderCell.Value = "f4";
            for (int i = 1; i <= 5; i++)
            {
                DGV.Columns[i - 1].HeaderCell.Value = i.ToString();
                DGV.Rows[0].Cells[i - 1].Value = f1[i - 1].ToString();
                DGV.Rows[1].Cells[i - 1].Value = f2[i - 1].ToString();
                DGV.Rows[2].Cells[i - 1].Value = f3[i - 1].ToString();
                DGV.Rows[3].Cells[i - 1].Value = f4[i - 1].ToString();
            }
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            list.Add(f1);
            list.Add(f2);
            list.Add(f3);
            list.Add(f4);
        }
        private void PrintList(List<List<double>> list, RichTextBox rtb)
        {
            foreach (var l in list)
            {
                foreach (var el in l)
                    rtb.Text += String.Format("{0, 3}  ", el);
                rtb.Text += "\n";
            }
            rtb.Text += "\n";
        }
        private bool CheckData()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 5; j++)
                {
                    if (!double.TryParse(DGV.Rows[i].Cells[j].Value.ToString(), out _))
                    {
                        MessageBox.Show($"В ячейке [{i + 1};{j + 1}] содержится не число!", "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (Math.Sign(Convert.ToDouble(DGV.Rows[i].Cells[j].Value)) != 1)
                    {
                        MessageBox.Show($"В ячейке [{i + 1};{j + 1}] содержится не положительное число!", "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            return true;
        }
        private void GetData(bool flag)
        {
            if (!CheckData()) return;
            if (flag)
            {
                for (int i = 0; i < DGV.Columns.Count; i++)
                {
                    f1[i] = Convert.ToDouble(DGV.Rows[0].Cells[i].Value);
                    f2[i] = Convert.ToDouble(DGV.Rows[1].Cells[i].Value);
                    f3[i] = Convert.ToDouble(DGV.Rows[2].Cells[i].Value);
                    f4[i] = Convert.ToDouble(DGV.Rows[3].Cells[i].Value);
                }
            }
            else
            {
                for (int i = 0; i < DGV.Columns.Count; i++)
                {
                    f1[i] = Convert.ToDouble(DGV.Rows[3].Cells[i].Value);
                    f2[i] = Convert.ToDouble(DGV.Rows[2].Cells[i].Value);
                    f3[i] = Convert.ToDouble(DGV.Rows[1].Cells[i].Value);
                    f4[i] = Convert.ToDouble(DGV.Rows[0].Cells[i].Value);
                }
            }
        }
        private void ClearAll()
        {
            DGVStep1_1.Rows.Clear();
            DGVStep1_2.Rows.Clear();
            DGVStep2_1.Rows.Clear();
            DGVStep2_2.Rows.Clear();
            DGVStep3_1.Rows.Clear();
            DGVStep3_2.Rows.Clear();
            DGVlast.Rows.Clear();
            maxesStep1.Clear();
            indexesStep1.Clear();
            maxesStep2.Clear();
            indexesStep2.Clear();
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            GetData(true);
            ClearAll();
            Step(f2, f1, 1, DGVStep1_1, DGVStep1_2, maxesStep1, indexesStep1, 1);
            Step(f3, maxesStep1, 2, DGVStep2_1, DGVStep2_2, maxesStep2, indexesStep2, 1);
            StepLast(f4, maxesStep2, 3, DGVStep3_1, DGVStep3_2, maxesStep3, indexesStep3, 1);
            Finals(1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            GetData(false);
            ClearAll();
            Step(f2, f1, 3, DGVStep1_1, DGVStep1_2, maxesStep1, indexesStep1, 2);
            Step(f3, maxesStep1, 2, DGVStep2_1, DGVStep2_2, maxesStep2, indexesStep2, 2);
            StepLast(f4, maxesStep2, 1, DGVStep3_1, DGVStep3_2, maxesStep3, indexesStep3, 2);
            Finals(2);
        }
        private void Step(List<double> listVertical,
            List<double> listHorizontal,
            int num,
            DataGridView DGVbig,
            DataGridView DGVsmall,
            List<double> maxesList,
            List<int> indexesList,
            int type)
        {
            Application.DoEvents();
            #region BigTable
            DGVbig.ColumnCount = 7;
            DGVbig.RowCount = 7;
            DGVsmall.ColumnCount = 6;
            DGVsmall.RowCount = 2;

            if (type == 1)
            {
                DGVbig.Rows[0].HeaderCell.Value = $"x{num + 1}";
                DGVbig.Columns[0].HeaderCell.Value = $"x{num}";
                DGVbig.Rows[0].Cells[0].Value = $"f{num + 1}(x{num + 1})/f{num}(x{num})";
                DGVsmall.Columns[0].HeaderCell.Value = $"s{num}";
                DGVsmall.Rows[0].Cells[0].Value = $"f{num + 1}(s{num})";
                DGVsmall.Rows[1].Cells[0].Value = $"x{num + 1}";
            }
            else
            {
                DGVbig.Rows[0].HeaderCell.Value = $"x{num}";
                DGVbig.Columns[0].HeaderCell.Value = $"x{num + 1}";
                DGVbig.Rows[0].Cells[0].Value = $"f{num}(x{num})/f{num + 1}(x{num + 1})";
                DGVsmall.Columns[0].HeaderCell.Value = $"s{num + 1}";
                DGVsmall.Rows[0].Cells[0].Value = $"f{num}(s{num + 1})";
                DGVsmall.Rows[1].Cells[0].Value = $"x{num}";
            }

            for (int i = 1; i < 7; i++)
            {
                DGVbig.Rows[i].HeaderCell.Value = (i - 1).ToString();
                DGVbig.Columns[i].HeaderCell.Value = (i - 1).ToString();
            }

            DGVbig.Rows[0].Cells[0].Style.BackColor = Color.LightGray;
            DGVbig.Rows[1].Cells[0].Value = 0;
            DGVbig.Rows[1].Cells[0].Style.BackColor = Color.LightGray;
            DGVbig.Rows[0].Cells[1].Value = 0;
            DGVbig.Rows[0].Cells[1].Style.BackColor = Color.LightGray;

            for (int i = 2; i < 7; i++) //вот оно, заполнение
            {
                DGVbig.Rows[i].Cells[0].Value = listVertical[i - 2];
                DGVbig.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                DGVbig.Rows[0].Cells[i].Value = listHorizontal[i - 2];
                DGVbig.Rows[0].Cells[i].Style.BackColor = Color.LightGray;
            }

            for (int i = 1; i < 7; i++)
                for (int j = 1; j < 7; j++)
                {
                    if (i + j < 8)
                        DGVbig.Rows[i].Cells[j].Value = Convert.ToDouble(DGVbig.Rows[i].Cells[0].Value) +
                                                            Convert.ToDouble(DGVbig.Rows[0].Cells[j].Value);
                    else DGVbig.Rows[i].Cells[j].Value = 0;
                }
            DGVbig.CurrentCell.Selected = false;

            List<Tuple<double, int>> l1 = new List<Tuple<double, int>>(2);
            List<Tuple<double, int>> l2 = new List<Tuple<double, int>>(2);
            List<Tuple<double, int>> l3 = new List<Tuple<double, int>>(2);
            List<Tuple<double, int>> l4 = new List<Tuple<double, int>>(2);
            List<Tuple<double, int>> l5 = new List<Tuple<double, int>>(2);

            for (int i = 1; i < 7; i++) //rows
                for (int j = 1; j < 7; j++) //columns
                {
                    switch (i + j)
                    {
                        case 3:
                            l1.Add(new Tuple<double, int>(Convert.ToDouble(DGVbig.Rows[i].Cells[j].Value), i - 1));
                            DGVbig.Rows[i].Cells[j].Style.BackColor = Color.LightGreen;
                            break;
                        case 4:
                            l2.Add(new Tuple<double, int>(Convert.ToDouble(DGVbig.Rows[i].Cells[j].Value), i - 1));
                            DGVbig.Rows[i].Cells[j].Style.BackColor = Color.LightSkyBlue;
                            break;
                        case 5:
                            l3.Add(new Tuple<double, int>(Convert.ToDouble(DGVbig.Rows[i].Cells[j].Value), i - 1));
                            DGVbig.Rows[i].Cells[j].Style.BackColor = Color.LightSalmon;
                            break;
                        case 6:
                            l4.Add(new Tuple<double, int>(Convert.ToDouble(DGVbig.Rows[i].Cells[j].Value), i - 1));
                            DGVbig.Rows[i].Cells[j].Style.BackColor = Color.LightYellow;
                            break;
                        case 7:
                            l5.Add(new Tuple<double, int>(Convert.ToDouble(DGVbig.Rows[i].Cells[j].Value), i - 1));
                            DGVbig.Rows[i].Cells[j].Style.BackColor = Color.LightSteelBlue;
                            break;
                    }
                }

            maxesList.Add(Aboba(l1).Item1);
            maxesList.Add(Aboba(l2).Item1);
            maxesList.Add(Aboba(l3).Item1);
            maxesList.Add(Aboba(l4).Item1);
            maxesList.Add(Aboba(l5).Item1);
            indexesList.Add(Aboba(l1).Item2);
            indexesList.Add(Aboba(l2).Item2);
            indexesList.Add(Aboba(l3).Item2);
            indexesList.Add(Aboba(l4).Item2);
            indexesList.Add(Aboba(l5).Item2);
            #endregion

            #region SmallTable
            for (int i = 1; i < 6; i++)
            {
                DGVsmall.Columns[i].HeaderCell.Value = i.ToString();
                DGVsmall.Rows[0].Cells[i].Value = maxesList[i - 1];
                DGVsmall.Rows[1].Cells[i].Value = indexesList[i - 1];
            }

            DGVsmall.CurrentCell.Selected = false;
            #endregion
        }
        private void StepLast(List<double> listVertical,
            List<double> listHorizontal,
            int num,
            DataGridView DGVbig,
            DataGridView DGVsmall,
            List<double> maxesList,
            List<int> indexesList,
            int type)
        {
            Application.DoEvents();
            #region BigTable
            DGVbig.ColumnCount = 7;
            DGVbig.RowCount = 7;
            DGVsmall.ColumnCount = 6;
            DGVsmall.RowCount = 2;

            if (type == 1)
            {
                DGVbig.Rows[0].HeaderCell.Value = $"x{num + 1}";
                DGVbig.Columns[0].HeaderCell.Value = $"x{num}";
                DGVbig.Rows[0].Cells[0].Value = $"f{num + 1}(x{num + 1})/f{num}(x{num})";
                DGVsmall.Columns[0].HeaderCell.Value = $"s{num}";
                DGVsmall.Rows[0].Cells[0].Value = $"f{num + 1}(s{num})";
                DGVsmall.Rows[1].Cells[0].Value = $"x{num + 1}";
            }
            else
            {
                DGVbig.Rows[0].HeaderCell.Value = $"x{num}";
                DGVbig.Columns[0].HeaderCell.Value = $"x{num + 1}";
                DGVbig.Rows[0].Cells[0].Value = $"f{num}(x{num})/f{num + 1}(x{num + 1})";
                DGVsmall.Columns[0].HeaderCell.Value = $"s{num+1}";
                DGVsmall.Rows[0].Cells[0].Value = $"f{num}(s{num + 1})";
                DGVsmall.Rows[1].Cells[0].Value = $"x{num}";
            }

            for (int i = 1; i < 7; i++)
            {
                DGVbig.Rows[i].HeaderCell.Value = (i - 1).ToString();
                DGVbig.Columns[i].HeaderCell.Value = (i - 1).ToString();
            }

            DGVbig.Rows[0].Cells[0].Style.BackColor = Color.LightGray;
            DGVbig.Rows[1].Cells[0].Value = 0;
            DGVbig.Rows[1].Cells[0].Style.BackColor = Color.LightGray;
            DGVbig.Rows[0].Cells[1].Value = 0;
            DGVbig.Rows[0].Cells[1].Style.BackColor = Color.LightGray;

            for (int i = 2; i < 7; i++) //вот оно, заполнение
            {
                DGVbig.Rows[i].Cells[0].Value = listVertical[i - 2];
                DGVbig.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                DGVbig.Rows[0].Cells[i].Value = listHorizontal[i - 2];
                DGVbig.Rows[0].Cells[i].Style.BackColor = Color.LightGray;
            }

            for (int i = 1; i < 7; i++)
                for (int j = 1; j < 7; j++)
                {
                    if (i + j == 7)
                        DGVbig.Rows[i].Cells[j].Value = Convert.ToDouble(DGVbig.Rows[i].Cells[0].Value) +
                                                            Convert.ToDouble(DGVbig.Rows[0].Cells[j].Value);
                    else DGVbig.Rows[i].Cells[j].Value = 0;
                }
            DGVbig.CurrentCell.Selected = false;

            List<Tuple<double, int>> list = new List<Tuple<double, int>>(2);

            for (int i = 1; i < 7; i++) //rows
                for (int j = 1; j < 7; j++) //columns
                {
                    if (i + j == 7)
                    {
                        list.Add(new Tuple<double, int>(Convert.ToDouble(DGVbig.Rows[i].Cells[j].Value), i - 1));
                        DGVbig.Rows[i].Cells[j].Style.BackColor = Color.SandyBrown;
                        break;
                    }
                }
            maxesList.Add(0);
            maxesList.Add(0);
            maxesList.Add(0);
            maxesList.Add(0);
            maxesList.Add(Aboba(list).Item1);
            indexesList.Add(1);
            indexesList.Add(2);
            indexesList.Add(3);
            indexesList.Add(4);
            indexesList.Add(Aboba(list).Item2);
            #endregion

            #region SmallTable
            for (int i = 1; i < 6; i++)
            {
                DGVsmall.Columns[i].HeaderCell.Value = i.ToString();
                DGVsmall.Rows[0].Cells[i].Value = maxesList[i - 1];
                DGVsmall.Rows[1].Cells[i].Value = indexesList[i - 1];
            }

            DGVsmall.CurrentCell.Selected = false;
            #endregion
        }
        private Tuple<double, int> Aboba(List<Tuple<double, int>> list)
        {
            double max = 0;
            int ind = 0;
            foreach (var el in list)
                if (el.Item1 > max)
                {
                    max = el.Item1;
                    ind = el.Item2;
                }
            return new Tuple<double, int>(max, ind);
        }
        private void Finals(int num)
        {
            Application.DoEvents();
            Thread.Sleep(200);
            int remainingMoney = 5;

            DGVlast.RowCount = 5;
            DGVlast.ColumnCount = 3;

            DGVlast.Columns[0].HeaderCell.Value = "Предприятие";
            DGVlast.Columns[1].HeaderCell.Value = "Деньги";
            DGVlast.Columns[2].HeaderCell.Value = "Прибыль";
            for (int i = 0; i < 4; i++)
                DGVlast.Rows[i].Cells[0].Value = (i + 1).ToString();
            DGVlast.Rows[4].Cells[0].Value = "Итого";
            for (int i = 0; i < 3; i++)
                DGVlast.Rows[4].Cells[i].Style.BackColor = Color.LightGray;

            #region 4to3
            Application.DoEvents();
            Thread.Sleep(200);

            int lastIndex = Convert.ToInt32(DGVStep3_2.Rows[1].Cells[5].Value);
            DGVStep3_2.Rows[0].Cells[5].Style.BackColor = Color.Violet;
            DGVStep3_2.Rows[1].Cells[5].Style.BackColor = Color.Violet;

            if (num == 1)
            {
                DGVlast.Rows[3].Cells[1].Value = lastIndex.ToString();
                DGVlast.Rows[3].Cells[2].Value = DGV.Rows[3].Cells[lastIndex - 1].Value;
            }
            else
            {
                DGVlast.Rows[0].Cells[1].Value = lastIndex.ToString();
                DGVlast.Rows[0].Cells[2].Value = DGV.Rows[0].Cells[lastIndex - 1].Value;
            }

            textBoxMinus4to3.Text = lastIndex.ToString();
            remainingMoney -= lastIndex;
            textBoxOst4to3.Text = remainingMoney.ToString();
            #endregion

            #region 3to2
            Application.DoEvents();
            Thread.Sleep(200);

            double step2max = 0;
            int step2ind = 0, i1;
            for (i1 = 1; i1 <= remainingMoney; i1++)
                if (Convert.ToDouble(DGVStep2_2.Rows[0].Cells[i1].Value) > step2max)
                {
                    step2max = Convert.ToDouble(DGVStep2_2.Rows[0].Cells[i1].Value);
                    step2ind = Convert.ToInt32(DGVStep2_2.Rows[1].Cells[i1].Value);
                }
            DGVStep2_2.Rows[0].Cells[i1 - 1].Style.BackColor = Color.Violet;
            DGVStep2_2.Rows[1].Cells[i1 - 1].Style.BackColor = Color.Violet;
            if (step2ind != 0)
            {
                if (num == 1)
                {
                    DGVlast.Rows[2].Cells[1].Value = step2ind.ToString();
                    DGVlast.Rows[2].Cells[2].Value = DGV.Rows[2].Cells[step2ind - 1].Value;
                }
                else
                {
                    DGVlast.Rows[1].Cells[1].Value = step2ind.ToString();
                    DGVlast.Rows[1].Cells[2].Value = DGV.Rows[1].Cells[step2ind - 1].Value;
                }
            }
            else
            {
                if (num == 1)
                {
                    DGVlast.Rows[2].Cells[1].Value = 0;
                    DGVlast.Rows[2].Cells[2].Value = 0;
                }
                else
                {
                    DGVlast.Rows[1].Cells[1].Value = 0;
                    DGVlast.Rows[1].Cells[2].Value = 0;
                }
            }

            textBoxMinus3to2.Text = step2ind.ToString();
            remainingMoney -= step2ind;
            textBoxOst3to2.Text = remainingMoney.ToString();
            #endregion

            #region 2to1
            Application.DoEvents();
            Thread.Sleep(200);

            double step3max = 0;
            int step3ind = 0, i2;
            for (i2 = 1; i2 <= remainingMoney; i2++)
                if (Convert.ToDouble(DGVStep1_2.Rows[0].Cells[i2].Value) > step3max)
                {
                    step3max = Convert.ToDouble(DGVStep1_2.Rows[0].Cells[i2].Value);
                    step3ind = Convert.ToInt32(DGVStep1_2.Rows[1].Cells[i2].Value);
                }
            DGVStep1_2.Rows[0].Cells[i2 - 1].Style.BackColor = Color.Violet;
            DGVStep1_2.Rows[1].Cells[i2 - 1].Style.BackColor = Color.Violet;

            if (step3ind != 0)
            {
                if (num == 1)
                {
                    DGVlast.Rows[1].Cells[1].Value = step3ind.ToString();
                    DGVlast.Rows[1].Cells[2].Value = DGV.Rows[1].Cells[step3ind - 1].Value;
                }
                else
                {
                    DGVlast.Rows[2].Cells[1].Value = step3ind.ToString();
                    DGVlast.Rows[2].Cells[2].Value = DGV.Rows[2].Cells[step3ind - 1].Value;
                }
            }
            else
            {
                if (num == 1)
                {
                    DGVlast.Rows[1].Cells[1].Value = 0;
                    DGVlast.Rows[1].Cells[2].Value = 0;
                }
                else
                {
                    DGVlast.Rows[2].Cells[1].Value = 0;
                    DGVlast.Rows[2].Cells[2].Value = 0;
                }
            }

            textBoxMinus2to1.Text = step3ind.ToString();
            remainingMoney -= step3ind;
            textBoxOst2to1.Text = remainingMoney.ToString();
            #endregion

            #region last
            Application.DoEvents();
            Thread.Sleep(200);

            if (remainingMoney != 0)
            {
                if (num == 1)
                {
                    DGVlast.Rows[0].Cells[1].Value = remainingMoney.ToString();
                    DGVlast.Rows[0].Cells[2].Value = DGV.Rows[0].Cells[remainingMoney - 1].Value;
                }
                else
                {
                    DGVlast.Rows[3].Cells[1].Value = remainingMoney.ToString();
                    DGVlast.Rows[3].Cells[2].Value = DGV.Rows[3].Cells[remainingMoney - 1].Value;
                }
            }
            else
            {
                if (num == 1)
                {
                    DGVlast.Rows[0].Cells[1].Value = 0;
                    DGVlast.Rows[0].Cells[2].Value = 0;
                }
                else
                {
                    DGVlast.Rows[3].Cells[1].Value = 0;
                    DGVlast.Rows[3].Cells[2].Value = 0;
                }
            }

            double result = 0;
            for (int i = 0; i <= 3; i++)
                result += Convert.ToDouble(DGVlast.Rows[i].Cells[2].Value);
            DGVlast.Rows[4].Cells[1].Value = "5";
            DGVlast.Rows[4].Cells[2].Value = result;
            #endregion
        }
    }
}