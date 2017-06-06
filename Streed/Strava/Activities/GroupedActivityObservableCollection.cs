using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Activities
{
    public class GroupedActivityObservableCollection : ActivityObservableCollection
    {
        public GroupedActivityObservableCollection(string title, DateTime titleDateTime, Activity activity)
            : base()
        {
            Title = title;
            TitleDateTime = titleDateTime;
            Add(activity);
        }

        public GroupedActivityObservableCollection(string title, DateTime titleDateTime, IEnumerable<Activity> collection)
            : base(collection)
        {
            Title = title;
            TitleDateTime = titleDateTime;
        }


        public GroupedActivityObservableCollection(string title, DateTime titleDateTime, List<Activity> list)
            : base(list)
        {
            Title = title;
            TitleDateTime = titleDateTime;
        }

        public DateTime TitleDateTime { get; private set; }
        public string Title { get; private set; }
        public bool HasItems { get { return (this.Count() > 0); } }
    }
}
