using ProjectArenda.Models;
using ProjectArenda.Tools;
using System;
using System.Windows.Forms;

namespace ProjectArenda.Forms
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            EmployeeTools employeeTools = new EmployeeTools();
            Employee employee = employeeTools.LoginEmployee(login, password);

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Поля заполнены некорректно");
            }
            else if (employee.Id != 0)
            {
                MainForm mainForm = new MainForm(employee);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Сотрудник не найден. Проверьте корректность логина и пароля");
            }
        }
    }
}
