using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //SelectCommand = new RelayCommand(async () =>
            //  {
            //      await
            //  });


            using(var conn=new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
                conn.Open();
                SqlCommand command=conn.CreateCommand();
                command.CommandText = "WAITFOR DELAY '00:00:05';";
                command.CommandText += txtbox.Text;

                var table = new DataTable();

                using (var reader= await command.ExecuteReaderAsync())
                {
                    do
                    {
                        var hasColumnAdded = false;
                        while (await reader.ReadAsync())
                        {
                            if (!hasColumnAdded)
                            {
                                hasColumnAdded = true;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    table.Columns.Add(reader.GetName(i));
                                }
                            }

                            var row=table.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i]=await reader.GetFieldValueAsync<Object>(i);
                            }
                            table.Rows.Add(row);


                        }
                    } while (reader.NextResult());

                    datagrid.ItemsSource = table.DefaultView;
                }
            }
        }
    }
}
