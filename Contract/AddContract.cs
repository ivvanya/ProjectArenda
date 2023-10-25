using ProjectArenda.DataBase;
using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Contract
{
    public partial class AddContract : Form
    {
        private AccessRights globalAccessRights;

        private List<Periodicity> periodicityList = new List<Periodicity>();
        public AddContract(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();

            comboBox1.Items.Add("Физические лица");
            comboBox1.Items.Add("Юридические лица");
            comboBox1.SelectedIndex = 0;

            PeriodicitySql periodicitySql = new PeriodicitySql();
            foreach (Periodicity periodicity in periodicitySql.SelectAll())
            {
                comboBox2.Items.Add(periodicity.Name);
                periodicityList.Add(periodicity);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                label2.Text = "Паспортные данные\r\nФормат: 0000 000000";
            else
                label2.Text = "Номер ИНН";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 || textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            else
            {
                ProjectArenda.Models.Contract contract = new ProjectArenda.Models.Contract();
                contract.employee.Id = globalAccessRights.EmployeeId;
                contract.Additionaly = textBox3.Text;
                contract.StartTime = dateTimePicker1.Value;
                contract.EndTime = dateTimePicker2.Value;

                foreach (Periodicity periodicity in periodicityList)
                    if (comboBox2.Text == periodicity.Name)
                    {
                        contract.periodicity.Id = periodicity.Id;
                        break;
                    }

                if (comboBox1.SelectedIndex == 0)
                {
                    try
                    {
                        contract.individual.PassSeries = textBox1.Text.Split(' ')[0];
                        contract.individual.PassNumber = textBox1.Text.Split(' ')[1];
                    }
                    catch
                    {
                        MessageBox.Show("Введите паспортные данные в формате '0000 000000'");
                        return;
                    }
                }
                else
                    contract.entity.INN = textBox1.Text;

                try
                {
                    contract.Penalty = Convert.ToInt32(textBox2.Text);
                }
                catch
                {
                    MessageBox.Show("Неправильный формат ввода суммы штрафа");
                    return;
                }

                ContractTools contractTools = new ContractTools();
                if (contractTools.addContract(contract, comboBox1.SelectedIndex))
                {
                    AddRooms newForm = new AddRooms();
                    this.Close();
                    newForm.Show();
                }
                else return;
            }
        }
    }
}
