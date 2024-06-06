using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace EHWpfViews
{
    [ComVisible(true), Guid(ClassId)]
    public class WindowWrapper
    {
        public const string ClassId = "3D853E7B-01DA-4944-8E65-5E36B501E889";

        public void ShowWindow(string dbConnectionString)
        {
            try
            {
                var window = new ReportView();
                window.DataContext = new MainViewModel(new ehfleet_classlibrary.General.Database(dbConnectionString));
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
