using ProjectArenda.DataBase;
using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Windows.Forms;

namespace ReferenceBook
{
    public partial class Buildings : Form
    {
        private AccessRights globalAccessRights;
        public Buildings(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTable();
        }

        private void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            BuildingSql buildingSql = new BuildingSql();
            foreach (Building building in buildingSql.BuildingsWithDistricts())
            {
                dataGridView1.Rows.Add(building.Id, building.district.Id, building.district.Name, building.Address,
                    building.Floors, building.Rooms, building.ComNumb);
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
                        BuildingSql buildingSql = new BuildingSql();
                        buildingSql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
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
                Building building = new Building();
                building.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                building.district.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value);
                building.district.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);

                building.Address = Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
                try
                {
                    building.Floors = Convert.ToInt32(dataGridView1.CurrentRow.Cells[4].Value);
                }
                catch
                {
                    MessageBox.Show("Неправильный формат ввода количества этажей");
                    return;
                }
                building.Rooms = Convert.ToInt32(dataGridView1.CurrentRow.Cells[5].Value);
                building.ComNumb = Convert.ToString(dataGridView1.CurrentRow.Cells[6].Value);

                BuildingTools buildingTools = new BuildingTools();
                string status = buildingTools.checkData(building);

                if (status != "good")
                {
                    MessageBox.Show(status);
                    return;
                }
                else
                {
                    if (dataGridView1.CurrentRow.Cells[0].Value == null)
                    {
                        buildingTools.AddBuilding(building);
                        MessageBox.Show("Данные здания добавлены");
                    }
                    else
                    {
                        buildingTools.UpdateBuilding(building);
                        MessageBox.Show("Данные здания обновлены");
                    }
                    UpdateTable();
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
