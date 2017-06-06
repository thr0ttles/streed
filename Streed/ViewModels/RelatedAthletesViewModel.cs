using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Streed.ViewModels
{
    public class RelatedAthletesViewModel : BaseViewModel
    {
        public ObservableCollection<Strava.Activities.Activity> RelatedActivities { get; set; }

        public string GroupActivityType 
        {
            get { return _groupActivityType; }
            set { SetProperty<string>(ref _groupActivityType, value); }
        }

        private string _groupActivityType = string.Empty;

        public RelatedAthletesViewModel()
        {
            RelatedActivities = new ObservableCollection<Strava.Activities.Activity>();
        }
    }
}
