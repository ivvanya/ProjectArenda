using ProjectArenda.DataBase;
using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Contract
{
    public partial class AddRooms : Form
    {
        private bool InsertOrUpdate;
        private int contractId = 0;
        public AddRooms(int contractId)
        {
            this.contractId = contractId;
            InsertOrUpdate = false;
            TopMost = true;
            InitializeComponent();
            UpdateRoomGridToUpdate();
        }

        public AddRooms()
        {
            InsertOrUpdate = true;
            TopMost = true;
            InitializeComponent();
            UpdateRoomGridToAdd();
        }

        private void UpdateObjectiveColumn()
        {
            ObjectiveSql objectiveSql = new ObjectiveSql();
            foreach (Objective objective in objectiveSql.SelectAll())
            {
                Column3.Items.Add(objective.Name);
            }
        }

        private void UpdateRoomGridToAdd()
        {
            UpdateObjectiveColumn();

            RoomListSql roomListSql = new RoomListSql();
            List<Room> rooms = roomListSql.SelectRoomsToAdd();
            foreach (Room room in rooms)
            {
                dataGridView1.Rows.Add(room.Id, room.building.district.Name + ", " + room.building.Address + ", " + room.Number);
            }
        }

        private void UpdateRoomGridToUpdate()
        {
            UpdateRoomGridToAdd();

            RoomListSql roomListSql = new RoomListSql();
            List<RoomList> roomlists = roomListSql.SelectRoomsToUpdate(contractId);
            for (int i = 0; i < dataGridView1.RowCount; i++)
                foreach (RoomList roomlist in roomlists)
                    if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == roomlist.room.Id)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = true;
                        dataGridView1.Rows[i].Cells[3].Value = roomlist.objective.Name;
                        dataGridView1.Rows[i].Cells[4].Value = roomlist.Price;
                        dataGridView1.Rows[i].Cells[5].Value = roomlist.RentalPeriod;
                    }
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

        private void button1_Click(object sender, EventArgs e)
        {
            List<RoomList> roomLists = new List<RoomList>();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[2].Value) == true)
                {
                    if (dataGridView1.Rows[i].Cells[3].Value == null || dataGridView1.Rows[i].Cells[4].Value == null ||
                        dataGridView1.Rows[i].Cells[5].Value == null)
                    {
                        MessageBox.Show("Заполните все поля в строке " + (i + 1).ToString());
                        dataGridView1.Rows[i].Selected = true;
                        return;
                    }
                    else
                    {
                        RoomList roomList = new RoomList();
                        roomList.contract.Id = contractId;
                        roomList.room.Id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                        roomList.objective.Name = Convert.ToString(dataGridView1.Rows[i].Cells[3].Value);
                        try
                        {
                            roomList.Price = Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value);
                            roomList.RentalPeriod = Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
                        }
                        catch
                        {
                            MessageBox.Show("Введите корректные стоимость или срок аренды в строке " + (i + 1).ToString());
                            dataGridView1.Rows[i].Selected = true;
                            return;
                        }

                        if (roomList.Price < 1 || roomList.RentalPeriod < 1)
                        {
                            MessageBox.Show("Введите корректные стоимость или срок аренды в строке " + (i + 1).ToString());
                            dataGridView1.Rows[i].Selected = true;
                            return;
                        }
                        else
                        {
                            roomLists.Add(roomList);
                        }
                    }
                }
                else continue;
            }
            RoomListTools roomListTools = new RoomListTools();
            if (InsertOrUpdate == true)
            {
                roomListTools.CreateRoomList(roomLists);
                MessageBox.Show("Данные добавлены");
            }
            else
            {
                roomListTools.UpdateRoomList(roomLists);
                MessageBox.Show("Данные обновлены");
            }

            this.Close();
        }
    }
}
