using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System;
using System.Windows.Forms;

namespace ReferenceBook
{
    public partial class Periodicitys : Form
    {
        private AccessRights globalAccessRights;
        public Periodicitys(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTable();
        }

        private void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            PeriodicitySql periodicitySql = new PeriodicitySql();
            foreach (Periodicity periodicity in periodicitySql.SelectAll())
            {
                dataGridView1.Rows.Add(periodicity.Id, periodicity.Name, periodicity.Description);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (globalAccessRights.Delete)
            {
                if (dataGridView1.CurrentRow.Cells[0].Value == null)
                    MessageBox.Show("Строка еще не добавлена в базу данных");
                else
                {
                    DialogResult result = MessageBox.Show("Вы точно хотите удалить запись?",
                        "Предупреждение", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        PeriodicitySql periodicitySql = new PeriodicitySql();
                        periodicitySql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
                        UpdateTable();
                    }
                    else return;
                }
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
                Periodicity periodicity = new Periodicity();
                PeriodicitySql periodicitySql = new PeriodicitySql();
                if (dataGridView1.CurrentRow.Cells[1].Value == null)
                {
                    MessageBox.Show("Строка названия не должна быть пустой");
                    return;
                }
                else
                {
                    if (dataGridView1.CurrentRow.Cells[0].Value == null)
                    {
                        periodicity.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        periodicity.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        periodicitySql.WriteToDb(periodicity);
                    }
                    else
                    {
                        periodicity.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        periodicity.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        periodicity.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        periodicitySql.UpdateInDb(periodicity);
                    }
                }
                UpdateTable();
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
