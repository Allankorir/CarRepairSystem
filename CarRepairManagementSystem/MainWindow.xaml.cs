using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarRepairManagementSystem.Models;
using System.Data.SqlClient;


namespace CarRepairManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            dbHelper = new DatabaseHelper();
            RefreshVehicleDataGrid();
            LoadInventoryData();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the maximum date for the DatePicker to today
            YearDatePicker.DisplayDateEnd = DateTime.Now;
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Example: Add a new vehicle
            DatabaseHelper dbHelper = new DatabaseHelper();
            string insertQuery = "INSERT INTO Vehicle (Make, Model, Year, NewUsed) VALUES (@Make, @Model, @Year, @NewUsed)";

            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@Make", MakeTextBox.Text);
                cmd.Parameters.AddWithValue("@Model", ModelTextBox.Text);

                // Use the Year from the DatePicker
                if (YearDatePicker.SelectedDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@Year", YearDatePicker.SelectedDate.Value.Year);
                }
                else
                {
                    MessageBox.Show("Please select a valid year.");
                    return;
                }

                cmd.Parameters.AddWithValue("@NewUsed", NewUsedCheckBox.IsChecked.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            RefreshVehicleDataGrid();
        }

        private void RefreshVehicleDataGrid()
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            string query = "SELECT * FROM Vehicle";
            DataSet dataSet = dbHelper.GetData(query);
            VehicleDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper dbHelper = new DatabaseHelper();

            // Example: Load Vehicle data
            DataSet vehicleData = dbHelper.GetData("SELECT * FROM Vehicle");
            VehicleDataGrid.ItemsSource = vehicleData.Tables[0].DefaultView;
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a vehicle is selected in the DataGrid
            if (VehicleDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a vehicle to update.");
                return;
            }

            // Retrieve selected vehicle's ID
            DataRowView selectedRow = (DataRowView)VehicleDataGrid.SelectedItem;
            int vehicleId = (int)selectedRow["ID"]; // Ensure the column name matches the database

            // Prepare the update query
            string updateQuery = "UPDATE Vehicle SET Make = @Make, Model = @Model, Year = @Year, NewUsed = @NewUsed WHERE ID = @ID";

            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@Make", MakeTextBox.Text);
                cmd.Parameters.AddWithValue("@Model", ModelTextBox.Text);

                // Use the Year from the DatePicker
                if (YearDatePicker.SelectedDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@Year", YearDatePicker.SelectedDate.Value.Year);
                }
                else
                {
                    MessageBox.Show("Please select a valid year.");
                    return;
                }

                cmd.Parameters.AddWithValue("@NewUsed", NewUsedCheckBox.IsChecked.Value);
                cmd.Parameters.AddWithValue("@ID", vehicleId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Refresh the DataGrid
            RefreshVehicleDataGrid();
        }
        private void VehicleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure a vehicle is selected in the DataGrid
            if (VehicleDataGrid.SelectedItem == null)
            {
                return;
            }

            // Retrieve the selected row
            DataRowView selectedRow = (DataRowView)VehicleDataGrid.SelectedItem;

            // Extract values from the selected row
            string make = selectedRow["Make"].ToString();
            string model = selectedRow["Model"].ToString();
            int year;
            bool isNewUsed = Convert.ToBoolean(selectedRow["NewUsed"]);

            // Convert year to integer
            if (!int.TryParse(selectedRow["Year"].ToString(), out year))
            {
                MessageBox.Show("Invalid Year format.");
                return;
            }

            // Populate controls with selected data
            MakeTextBox.Text = make;
            ModelTextBox.Text = model;
            YearDatePicker.SelectedDate = new DateTime(year, 1, 1); // Set to January 1st of the selected year
            NewUsedCheckBox.IsChecked = isNewUsed;
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a vehicle is selected in the DataGrid
            if (VehicleDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a vehicle to delete.");
                return;
            }

            // Retrieve selected vehicle's ID
            DataRowView selectedRow = (DataRowView)VehicleDataGrid.SelectedItem;
            int vehicleId = (int)selectedRow["ID"]; // Ensure the column name matches the database

            // Prepare the delete query
            string deleteQuery = "DELETE FROM Vehicle WHERE ID = @ID";

            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@ID", vehicleId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Refresh the DataGrid
            RefreshVehicleDataGrid();
        }
        private void LoadInventoryData()
        {
            string query = "SELECT * FROM Inventory";
            DataSet dataSet = dbHelper.GetData(query);
            InventoryDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
        }
        private void AddInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // SQL query to insert data into the Inventory table
                string query = "INSERT INTO Inventory (VehicleID, NumberOnHand, Price, Cost) VALUES (@VehicleID, @NumberOnHand, @Price, @Cost)";

                using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Validate and parse inputs
                    if (int.TryParse(VehicleIDTextBox.Text, out int vehicleID) &&
                        int.TryParse(NumberOnHandTextBox.Text, out int numberOnHand) &&
                        decimal.TryParse(PriceTextBox.Text, out decimal price) &&
                        decimal.TryParse(CostTextBox.Text, out decimal cost))
                    {
                        // Add parameters to SQL command
                        cmd.Parameters.AddWithValue("@VehicleID", vehicleID);
                        cmd.Parameters.AddWithValue("@NumberOnHand", numberOnHand);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Cost", cost);

                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the SQL command
                        MessageBox.Show("Inventory added successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid numeric values.");
                    }
                }
                LoadInventoryData(); // Refresh the DataGrid to show the updated inventory
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        private void UpdateInventory_Click(object sender, RoutedEventArgs e)
        {
            string query = "UPDATE Inventory SET VehicleID = @VehicleID, NumberOnHand = @NumberOnHand, Price = @Price, Cost = @Cost WHERE ID = @ID";
            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", InventoryIDTextBox.Text);
                cmd.Parameters.AddWithValue("@VehicleID", VehicleIDTextBox.Text);
                cmd.Parameters.AddWithValue("@NumberOnHand", NumberOnHandTextBox.Text);
                cmd.Parameters.AddWithValue("@Price", PriceTextBox.Text);
                cmd.Parameters.AddWithValue("@Cost", CostTextBox.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadInventoryData();
        }

        private void DeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            string query = "DELETE FROM Inventory WHERE ID = @ID";
            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", InventoryIDTextBox.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadInventoryData();
        }

        private void InventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is DataRowView selectedRow)
            {
                InventoryIDTextBox.Text = selectedRow["ID"].ToString();
                VehicleIDTextBox.Text = selectedRow["VehicleID"].ToString();
                NumberOnHandTextBox.Text = selectedRow["NumberOnHand"].ToString();
                PriceTextBox.Text = selectedRow["Price"].ToString();
                CostTextBox.Text = selectedRow["Cost"].ToString();
            }
        }
        private void RefreshInventory_Click(object sender, RoutedEventArgs e)
        {
            LoadInventoryData();
        }
        private void LoadRepairData()
        {
            string query = "SELECT * FROM Repair";
            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                var repairs = new List<Repair>();

                while (reader.Read())
                {
                    repairs.Add(new Repair
                    {
                        ID = (int)reader["ID"],
                        InventoryID = (int)reader["InventoryID"],
                        RepairDetails = reader["RepairDetails"].ToString()
                    });
                }
                RepairDataGrid.ItemsSource = repairs;
            }
        }

        // Add Repair
        private void AddRepair_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO Repair (InventoryID, RepairDetails) VALUES (@InventoryID, @RepairDetails)";
            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InventoryID", int.Parse(RepairTab_InventoryIDTextBox.Text));
                cmd.Parameters.AddWithValue("@RepairDetails", RepairDetailsTextBox.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadRepairData();
        }

        // Update Repair
        private void UpdateRepair_Click(object sender, RoutedEventArgs e)
        {
            string query = "UPDATE Repair SET InventoryID = @InventoryID, RepairDetails = @RepairDetails WHERE ID = @ID";
            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", int.Parse(RepairIDTextBox.Text));
                cmd.Parameters.AddWithValue("@InventoryID", int.Parse(InventoryIDTextBox.Text));
                cmd.Parameters.AddWithValue("@RepairDetails", RepairDetailsTextBox.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadRepairData();
        }

        // Delete Repair
        private void DeleteRepair_Click(object sender, RoutedEventArgs e)
        {
            string query = "DELETE FROM Repair WHERE ID = @ID";
            using (SqlConnection conn = new SqlConnection(dbHelper.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", int.Parse(RepairIDTextBox.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadRepairData();
        }

        // Handle Repair DataGrid Selection
        private void RepairDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RepairDataGrid.SelectedItem is Repair selectedRepair)
            {
                RepairIDTextBox.Text = selectedRepair.ID.ToString();
                InventoryIDTextBox.Text = selectedRepair.InventoryID.ToString();
                RepairDetailsTextBox.Text = selectedRepair.RepairDetails;
            }
        }

        // Refresh Repair Data
        private void RefreshRepair_Click(object sender, RoutedEventArgs e)
        {
            LoadRepairData();
        }
    }
}
    