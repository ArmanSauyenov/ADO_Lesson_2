using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Configuration;
using System.Data;



namespace ADO_Lesson2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string connectionString = "";
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = connectionString;

            // SqlDataAdapter da = new SqlDataAdapter("select * from newEquipment",con); // variant1

            // variant2
            DataSet ds = new DataSet();  //некий контейнер который хранит возвращаемый результат
            SqlCommand cmd = new SqlCommand(); //передает нужную команду в базу
            cmd.CommandText = "select top 3 * from AccessTab;";
            //+ " select * from newEquipment;"; //
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            //выгружает данные с cmd используя con
            da.Fill(ds);


            ds.Tables[0].ColumnChanged += ch_ChangeColumn; //delegat


            foreach (DataTable table in ds.Tables)
            {

                Label l = new Label() { Content = table.TableName, FontWeight = FontWeights.Bold };
                spDataInfo.Children.Add(l);

                //foreach (DataColumn column in table.Columns)
                //{
                //    string columnInfo = string.Format("\t{0} - dataType: {1}", column.ColumnName, column.DataType);
                //    Label lColumn = new Label() { Content = column.ColumnName, FontStyle=FontStyles.Italic };
                //    spDataInfo.Children.Add(lColumn);

                //}

                int y = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (y == 0)
                    {
                        labelStatusBar.Content += "\nBefore: " + row.RowState + "\n**";
                        row["strTabName"] = " newValue";
                        labelStatusBar.Content += "\nAfter: " + row.RowState + "\n";

                        y++;


                    }
                    var cells = row.ItemArray;
                    Label lRow = new Label()  { FontStyle = FontStyles.Italic  };

                    foreach (object cel in cells)  {lRow.Content += "\t" + cel.ToString();}
                    spDataInfo.Children.Add(lRow);
                }

                ds.AcceptChanges();

                if (ds.HasChanges())
                {
                    Label lChange = new Label() { FontStyle = FontStyles.Italic,
                    Foreground = new SolidColorBrush(Colors.Red),
                    Content="До изменения"};

                    foreach (DataTable tableChange in ds.GetChanges(DataRowState.Modified).Tables)
                    {
                        foreach(DataRow row in tableChange.Rows)
                        {
                            lChange.Content += "\n-------------------------------------------------\n";
                            foreach (var itemRow in row.ItemArray)
                            {
                                lChange.Content += string.Format("\t Before: {0}; After: {1}", row["strTabName", DataRowVersion.Original],
                                 row["strTabName", DataRowVersion.Current]);
                            }
                            lChange.Content += "\n-------------------------------------------------\n";
                        }
                    }

                    spDataInfo.Children.Add(lChange);
                }

            }
        }

        private void ch_ChangeColumn(object sender, DataColumnChangeEventArgs e)
        {
            labelStatusBar.Content +=

                e.Column + " - " + e.ProposedValue + " - " + e.Row[e.Column, DataRowVersion.Original];
        }


    }
}
    
