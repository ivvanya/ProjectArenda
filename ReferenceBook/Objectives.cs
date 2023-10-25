using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReferenceBook
{
    public partial class Objectives : Form
    {
        private AccessRights globalAccessRights;
        public Objectives(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTable();
        }

        private void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            ObjectiveSql objectiveSql = new ObjectiveSql();
            foreach (Objective objective in objectiveSql.SelectAll())
            {
                dataGridView1.Rows.Add(objective.Id, objective.Name, objective.Description);
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
                        ObjectiveSql objectiveSql = new ObjectiveSql();
                        objectiveSql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
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
                Objective objective = new Objective();
                ObjectiveSql objectiveSql = new ObjectiveSql();
                if (dataGridView1.CurrentRow.Cells[1].Value == null)
                {
                    MessageBox.Show("Строка названия не должна быть пустой");
                    return;
                }
                else
                {
                    if (dataGridView1.CurrentRow.Cells[0].Value == null)
                    {
                        objective.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        objective.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        objectiveSql.WriteToDb(objective);
                    }
                    else
                    {
                        objective.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        objective.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        objective.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        objectiveSql.UpdateInDb(objective);
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
