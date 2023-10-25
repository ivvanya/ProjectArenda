using ProjectArenda.DataBase;
using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Room
{
    public partial class AddRoom : Form
    {
        private List<District> districts = new List<District>();
        private List<Building> buildings = new List<Building>();
        private List<Decoration> decorations = new List<Decoration>();
        private AccessRights globalAccessRights;
        public AddRoom(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            UpdateDistrictBox();
            UpdateDecorationBox();
        }

        private void UpdateDistrictBox()
        {
            comboBox1.Items.Clear();
            DistrictSql districtSql = new DistrictSql();
            foreach (District district in districtSql.SelectAllDistricts())
            {
                districts.Add(district);
                comboBox1.Items.Add(district.Name);
            }
        }
        private void UpdateBuildingBox()
        {
            int districtid = 0;
            foreach (District district in districts)
            {
                if (comboBox1.Text == district.Name)
                {
                    districtid = district.Id;
                    break;
                }
            }
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            RoomSql roomSql = new RoomSql();
            foreach (Building building in roomSql.SelectBuildingsByDistrict(districtid))
            {
                buildings.Add(building);
                comboBox2.Items.Add(building.Address);
            }
        }
        private void UpdateFloorBox()
        {
            int floors = 0;
            foreach (Building building in buildings)
            {
                if (comboBox2.Text == building.Address)
                {
                    floors = building.Floors;
                    break;
                }
            }
            comboBox3.Items.Clear();
            comboBox3.Text = "";
            RoomSql roomSql = new RoomSql();
            for (int i = 1; i <= floors; i++)
            {
                comboBox3.Items.Add(i);
            }
        }
        private void UpdateDecorationBox()
        {
            comboBox4.Items.Clear();
            DecorationSql decorationSql = new DecorationSql();
            foreach (Decoration decoration in decorationSql.SelectAll())
            {
                decorations.Add(decoration);
                comboBox4.Items.Add(decoration.Name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBuildingBox();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFloorBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 || comboBox3.SelectedIndex == -1 ||
                comboBox4.SelectedIndex == -1 || textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            else
            {
                int buildingid = 0;
                foreach (Building building in buildings)
                {
                    if (comboBox2.Text == building.Address)
                    {
                        buildingid = building.Id;
                        break;
                    }
                }
                int decorationid = 0;
                foreach (Decoration decoration in decorations)
                {
                    if (comboBox4.Text == decoration.Name)
                    {
                        decorationid = decoration.Id;
                        break;
                    }
                }

                ProjectArenda.Models.Room room = new ProjectArenda.Models.Room();
                room.building.Id = buildingid;
                try
                {
                    room.Number = Convert.ToInt32(textBox1.Text);
                    room.Square = Convert.ToInt32(textBox2.Text);
                }
                catch
                {
                    MessageBox.Show("Неправильный формат ввода номера или площади");
                    return;
                }
                room.Floor = Convert.ToInt32(textBox1.Text);
                room.decoration.Id = decorationid;
                room.Phone = checkBox1.Checked;

                RoomTools roomTools = new RoomTools();
                roomTools.AddRoom(room);
                MessageBox.Show("Данные помещения добавлены");
                this.Close();
            }
        }
    }
}
