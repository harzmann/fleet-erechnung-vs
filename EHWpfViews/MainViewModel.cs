using ehfleet_classlibrary;
using Prism.Mvvm;
using Stimulsoft.Report;

namespace EHWpfViews
{
    public class MainViewModel : BindableBase
    {
        private StiReport _report;

        private General.Database _db;

        public MainViewModel(General.Database db)
        {
            _db = db;
        }

        public StiReport Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }
    }
}
