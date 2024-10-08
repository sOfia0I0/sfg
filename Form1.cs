using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace tpergtr
{
    public partial class Form1 : Form
    {
    \\ Вынесение коннекта к базе данных в отдельный класс для наследования 
        private string connectionString = "server=localhost;database=workinfreedom;uid=root;pwd=cDta5hdh56yupo";
        private MySqlConnection connection;
        private MySqlCommand command;
        private Form2 form2;
        public Form1()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            string[] roles = { "Заказчик", "Фрилансер", "Администратор" };
            guna2ComboBox1.Items.AddRange(roles);
            guna2ComboBox2.Items.AddRange(roles);
            form2 = new Form2();
        }
        private bool CheckExistingUser(string login, string parol) // проверка существования пользователя в базе данных по его логину и паролю
        {
            try
            {
                \\ Возможно не обязательно чтобы пароль провеялся на оригинальность он может и повторятся 
                string query = "SELECT COUNT(*) FROM users WHERE login = @login OR parol = @parol";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@parol", parol);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке существующего пользователя: " + ex.Message);
                return false;
            }
        }
        private void guna2Button2_Click(object sender, EventArgs e) // регистрация
        {
            \\ Для упрощения ориентации в программе TextBox необходимо уникально подписывать
            string familiya = guna2TextBox4.Text;
            string imya = guna2TextBox3.Text;
            string telefon = guna2TextBox5.Text;
            string login = guna2TextBox7.Text;
            string parol = guna2TextBox6.Text;
            string role = guna2ComboBox2.SelectedItem.ToString();
            if (string.IsNullOrEmpty(familiya) || string.IsNullOrEmpty(imya) || string.IsNullOrEmpty(telefon) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(parol))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }
            try
            {
                connection.Open();
                if (CheckExistingUser(login, parol))
                {
                    MessageBox.Show("Логин и пароль уже существуют. Попробуйте другие.");
                    return;
                }
                string query = "INSERT INTO users (familiya, imya, telefon, login, parol, role) VALUES (@familiya, @imya, @telefon, @login, @parol, @role)";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@familiya", familiya);
                command.Parameters.AddWithValue("@imya", imya);
                command.Parameters.AddWithValue("@telefon", telefon);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@parol", parol);
                command.Parameters.AddWithValue("@role", role);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Пользователь успешно добавлен в базу данных.");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении пользователя.");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Duplicate entry"))
                {
                    MessageBox.Show("Логин уже занят. Попробуйте другой логин.");
                }
                else
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            }
            finally
            {
                connection.Close();
                guna2TextBox4.Text = string.Empty;
                guna2TextBox3.Text = string.Empty;
                guna2TextBox5.Text = string.Empty;
                guna2TextBox7.Text = string.Empty;
                guna2TextBox6.Text = string.Empty;
                guna2ComboBox2.SelectedIndex = -1;
            }
        }
        private int GetTabIndexForRole(string role)
        {
            switch (role)
            {
                case "Заказчик":
                    return 0;
                case "Фрилансер":
                    return 1;
                case "Администратор":
                    return 2;
                default:
                    return -1;
            }
        }
        private void guna2Button1_Click(object sender, EventArgs e) // вход
        {
            string login = guna2TextBox1.Text;
            string parol = guna2TextBox2.Text;
            string role = guna2ComboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(parol) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            try
            {
                connection.Open();
                string query = "SELECT id FROM users WHERE login = @login AND parol = @parol AND role = @role";
                command = new MySqlCommand(query, connection);
                \\ Можно использовать цикл или метод для ввода данных в БД
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@parol", parol);
                command.Parameters.AddWithValue("@role", role);
                int userId = Convert.ToInt32(command.ExecuteScalar());
                if (userId > 0)
                {
                    form2.currentRole = role;
                    form2.currentUserId = userId.ToString();
                    form2.guan2TabControl1.SelectedIndex = GetTabIndexForRole(role);
                    form2.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Неверный логин, пароль или роль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                connection.Close();
                guna2TextBox1.Text = string.Empty;
                guna2TextBox2.Text = string.Empty;
                guna2ComboBox1.SelectedIndex = -1;
            }
        }
        private void guna2CircleButton1_Click(object sender, EventArgs e) // закрыть форму 1
        {
            Close();
        }
    }
}
