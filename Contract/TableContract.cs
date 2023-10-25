using ProjectArenda.DataBase;
using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Windows.Forms;

namespace Contract
{
    public partial class TableContract : Form
    {
        private AccessRights globalAccessRights;
        public TableContract(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTableIndividual();
            comboBox1.Items.Add("Физические лица");
            comboBox1.Items.Add("Юридические лица");
            comboBox1.SelectedIndex = 0;
        }

        private void UpdateTableIndividual()
        {
            dataGridView1.Rows.Clear();
            ContractSql contractSql = new ContractSql();
            foreach (ProjectArenda.Models.Contract contract in contractSql.AllContractsIndividual())
            {
                dataGridView1.Rows.Add(contract.Id, contract.individual.Id, contract.individual.Name, contract.employee.Id,
                    contract.employee.Name, contract.periodicity.Id, contract.periodicity.Name, contract.Penalty,
                    contract.Additionaly, contract.StartTime, contract.EndTime);
            }
        }
        private void UpdateTableEntity()
        {
            dataGridView1.Rows.Clear();
            ContractSql contractSql = new ContractSql();
            foreach (ProjectArenda.Models.Contract contract in contractSql.AllContractsEntity())
            {
                dataGridView1.Rows.Add(contract.Id, contract.entity.Id, contract.entity.Name, contract.employee.Id,
                    contract.employee.Name, contract.periodicity.Id, contract.periodicity.Name, contract.Penalty,
                    contract.Additionaly, contract.StartTime, contract.EndTime);
            }
        }

        private void UpdateTable()
        {
            if (comboBox1.SelectedIndex == 0)
                UpdateTableIndividual();
            else
                UpdateTableEntity();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (globalAccessRights.Edit)
            {
                AddRooms newForm = new AddRooms(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
                newForm.Show();
            }
            else
            {
                MessageBox.Show("У вас недостаточно прав");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (globalAccessRights.Delete)
            {
                DialogResult result = MessageBox.Show("Вы точно хотите удалить запись?",
                "Предупреждение", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ContractSql contractSql = new ContractSql();
                    contractSql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
                    UpdateTable();
                }
                else return;
            }
            else
            {
                MessageBox.Show("У вас недостаточно прав");
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (globalAccessRights.Edit)
            {
                ProjectArenda.Models.Contract contract = new ProjectArenda.Models.Contract();
                contract.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                contract.periodicity.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[6].Value);
                contract.Additionaly = Convert.ToString(dataGridView1.CurrentRow.Cells[8].Value);
                try
                {
                    contract.Penalty = Convert.ToInt32(dataGridView1.CurrentRow.Cells[7].Value);
                }
                catch
                {
                    MessageBox.Show("Введите корректное значение штрафа");
                    return;
                }

                if (contract.periodicity.Name == "" || contract.Penalty < 1)
                {
                    MessageBox.Show("Поля заполнены некорректно");
                    return;
                }
                else
                {
                    ContractTools contractTools = new ContractTools();
                    contractTools.updateContract(contract);
                }
            }
            else
            {
                MessageBox.Show("У вас недостаточно прав");
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
