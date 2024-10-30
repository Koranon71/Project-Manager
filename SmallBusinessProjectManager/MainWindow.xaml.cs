using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace SmallBusinessProjectManager
{
    public partial class MainWindow : Window
    {
        private string dbPath = "Data Source=SmallBusinessProjects.db";

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        // Метод инициализации базы данных и создание таблицы
        private void InitializeDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbPath))
            {
                connection.Open();
                string tableCreationQuery = @"
                    CREATE TABLE IF NOT EXISTS Projects (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProjectName TEXT NOT NULL,
                        Description TEXT
                    )";
                SQLiteCommand command = new SQLiteCommand(tableCreationQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        // Метод для загрузки данных из SQLite
        private void LoadData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbPath))
            {
                connection.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM Projects", connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        // Метод для добавления данных
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(dbPath))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand("INSERT INTO Projects (ProjectName, Description) VALUES (@name, @description)", connection);
                command.Parameters.AddWithValue("@name", projectNameTextBox.Text);
                command.Parameters.AddWithValue("@description", projectDescriptionTextBox.Text);
                command.ExecuteNonQuery();
            }
            LoadData();
            ClearFields();
        }

        // Метод для обновления данных
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView row)
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbPath))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("UPDATE Projects SET ProjectName=@name, Description=@description WHERE Id=@id", connection);
                    command.Parameters.AddWithValue("@name", projectNameTextBox.Text);
                    command.Parameters.AddWithValue("@description", projectDescriptionTextBox.Text);
                    command.Parameters.AddWithValue("@id", row["Id"]);
                    command.ExecuteNonQuery();
                }
                LoadData();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Выберите запись для обновления.");
            }
        }

        // Метод для удаления данных
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView row)
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbPath))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("DELETE FROM Projects WHERE Id=@id", connection);
                    command.Parameters.AddWithValue("@id", row["Id"]);
                    command.ExecuteNonQuery();
                }
                LoadData();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        // Метод для очистки полей ввода
        private void ClearFields()
        {
            projectNameTextBox.Clear();
            projectDescriptionTextBox.Clear();
        }

        // Выбор строки для обновления
        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView row)
            {
                projectNameTextBox.Text = row["ProjectName"].ToString();
                projectDescriptionTextBox.Text = row["Description"].ToString();
            }
        }
    }
}