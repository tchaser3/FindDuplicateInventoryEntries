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
using InventoryDLL;
using NewPartNumbersDLL;
using NewEventLogDLL;
using DataValidationDLL;
using CharterInventoryDLL;

namespace FindDuplicateInventoryEntries
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CharterInventoryClass TheCharterInventoryClass = new CharterInventoryClass();

        //setting up the data
        InventoryDataSet TheInventoryDataSet;
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();
        DuplicateInventoryEntries TheDuplicateInventoryEntriesDataSet = new DuplicateInventoryEntries();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheInventoryDataSet = TheInventoryClass.GetInventoryInfo();

            dgrResults.ItemsSource = TheDuplicateInventoryEntriesDataSet.duplicates;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intSecondCounter;
            int intPartID;
            int intRecordsReturned;
            int intWarehouseID;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheInventoryDataSet.inventory.Rows.Count - 1;
                TheDuplicateInventoryEntriesDataSet.duplicates.Rows.Clear();

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intPartID = TheInventoryDataSet.inventory[intCounter].PartID;
                    intWarehouseID = TheInventoryDataSet.inventory[intCounter].WarehouseID;

                    TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, intWarehouseID);

                    intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count - 1;

                    if(intRecordsReturned > 0)
                    {
                        for(intSecondCounter = 0; intSecondCounter <= intRecordsReturned; intSecondCounter++)
                        {
                            DuplicateInventoryEntries.duplicatesRow NewDuplicateRow = TheDuplicateInventoryEntriesDataSet.duplicates.NewduplicatesRow();

                            NewDuplicateRow.JDEPartNumber = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[intSecondCounter].JDEPartNumber;
                            NewDuplicateRow.PartDescription = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[intSecondCounter].PartDescription;
                            NewDuplicateRow.PartID = intPartID;
                            NewDuplicateRow.PartNumber = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[intSecondCounter].PartNumber;
                            NewDuplicateRow.Quantity = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[intSecondCounter].Quantity;
                            NewDuplicateRow.WarehouseID = intWarehouseID;

                            TheDuplicateInventoryEntriesDataSet.duplicates.Rows.Add(NewDuplicateRow);
                        }
                    }

                    
                }

                dgrResults.ItemsSource = TheDuplicateInventoryEntriesDataSet.duplicates;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Find Duplicate Inventory Entries // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
    }
}
