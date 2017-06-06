using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Activities
{
    public class ActivityObservableCollection : ObservableCollection<Activity>
    {
        public ActivityObservableCollection()
            : base()
        { }

        public ActivityObservableCollection(IEnumerable<Activity> collection)
            : base(collection)
        { }

        public ActivityObservableCollection(List<Activity> list)
            : base(list)
        { }

        public void AddRange(IEnumerable<Activity> collection)
        {
            collection.ToList().ForEach(c => Add(c));
        }

        public void ReplaceItem(Activity oldItem, Activity newItem)
        {
            var index = this.IndexOf(oldItem);
            if (index != -1)
                this.SetItem(index, newItem);
        }
    }
}
