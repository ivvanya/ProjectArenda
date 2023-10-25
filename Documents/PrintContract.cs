using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Documents
{
    public partial class PrintContract : Form
    {
        private string roomListText;
        private AccessRights globalAccessRights;
        public PrintContract(AccessRights globalAccessRights)
        {
            this.globalAccessRights = globalAccessRights;
            InitializeComponent();
            DocumentsSql documentsSql = new DocumentsSql();
            foreach (Contract contract in documentsSql.GetContracts())
            {
                comboBox1.Items.Add(contract.Id);
            }
            comboBox1.SelectedIndex = 0;
            UpdateTable();

        }
        private void UpdateTable()
        {
            DocumentsSql documentsSql = new DocumentsSql();
            Contract contract = documentsSql.GetContract(Convert.ToInt32(comboBox1.Text));
            label14.Text = contract.StartTime.ToString("dd.MM.yyyy");
            label15.Text = contract.employee.Name;
            label16.Text = contract.employee.role.Name;

            if (contract.tenant.FaceStatus == true)
            {
                label17.Text = contract.individual.Name;
                label18.Text = "--------";
                label19.Text = "физического лица";
                label24.Text = "--------";
                label25.Text = "--------";
                label26.Text = contract.individual.Number;
            }
            else
            {
                label17.Text = contract.entity.Name;
                label18.Text = contract.entity.HeadName;
                label19.Text = "директора компании";
                label24.Text = contract.entity.Address;
                label25.Text = contract.entity.Bank + ", " + contract.entity.BankAccount;
                label26.Text = contract.entity.Number;
            }

            List<RoomList> roomLists = documentsSql.GetRoomLists(Convert.ToInt32(comboBox1.Text));
            label20.Text = roomLists.Count.ToString();

            int square = 0;
            int price = 0;
            int count = 0;
            roomListText = "";
            foreach (RoomList roomList in roomLists)
            {
                string spaces = "            ";
                square += roomList.room.Square;
                price += roomList.Price;
                roomListText += "1.2." + (count + 1).ToString() + ".1. Арендатор обязуется использовать помещение для "
                    + roomList.objective.Name + ".\r" + spaces + "1.2." + (count + 1).ToString() + ".2. Срок аренды помещения устанавливается на "
                    + roomList.RentalPeriod + " месяцев с " + contract.StartTime.ToString("dd.MM.yyyy") + " г. по "
                    + contract.StartTime.AddMonths(roomList.RentalPeriod).ToString("dd.MM.yyyy") + " г.\r" + spaces;
                count++;
            }
            label21.Text = square.ToString();
            label23.Text = price.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var helper = new WordHelper("../../../" + "Documents/bin/Debug/" + "dogovor.doc"); //debug
            //var helper = new WordHelper("Templates/" + "dogovor.doc"); //release
            var items = new Dictionary<string, string>
            {
                { "<Дата заключения договора>", label14.Text },
                { "<ФИО сотрудника>", label15.Text },
                { "<Должность сотрудника>", label16.Text },
                { "<Компания/физ.лицо>", label17.Text },
                { "<ФИО директора/физ.лица>", label18.Text },
                { "<Директор/физ.лицо>", label19.Text },
                { "<Количество помещений>", label20.Text } ,
                { "<Общая площадь>", label21.Text } ,
                { "<Перечень помещений>", roomListText } ,
                { "<Арендная плата>", label23.Text } ,
                { "<Адрес компании>", label24.Text } ,
                { "<Банковский счет>", label25.Text } ,
                { "<Номер телефона>", label26.Text }
            };
            if (items["<Перечень помещений>"].Length > 255)
            {

                string fullString = items["<Перечень помещений>"];
                int cnt = 0;
                int tempNumber = 0;
                string tempKey0 = "";
                string tempKey = "<Перечень помещений" + tempNumber.ToString() + ">";
                items["<Перечень помещений>"] = fullString.Substring(0, 230) + tempKey;

                cnt += 230;
                while (cnt < fullString.Length - 1)
                {
                    tempNumber += 1;
                    tempKey0 = "<Перечень помещений" + (tempNumber - 1).ToString() + ">";
                    tempKey = "<Перечень помещений" + tempNumber.ToString() + ">";

                    if (fullString.Length - cnt < 230)
                    {
                        items.Add(tempKey0, fullString.Substring(cnt, fullString.Length - cnt));
                    }
                    else
                    {
                        items.Add(tempKey0, fullString.Substring(cnt, 230) + tempKey);
                    }
                    cnt += 230;
                }
            }
            bool createDocument = helper.Process(items);
            if (createDocument)
            {
                MessageBox.Show("Документ помещен в корневую папку");
            }
            else
            {
                MessageBox.Show("Документ не создан");
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTable();
        }
    }
}
