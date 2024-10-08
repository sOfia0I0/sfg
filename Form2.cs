using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace tpergtr
{
    public partial class Form2 : Form
    {
        private string connectionString = "server=localhost;database=workinfreedom;uid=root;pwd=cDta5hdh56yupo";
        private MySqlConnection connection;
        public string currentRole;
        public string currentUserId;
        public event EventHandler ClearDataGridView;
        private MySqlCommand command;
        private MySqlDataAdapter adapter;
        private void guan2TabControl1_Selecting(object sender, TabControlCancelEventArgs e) // доступ к вкладкам
        {
            if (e.TabPage.Name != currentRole)
            {
                e.Cancel = true;
                MessageBox.Show("У вас нет доступа к этой вкладке.");
            }
        }
        public Form2()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            guan2TabControl1.Selecting += guan2TabControl1_Selecting;
            guna2ComboBox3.Items.AddRange(new string[] { "Название", "Категория", "Язык" }); // поиск для фрилансера
            guna2ComboBox5.Items.AddRange(new string[] { "Имя", "Фамилия", "Телефон", "Логин", "Роль" }); // поиск для администратора (пользователи)
            guna2ComboBox6.Items.AddRange(new string[] { "Название", "Категория", "Язык", "Телефон заказчика", "Телефон фрилансера" }); // поиск для администратора (проекты)
            LoadCategories();
            LoadLanguages();
            LoadSpecializations();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            switch (currentRole)
            {
                case "Заказчик":
                    guan2TabControl1.SelectedIndex = 0;
                    LoadZakazchikData();
                    break;
                case "Фрилансер":
                    guan2TabControl1.SelectedIndex = 1;
                    LoadFreelancerData();
                    break;
                case "Администратор":
                    guan2TabControl1.SelectedIndex = 2;
                    LoadUserStatistics();
                    break;
            }
        }
        private void guna2CircleButton1_Click(object sender, EventArgs e) // закрыть форму 2
        {
            (guna2DataGridView2.DataSource as DataTable)?.Clear();
            (guna2DataGridView6.DataSource as DataTable)?.Clear();
            guna2ComboBox5.SelectedIndex = -1;
            guna2ComboBox6.SelectedIndex = -1;
            guna2TextBox4.Text = string.Empty;
            guna2TextBox8.Text = string.Empty;
            (guna2DataGridView5.DataSource as DataTable)?.Clear();
            (guna2DataGridView7.DataSource as DataTable)?.Clear();
            this.Close();
        }
        // интерфейс заказчика
        private void LoadZakazchikData() // загрузка данных заказчика
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id, familiya AS 'Фамилия', imya AS 'Имя', telefon AS 'Телефон', login AS 'Логин', parol AS 'Пароль' " + "FROM users WHERE id = @currentUserId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@currentUserId", currentUserId);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                guna2DataGridView1.DataSource = dataTable;
                guna2DataGridView1.Columns["id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void UpdateZakazchikData(int selectedId) // обновлление данных заказчика
        {
            try
            {           
                string familiya = guna2DataGridView1.CurrentRow.Cells["Фамилия"].Value.ToString();
                string imya = guna2DataGridView1.CurrentRow.Cells["Имя"].Value.ToString();
                string telefon = guna2DataGridView1.CurrentRow.Cells["Телефон"].Value.ToString();
                string login = guna2DataGridView1.CurrentRow.Cells["Логин"].Value.ToString();
                string parol = guna2DataGridView1.CurrentRow.Cells["Пароль"].Value.ToString();
                string query = "UPDATE users SET familiya = @familiya, imya = @imya, telefon = @telefon, login = @login, parol = @parol WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {          
                
                    command.Parameters.AddWithValue("@familiya", familiya);
                    command.Parameters.AddWithValue("@imya", imya);
                    command.Parameters.AddWithValue("@telefon", telefon);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@parol", parol);
                    command.Parameters.AddWithValue("@id", selectedId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    LoadZakazchikData();
                    MessageBox.Show("Данные успешно обновлены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2Button2_Click_1(object sender, EventArgs e) // изменить данные заказчика
        {
            if (guna2DataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите запись для обновления.");
                return;
            }
            int selectedId = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);
            UpdateZakazchikData(selectedId);
        }
        private void LoadCategories() // категории
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT nazvanie FROM kategorii";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    guna2ComboBox1.Items.Add(reader["nazvanie"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private int GetCategoryId(string kategoriaName) // id категории
        {
            int kategoriaId = -1;
            if (string.IsNullOrEmpty(kategoriaName))
            {
                return kategoriaId;
            }
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id FROM kategorii WHERE nazvanie = @kategoriaName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@kategoriaName", kategoriaName);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    kategoriaId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения ID категории: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return kategoriaId;
        }
        private void LoadLanguages() // языки
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT nazvanie FROM yazyki";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    guna2ComboBox2.Items.Add(reader["nazvanie"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки языков: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private int GetLanguageId(string languageName) // id языка
        {
            int languageId = -1;
            if (string.IsNullOrEmpty(languageName))
            {
                return languageId;
            }
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id FROM yazyki WHERE nazvanie = @languageName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@languageName", languageName);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    languageId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения ID языка: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return languageId;
        }
        private void guna2Button3_Click(object sender, EventArgs e) // создать вакансию
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string nazvanie = guna2TextBox1.Text;
                    string kategoria = guna2ComboBox1.SelectedItem?.ToString();
                    string yazik = guna2ComboBox2.SelectedItem?.ToString();
                    string opisanie = textBox1.Text;
                    DateTime srok_vipolnenia = guna2DateTimePicker1.Value;
                    decimal byudzhet = (decimal)guna2NumericUpDown1.Value;
                    if (string.IsNullOrEmpty(nazvanie) ||
                      string.IsNullOrEmpty(kategoria) ||
                      string.IsNullOrEmpty(yazik) ||
                      string.IsNullOrEmpty(opisanie) ||
                      byudzhet == 0)
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля.");
                        return;
                    }
                    int kategoriaId = GetCategoryId(kategoria);
                    int yazikId = GetLanguageId(yazik);
                    if (kategoriaId == -1 || yazikId == -1)
                    {
                        MessageBox.Show("Ошибка: Не удалось найти категорию или язык.");
                        return;
                    }
                    string query = "INSERT INTO vakansii (nazvanie, kategoria_id, opisanie, yazik_id, srok_vipolnenia, byudzhet, user_id) " +
                    "VALUES (@nazvanie, @kategoria_id, @opisanie, @yazik_id, @srok_vipolnenia, @byudzhet, @user_id)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nazvanie", nazvanie);
                        command.Parameters.AddWithValue("@kategoria_id", kategoriaId);
                        command.Parameters.AddWithValue("@opisanie", opisanie);
                        command.Parameters.AddWithValue("@yazik_id", yazikId);
                        command.Parameters.AddWithValue("@srok_vipolnenia", srok_vipolnenia);
                        command.Parameters.AddWithValue("@byudzhet", byudzhet);
                        command.Parameters.AddWithValue("@user_id", currentUserId);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Вакансия успешно добавлена!");
                    guna2TextBox1.Text = string.Empty;
                    textBox1.Text = string.Empty;
                    guna2ComboBox1.SelectedIndex = -1;
                    guna2ComboBox2.SelectedIndex = -1;
                    guna2DateTimePicker1.Value = DateTime.Now;
                    guna2NumericUpDown1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления вакансии: " + ex.Message);
            }
        }
        private void guna2Button1_Click_1(object sender, EventArgs e) // смотреть вакансии
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = @" SELECT v.nazvanie, k.nazvanie AS kategoria, v.opisanie, y.nazvanie AS yazik, v.srok_vipolnenia, v.byudzhet, v.status, 
                CASE WHEN v.status = 1 THEN u.familiya ELSE NULL END AS familiya_freelancer, CASE WHEN v.status = 1 THEN u.imya ELSE NULL END AS imya_freelancer, 
                CASE WHEN v.status = 1 THEN u.telefon ELSE NULL END AS telefon_freelancer, s.nazvanie AS specializaciya_freelancer, z.opyt_rabot AS opyt_rabot_freelancer FROM vakansii v 
                JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users u ON z.user_id = u.id LEFT 
                JOIN specializations s ON z.specialization_id = s.id WHERE v.user_id = @user_id;";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@user_id", currentUserId);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    guna2DataGridView2.DataSource = dt;
                    guna2DataGridView2.Columns["status"].ValueType = typeof(bool);
                    guna2DataGridView2.Columns["status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView2.Columns["status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView2.Columns["status"].ReadOnly = true;
                    guna2DataGridView2.Columns[0].HeaderText = "Название";
                    guna2DataGridView2.Columns[1].HeaderText = "Категория";
                    guna2DataGridView2.Columns[2].HeaderText = "Описание";
                    guna2DataGridView2.Columns[3].HeaderText = "Язык";
                    guna2DataGridView2.Columns[4].HeaderText = "Срок выполнения";
                    guna2DataGridView2.Columns[5].HeaderText = "Бюджет";
                    guna2DataGridView2.Columns[6].HeaderText = "Статус";
                    guna2DataGridView2.Columns[7].HeaderText = "Фамилия фрилансера";
                    guna2DataGridView2.Columns[8].HeaderText = "Имя фрилансера";
                    guna2DataGridView2.Columns[9].HeaderText = "Телефон фрилансера";
                    guna2DataGridView2.Columns[10].HeaderText = "Специализация фрилансера";
                    guna2DataGridView2.Columns[11].HeaderText = "Опыт работы фрилансера";
                    guna2DataGridView2.Columns["srok_vipolnenia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    guna2DataGridView2.Columns["familiya_freelancer"].Visible = true;
                    guna2DataGridView2.Columns["imya_freelancer"].Visible = true;
                    guna2DataGridView2.Columns["telefon_freelancer"].Visible = true;
                    guna2DataGridView2.Columns["specializaciya_freelancer"].Visible = true;
                    guna2DataGridView2.Columns["opyt_rabot_freelancer"].Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        // интерфейс фрилансера
        private void LoadFreelancerData() // загрузка данных фрилансера
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id, familiya AS 'Фамилия', imya AS 'Имя', telefon AS 'Телефон', login AS 'Логин', parol AS 'Пароль' " + "FROM users WHERE id = @currentUserId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@currentUserId", currentUserId);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                guna2DataGridView4.DataSource = dataTable;
                guna2DataGridView4.Columns["id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void UpdateFreelancerData(int selectedId) // обновлление данных фрилансера
        {
            try
            {           
                string familiya = guna2DataGridView4.CurrentRow.Cells["Фамилия"].Value.ToString();
                string imya = guna2DataGridView4.CurrentRow.Cells["Имя"].Value.ToString();
                string telefon = guna2DataGridView4.CurrentRow.Cells["Телефон"].Value.ToString();
                string login = guna2DataGridView4.CurrentRow.Cells["Логин"].Value.ToString();
                string parol = guna2DataGridView4.CurrentRow.Cells["Пароль"].Value.ToString();
                string query = "UPDATE users SET imya = @imya, familiya = @familiya, telefon = @telefon, login = @login, parol = @parol WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {               
                    command.Parameters.AddWithValue("@familiya", familiya);
                    command.Parameters.AddWithValue("@imya", imya);
                    command.Parameters.AddWithValue("@telefon", telefon);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@parol", parol);
                    command.Parameters.AddWithValue("@id", selectedId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    LoadFreelancerData();
                    MessageBox.Show("Данные успешно обновлены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2Button5_Click(object sender, EventArgs e) // изменить данные фрилансера
        {
            if (guna2DataGridView4.CurrentRow == null)
            {
                MessageBox.Show("Выберите запись для обновления.");
                return;
            }
            int selectedId = Convert.ToInt32(guna2DataGridView4.CurrentRow.Cells["id"].Value);
            UpdateFreelancerData(selectedId);
        }
        private void LoadSpecializations() // специализация
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT nazvanie FROM specializations";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                guna2ComboBox4.Items.Clear();
                while (reader.Read())
                {
                    guna2ComboBox4.Items.Add(reader["nazvanie"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки специализаций: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private int GetSpecializationId(string specializationName) // id специализации
        {
            int specializationId = -1;
            if (string.IsNullOrEmpty(specializationName))
            {
                return specializationId;
            }
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id FROM specializations WHERE nazvanie = @specializationName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@specializationName", specializationName);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    specializationId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения ID специализации: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return specializationId;
        }    
        private void guna2Button4_Click(object sender, EventArgs e) // поиск вакансии
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string searchColumn = guna2ComboBox3.SelectedItem?.ToString();
                string query = "";
                if (searchColumn == "Название")
                {
                    query = "SELECT v.nazvanie AS 'Название', k.nazvanie AS kategoria, v.opisanie, y.nazvanie AS yazik, v.srok_vipolnenia, v.byudzhet, v.status " +
                        "FROM vakansii v " +
                        "JOIN kategorii k ON v.kategoria_id = k.id " +
                        "JOIN yazyki y ON v.yazik_id = y.id " +
                        "WHERE v.nazvanie LIKE @search AND v.status = 0";
                }
                else if (searchColumn == "Категория")
                {
                    query = "SELECT v.nazvanie AS 'Название', k.nazvanie AS kategoria, v.opisanie, y.nazvanie AS yazik, v.srok_vipolnenia, v.byudzhet, v.status " +
                        "FROM vakansii v " +
                        "JOIN kategorii k ON v.kategoria_id = k.id " +
                        "JOIN yazyki y ON v.yazik_id = y.id " +
                        "WHERE k.nazvanie LIKE @search AND v.status = 0";
                }
                else if (searchColumn == "Язык")
                {
                    query = "SELECT v.nazvanie AS 'Название', k.nazvanie AS kategoria, v.opisanie, y.nazvanie AS yazik, v.srok_vipolnenia, v.byudzhet, v.status " +
                        "FROM vakansii v " +
                        "JOIN kategorii k ON v.kategoria_id = k.id " +
                        "JOIN yazyki y ON v.yazik_id = y.id " +
                        "WHERE y.nazvanie LIKE @search AND v.status = 0";
                }
                else
                {
                    MessageBox.Show("Выберите допустимый столбец для поиска.");
                    return;
                }
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", "%" + guna2TextBox2.Text + "%");
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    guna2DataGridView3.DataSource = null;
                    guna2DataGridView3.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        guna2DataGridView3.DataSource = dt;
                        guna2DataGridView3.Columns[0].HeaderText = "Название";
                        guna2DataGridView3.Columns[1].HeaderText = "Категория";
                        guna2DataGridView3.Columns[2].HeaderText = "Описание";
                        guna2DataGridView3.Columns[3].HeaderText = "Язык";
                        guna2DataGridView3.Columns[4].HeaderText = "Срок выполнения";
                        guna2DataGridView3.Columns[5].HeaderText = "Бюджет";
                        guna2DataGridView3.Columns[6].HeaderText = "Статус";
                        guna2DataGridView3.Columns["status"].ValueType = typeof(bool);
                        guna2DataGridView3.Columns["status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        guna2DataGridView3.Columns["status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        guna2DataGridView3.Columns["status"].ReadOnly = true;
                        guna2DataGridView3.Columns["srok_vipolnenia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }
                    else
                    {
                        MessageBox.Show("Совпадений не найдено.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }      
        private int GetVakansiyaId(string nazvanie) // id вакансии по названию
        {
            int vakansiyaId = -1;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id FROM vakansii WHERE nazvanie = @nazvanie";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nazvanie", nazvanie);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    vakansiyaId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения ID вакансии: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return vakansiyaId;
        }
        private void guna2Button6_Click(object sender, EventArgs e) // подача заявки
        {
            string specializationName = guna2ComboBox4.SelectedItem?.ToString();
            string experience = guna2TextBox3.Text;
            if (string.IsNullOrEmpty(specializationName) || string.IsNullOrEmpty(experience))
            {
                MessageBox.Show("Пожалуйста, выберите специализацию и введите опыт работы.");
                return;
            }
            int specializationId = GetSpecializationId(specializationName);
            if (specializationId == -1)
            {
                MessageBox.Show("Не удалось получить ID специализации.");
                return;
            }
            int vakansiyaId = -1;
            foreach (DataGridViewRow row in guna2DataGridView3.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Status"].Value))
                {
                    vakansiyaId = GetVakansiyaId(row.Cells["Название"].Value.ToString());
                    break;
                }
            }
            if (vakansiyaId == -1)
            {
                MessageBox.Show("Пожалуйста, выберите вакансию.");
                return;
            }
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string insertQuery = "INSERT INTO zayavki (user_id, specialization_id, vakansiya_id, opyt_rabot) VALUES (@userId, @specializationId, @vakansiyaId, @opyt_rabot)";
                command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@specializationId", specializationId);
                command.Parameters.AddWithValue("@vakansiyaId", vakansiyaId);
                command.Parameters.AddWithValue("@opyt_rabot", experience);
                command.ExecuteNonQuery();
                MessageBox.Show("Заявка успешно подана.");
                string updateQuery = "UPDATE vakansii SET status = 1 WHERE id = @vakansiyaId";
                command = new MySqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@vakansiyaId", vakansiyaId);
                command.ExecuteNonQuery();
                (guna2DataGridView3.DataSource as DataTable)?.Clear();
                guna2TextBox2.Text = string.Empty;
                guna2TextBox3.Text = string.Empty;
                guna2ComboBox3.SelectedIndex = -1;
                guna2ComboBox4.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подаче заявки: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2DataGridView3_CellClick(object sender, DataGridViewCellEventArgs e) // галочкка
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == guna2DataGridView3.Columns["Status"].Index)
            {
                bool currentValue = Convert.ToBoolean(guna2DataGridView3.Rows[e.RowIndex].Cells["Status"].Value);
                guna2DataGridView3.Rows[e.RowIndex].Cells["Status"].Value = !currentValue;
            }
        }
        private void guna2Button11_Click(object sender, EventArgs e) // смотреть заявки
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = @"SELECT v.nazvanie AS 'Название', k.nazvanie AS 'Категория', v.opisanie AS 'Описание', 
                y.nazvanie AS 'Язык', v.srok_vipolnenia AS 'Срок выполнения', v.byudzhet AS 'Бюджет', u.familiya AS 'Фамилия заказчика', 
                u.imya AS 'Имя заказчика', u.telefon AS 'Телефон заказчика' FROM zayavki z JOIN vakansii v ON z.vakansiya_id = v.id JOIN kategorii k ON v.kategoria_id = k.id 
                JOIN yazyki y ON v.yazik_id = y.id JOIN users u ON v.user_id = u.id WHERE z.user_id = @freelancerId;";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@freelancerId", currentUserId);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    guna2DataGridView6.DataSource = dt;
                    guna2DataGridView6.Columns[0].HeaderText = "Название";
                    guna2DataGridView6.Columns[1].HeaderText = "Категория";
                    guna2DataGridView6.Columns[2].HeaderText = "Описание";
                    guna2DataGridView6.Columns[3].HeaderText = "Язык";
                    guna2DataGridView6.Columns[4].HeaderText = "Срок выполнения";
                    guna2DataGridView6.Columns[5].HeaderText = "Бюджет";
                    guna2DataGridView6.Columns[6].HeaderText = "Фамилия заказчика";
                    guna2DataGridView6.Columns[7].HeaderText = "Имя заказчика";
                    guna2DataGridView6.Columns[8].HeaderText = "Телефон заказчика";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        // интерфейс администратора
        private int GetTotalUsers() // получить общее количество пользователей
        {
            int totalUsers = 0;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT COUNT(*) FROM users;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    totalUsers = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения общего количества пользователей: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return totalUsers;
        }
        private int GetZakazchikCount() // получить количество заказчиков
        {
            int clientCount = 0;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT COUNT(*) FROM users WHERE role = 'Заказчик';";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    clientCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения количества заказчиков: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return clientCount;
        }
        private int GetFreelancerCount() // получить количество фрилансеров
        {
            int freelancerCount = 0;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT COUNT(*) FROM users WHERE role = 'Фрилансер';";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    freelancerCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения количества фрилансеров: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return freelancerCount;
        }
        private int GetVacancyCount() // получить количество проектов
        {
            int vacancyCount = 0;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT COUNT(*) FROM vakansii;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    vacancyCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения количества вакансий: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return vacancyCount;
        }
        private void LoadUserStatistics()
        {
            guna2TextBox5.Text = GetTotalUsers().ToString();
            guna2TextBox7.Text = GetZakazchikCount().ToString();
            guna2TextBox6.Text = GetFreelancerCount().ToString();
            guna2TextBox9.Text = GetVacancyCount().ToString();
        }     
        private void guna2Button8_Click_1(object sender, EventArgs e) // вывод всех пользователей
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        // Сделать отдельный метод для вызова чтобы убрать дублирование
                        guna2DataGridView5.DataSource = dt;
                        guna2DataGridView5.Columns[0].HeaderText = "ID";
                        guna2DataGridView5.Columns[1].HeaderText = "Фамилия";
                        guna2DataGridView5.Columns[2].HeaderText = "Имя";
                        guna2DataGridView5.Columns[3].HeaderText = "Телефон";
                        guna2DataGridView5.Columns[4].HeaderText = "Логин";
                        guna2DataGridView5.Columns[5].HeaderText = "Пароль";
                        guna2DataGridView5.Columns[6].HeaderText = "Роль";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных о пользователях: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2Button7_Click_1(object sender, EventArgs e) // поиск пользователей
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string searchColumn = guna2ComboBox5.SelectedItem?.ToString();
                string query = "";
                if (searchColumn == "Фамилия")
                {
                    query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users WHERE familiya LIKE @search";
                }
                else if (searchColumn == "Имя")
                {
                    query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users WHERE imya LIKE @search";
                }
                else if (searchColumn == "Телефон")
                {
                    query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users WHERE telefon LIKE @search";
                }
                else if (searchColumn == "Логин")
                {
                    query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users WHERE login LIKE @search";
                }
                else if (searchColumn == "Роль")
                {
                    query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users WHERE role LIKE @search";
                }
                else
                {
                    MessageBox.Show("Выберите допустимый столбец для поиска.");
                    return;
                }
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", "%" + guna2TextBox4.Text + "%");
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    guna2DataGridView5.DataSource = null;
                    guna2DataGridView5.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        guna2DataGridView5.DataSource = dt;
                        guna2DataGridView5.Columns[0].HeaderText = "ID";
                        guna2DataGridView5.Columns[1].HeaderText = "Фамилия";
                        guna2DataGridView5.Columns[2].HeaderText = "Имя";
                        guna2DataGridView5.Columns[3].HeaderText = "Телефон";
                        guna2DataGridView5.Columns[4].HeaderText = "Логин";
                        guna2DataGridView5.Columns[5].HeaderText = "Пароль";
                        guna2DataGridView5.Columns[6].HeaderText = "Роль";
                    }
                    else
                    {
                        MessageBox.Show("Совпадений не найдено.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска данных: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void LoadAllUsers() // загрузка данных всех пользователей
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = "SELECT id, familiya, imya, telefon, login, parol, role FROM users";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        guna2DataGridView5.DataSource = dt;
                        guna2DataGridView5.Columns[0].HeaderText = "ID";
                        guna2DataGridView5.Columns[1].HeaderText = "Фамилия";
                        guna2DataGridView5.Columns[2].HeaderText = "Имя";
                        guna2DataGridView5.Columns[3].HeaderText = "Телефон";
                        guna2DataGridView5.Columns[4].HeaderText = "Логин";
                        guna2DataGridView5.Columns[5].HeaderText = "Пароль";
                        guna2DataGridView5.Columns[6].HeaderText = "Роль";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных о пользователях: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void UpdateUserData(int selectedUserId) // обновление данных всех пользователей
        {
            try
            {
                string familiya = guna2DataGridView5.CurrentRow.Cells[1].Value.ToString();
                string imya = guna2DataGridView5.CurrentRow.Cells[2].Value.ToString();
                string telefon = guna2DataGridView5.CurrentRow.Cells[3].Value.ToString();
                string login = guna2DataGridView5.CurrentRow.Cells[4].Value.ToString();
                string parol = guna2DataGridView5.CurrentRow.Cells[5].Value.ToString();
                string role = guna2DataGridView5.CurrentRow.Cells[6].Value.ToString();
                string query = "UPDATE users SET familiya = @familiya, imya = @imya, telefon = @telefon, login = @login, parol = @parol, role = @role WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@familiya", familiya);
                    command.Parameters.AddWithValue("@imya", imya);
                    command.Parameters.AddWithValue("@telefon", telefon);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@parol", parol);
                    command.Parameters.AddWithValue("@role", role);
                    command.Parameters.AddWithValue("@id", selectedUserId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    LoadAllUsers();
                    MessageBox.Show("Данные пользователя успешно обновлены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления данных пользователя: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2Button12_Click(object sender, EventArgs e) // администратор изменяет данные пользователя
        {
            if (guna2DataGridView5.CurrentRow == null)
            {
                MessageBox.Show("Выберите запись для обновления.");
                return;
            }
            int selectedUserId = Convert.ToInt32(guna2DataGridView5.CurrentRow.Cells["id"].Value);
            UpdateUserData(selectedUserId);
        }    
        private void guna2Button9_Click(object sender, EventArgs e) // вывод всех проектов для администратор
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, 
                v.status, CASE WHEN v.status = 1 THEN z.user_id ELSE NULL END AS Freelancer_ID, CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v 
                JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users uf ON z.user_id = uf.id; ";
                command = new MySqlCommand(query, connection);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    guna2DataGridView7.DataSource = dt;
                    guna2DataGridView7.Columns[0].HeaderText = "ID";
                    guna2DataGridView7.Columns[1].HeaderText = "Название";
                    guna2DataGridView7.Columns[2].HeaderText = "Категория";
                    guna2DataGridView7.Columns[3].HeaderText = "Описание";
                    guna2DataGridView7.Columns[4].HeaderText = "Язык";
                    guna2DataGridView7.Columns[5].HeaderText = "Срок выполнения";
                    guna2DataGridView7.Columns[6].HeaderText = "Бюджет";
                    guna2DataGridView7.Columns[7].HeaderText = "Телефон заказчика";
                    guna2DataGridView7.Columns[8].HeaderText = "Статус";
                    guna2DataGridView7.Columns[9].HeaderText = "ID фрилансера";
                    guna2DataGridView7.Columns[10].HeaderText = "Телефон фрилансера";
                    guna2DataGridView7.Columns["srok_vipolnenia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    guna2DataGridView7.Columns["status"].ValueType = typeof(bool);
                    guna2DataGridView7.Columns["status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView7.Columns["status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView7.Columns["status"].ReadOnly = true;
                    guna2DataGridView7.Columns["Freelancer_ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных о проектах: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2Button10_Click(object sender, EventArgs e) // поиск проектов для администратора
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string searchColumn = guna2ComboBox6.SelectedItem?.ToString();
                string query = "";
                if (searchColumn == "Название")
                {
                    // Возможно создания метода чтоьы убрать дублирование
                    query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, v.status, 
                    CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id 
                    JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users uf ON z.user_id = uf.id WHERE v.nazvanie LIKE @search; ";
                }
                else if (searchColumn == "Категория")
                {
                    query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, v.status, 
                    CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id 
                    JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users uf ON z.user_id = uf.id WHERE k.nazvanie LIKE @search; ";
                }
                else if (searchColumn == "Язык")
                {
                    query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, v.status, 
                    CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id 
                    JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users uf ON z.user_id = uf.id WHERE y.nazvanie LIKE @search; ";
                }
                else if (searchColumn == "Телефон заказчика")
                {
                    query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, v.status, 
                    CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id 
                    JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users uf ON z.user_id = uf.id WHERE u.telefon LIKE @search; ";
                }
                else if (searchColumn == "Телефон фрилансера")
                {
                    query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, v.status, 
                    CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id 
                    JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT JOIN users uf ON z.user_id = uf.id WHERE uf.telefon LIKE @search; ";
                }
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", "%" + guna2TextBox8.Text + "%");
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    // дублирование кода 
                    guna2DataGridView7.DataSource = dt;
                    guna2DataGridView7.Columns[0].HeaderText = "ID";
                    guna2DataGridView7.Columns[1].HeaderText = "Название";
                    guna2DataGridView7.Columns[2].HeaderText = "Категория";
                    guna2DataGridView7.Columns[3].HeaderText = "Описание";
                    guna2DataGridView7.Columns[4].HeaderText = "Язык";
                    guna2DataGridView7.Columns[5].HeaderText = "Срок выполнения";
                    guna2DataGridView7.Columns[6].HeaderText = "Бюджет";
                    guna2DataGridView7.Columns[7].HeaderText = "Телефон заказчика";
                    guna2DataGridView7.Columns[8].HeaderText = "Статус";
                    guna2DataGridView7.Columns[9].HeaderText = "Телефон фрилансера";
                    guna2DataGridView7.Columns["srok_vipolnenia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    guna2DataGridView7.Columns["status"].ValueType = typeof(bool);
                    guna2DataGridView7.Columns["status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView7.Columns["status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView7.Columns["status"].ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска проектов: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }     
        private void LoadAllProjects() // загрузка данных всех вакансий для администратора
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                string query = @" SELECT v.id, v.nazvanie, k.nazvanie AS Kategoria, v.opisanie, y.nazvanie AS Yazik, v.srok_vipolnenia, v.byudzhet, u.telefon AS Telefon_Zakazchika, v.status, 
                CASE WHEN v.status = 1 THEN z.user_id ELSE NULL END AS Freelancer_ID, CASE WHEN v.status = 1 THEN uf.telefon ELSE NULL END AS Telefon_Freelancera FROM vakansii v 
                JOIN kategorii k ON v.kategoria_id = k.id JOIN yazyki y ON v.yazik_id = y.id JOIN users u ON v.user_id = u.id LEFT JOIN zayavki z ON v.id = z.vakansiya_id LEFT 
                JOIN users uf ON z.user_id = uf.id; ";
                command = new MySqlCommand(query, connection);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    guna2DataGridView7.DataSource = dt;
                    guna2DataGridView7.Columns[0].HeaderText = "ID";
                    guna2DataGridView7.Columns[1].HeaderText = "Название";
                    guna2DataGridView7.Columns[2].HeaderText = "Категория";
                    guna2DataGridView7.Columns[3].HeaderText = "Описание";
                    guna2DataGridView7.Columns[4].HeaderText = "Язык";
                    guna2DataGridView7.Columns[5].HeaderText = "Срок выполнения";
                    guna2DataGridView7.Columns[6].HeaderText = "Бюджет";
                    guna2DataGridView7.Columns[7].HeaderText = "Телефон заказчика";
                    guna2DataGridView7.Columns[8].HeaderText = "Статус";
                    guna2DataGridView7.Columns[9].HeaderText = "ID фрилансера";
                    guna2DataGridView7.Columns[10].HeaderText = "Телефон фрилансера";
                    guna2DataGridView7.Columns["srok_vipolnenia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    guna2DataGridView7.Columns["status"].ValueType = typeof(bool);
                    guna2DataGridView7.Columns["status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView7.Columns["status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView7.Columns["status"].ReadOnly = true;
                    guna2DataGridView7.Columns["Freelancer_ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных о проектах: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void UpdateVacancyData(int selectedVacancyId) // обновление всех вакансий для администратора
        {
            try
            {
                string nazvanie = guna2DataGridView7.CurrentRow.Cells[1].Value.ToString();
                string kategoria = guna2DataGridView7.CurrentRow.Cells[2].Value.ToString();
                string opisanie = guna2DataGridView7.CurrentRow.Cells[3].Value.ToString();
                string yazik = guna2DataGridView7.CurrentRow.Cells[4].Value.ToString();
                DateTime srok_vipolnenia = Convert.ToDateTime(guna2DataGridView7.CurrentRow.Cells[5].Value);
                decimal byudzhet = Convert.ToDecimal(guna2DataGridView7.CurrentRow.Cells[6].Value);
                string query = "UPDATE vakansii SET nazvanie = @nazvanie, kategoria_id = @kategoria_id, opisanie = @opisanie, yazik_id = @yazik_id, srok_vipolnenia = @srok_vipolnenia, byudzhet = @byudzhet WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nazvanie", nazvanie);
                    command.Parameters.AddWithValue("@kategoria_id", GetCategoryId(kategoria));
                    command.Parameters.AddWithValue("@opisanie", opisanie);
                    command.Parameters.AddWithValue("@yazik_id", GetLanguageId(yazik));
                    command.Parameters.AddWithValue("@srok_vipolnenia", srok_vipolnenia);
                    command.Parameters.AddWithValue("@byudzhet", byudzhet);
                    command.Parameters.AddWithValue("@id", selectedVacancyId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    LoadAllProjects();
                    MessageBox.Show("Данные вакансии успешно обновлены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления данных вакансии: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void guna2Button13_Click(object sender, EventArgs e) // администратор изменяет данные вакансии
        {
            if (guna2DataGridView7.CurrentRow == null)
            {
                MessageBox.Show("Выберите запись для обновления.");
                return;
            }
            int selectedVacancyId = Convert.ToInt32(guna2DataGridView7.CurrentRow.Cells[0].Value);
            int editingColumnIndex = guna2DataGridView7.CurrentCell.ColumnIndex;
            if (editingColumnIndex == 7 || editingColumnIndex == 10)
            {
                MessageBox.Show("Изменить номер телефона заказчика или фрилансера можно только в разделе \"Пользователи\".");
                return;
            }
            UpdateVacancyData(selectedVacancyId);
        }
    }
}
