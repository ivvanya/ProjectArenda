using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System;
using System.Windows.Forms;

namespace ReferenceBook
{
    public partial class Districts : Form
    {
        private AccessRights globalAccessRights;
        public Districts(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTable();
        }
        private void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            DistrictSql districtSql = new DistrictSql();
            foreach (District district in districtSql.SelectAllDistricts())
            {
                dataGridView1.Rows.Add(district.Id, district.Name, district.Description);
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
                        DistrictSql districtSql = new DistrictSql();
                        districtSql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
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
                District district = new District();
                DistrictSql districtSql = new DistrictSql();
                if (dataGridView1.CurrentRow.Cells[1].Value == null)
                {
                    MessageBox.Show("Строка названия не должна быть пустой");
                    return;
                }
                else
                {
                    if (dataGridView1.CurrentRow.Cells[0].Value == null)
                    {
                        district.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        district.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        districtSql.WriteToDb(district);
                    }
                    else
                    {
                        district.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        district.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                        district.Description = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                        districtSql.UpdateInDb(district);
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
