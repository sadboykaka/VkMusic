using System;
using System.Collections.Generic;
using System.Linq;
using Algh.interfaces;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;

namespace Algh
{
    class SearchHelper
    {
        ObservableCollection<IAudio> Tracks;



        CancellationTokenSource cancelTokenSource;
        CancellationToken token;

        public EventHandler<ObservableCollection<IAudio>> SearchEvent;

        public SearchHelper(ObservableCollection<IAudio> tracks)
        {
            Tracks = tracks;
        }

        Task SearchTask;

        private string TrackMask(IAudio Track)
        {
            return (Track.Author + " " + Track.Name).ToLower();
        }

        private void TaskPr(string val)
        {
           var a = Tracks.Where(x => (x.Author + " " + x.Name).Contains(val));
            ObservableCollection<IAudio> res = new ObservableCollection<IAudio>();
            foreach (IAudio Track in Tracks)
            {
                if (token.IsCancellationRequested) return;
                if (TrackMask(Track).Contains(val)) res.Add(Track);
            }
            SearchEvent(this, res);
        }

        public void Search(string value)
        {
            if (SearchTask != null && !SearchTask.IsCompleted)
            {
                cancelTokenSource.Cancel();
            }
            if(value == "")
            {
                SearchEvent(this, Tracks);
                return;
            }
        
            SearchTask = new Task(() => TaskPr(value));
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            SearchTask.Start();
        
        }
    }



}
