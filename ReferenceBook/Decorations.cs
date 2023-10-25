using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System;
using System.Windows.Forms;

namespace ReferenceBook
{
    public partial class Decorations : Form
    {
        private AccessRights globalAccessRights;
        public Decorations(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTable();
        }

        private void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            DecorationSql decorationSql = new DecorationSql();
            foreach (Decoration decoration in decorationSql.SelectAll())
            {
                dataGridView1.Rows.Add(decoration.Id, decoration.Name, decoration.Description);
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
                        DecorationSql decorationSql = new DecorationSql();
                        decorationSql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
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
                Decoration decoration = new Decoration();
                DecorationSql decorationSql = new DecorationSql();
                if (dataGridView1.CurrentRow.Cells[1].Value == null)
                {
                    MessageBox.Show("Строка названия не должна быть пустой");
                    return;
                }
                else
                {
                    if (dataGridView1.CurrentRow.Cells[0].Value == null)
                    {
                        decoration.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        decoration.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        decorationSql.WriteToDb(decoration);
                    }
                    else
                    {
                        decoration.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        decoration.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        decoration.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        decorationSql.UpdateInDb(decoration);
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
