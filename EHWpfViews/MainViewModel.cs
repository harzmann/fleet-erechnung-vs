using Prism.Commands;
using Prism.Mvvm;
using Stimulsoft.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EHWpfViews
{
    public class MainViewModel : BindableBase
    {
        private EHFleetEntities _dbContext;

        private StiReport _report;

        public MainViewModel(EHFleetEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public StiReport Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }
    }
}
