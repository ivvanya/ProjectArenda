using ProjectArenda.DataBase;
using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Windows.Forms;

namespace Room
{
    public partial class TableRoom : Form
    {
        private AccessRights globalAccessRights;
        public TableRoom(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateTable();
        }

        private void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            RoomSql roomSql = new RoomSql();
            foreach (ProjectArenda.Models.Room room in roomSql.RoomBuildDecor())
            {
                dataGridView1.Rows.Add(room.Id, room.Number, room.Floor, room.Square,
                    room.Phone, room.decoration.Id, room.decoration.Name, room.building.Id, room.building.Address);
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
                    RoomSql roomSql = new RoomSql();
                    roomSql.DeleteFromDb(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
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
                ProjectArenda.Models.Room room = new ProjectArenda.Models.Room();
                room.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                room.Phone = Convert.ToBoolean(dataGridView1.CurrentRow.Cells[4].Value);
                room.decoration.Name = Convert.ToString(dataGridView1.CurrentRow.Cells[6].Value);
                room.building.Address = Convert.ToString(dataGridView1.CurrentRow.Cells[8].Value);
                try
                {
                    room.Number = Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value);
                    room.Floor = Convert.ToInt32(dataGridView1.CurrentRow.Cells[2].Value);
                    room.Square = Convert.ToInt32(dataGridView1.CurrentRow.Cells[3].Value);
                    room.building.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[7].Value);
                    room.decoration.Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[5].Value);
                }
                catch
                {
                    MessageBox.Show("Неправильный формат ввода - символы в числовых полях)");
                    return;
                }

                RoomTools roomTools = new RoomTools();
                string status = roomTools.checkData(room);

                if (status != "good")
                {
                    MessageBox.Show(status);
                    return;
                }
                else
                {
                    roomTools.UpdateRoom(room);
                    MessageBox.Show("Данные пользователя обновлены");
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                dataGridView1.Rows[i].Selected = false;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {

                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString().Contains(textBox1.Text) && textBox1.Text != "")
                        {
                            dataGridView1.Rows[i].Selected = true;
                            break;
                        }
            }
        }
    }
}
