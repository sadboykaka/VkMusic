using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace VkMusic2.Views
{
    class AudioListViewWithCheckBoxes : ListView
    {
        private bool CheckBoxes;

        private DataTemplate DataTemplateWithCheckBoxes = new DataTemplateAudio(true);
        private DataTemplate DataTemplateWithoutCheckBoxes = new DataTemplateAudio(false);

        public AudioListViewWithCheckBoxes(ObservableCollection<Algh.interfaces.IAudio> Tracks)
        {
            setCheckBoxes(false);
            ItemsSource = Tracks;
            HasUnevenRows = true;
           
        }

        public bool isCheckBoxesSet()
        {
            return CheckBoxes;
        }

        public List<Algh.interfaces.IAudio> GetSelectedItems()
        {
            if (!CheckBoxes || ItemsSource == null ) return null;
            var a = ((ITemplatedItemsView<Cell>)this).TemplatedItems;
            List<Algh.interfaces.IAudio> delt = new List<Algh.interfaces.IAudio>();
            foreach (ViewCell icell in a)
            {
                var track = (Algh.interfaces.IAudio)icell.BindingContext;
                var checkbox = ((Messier16.Forms.Controls.Checkbox)((StackLayout)icell.View).Children[0]);
                if (checkbox.Checked) delt.Add(track);
            }
            return delt;
        }

        public void setCheckBoxes(bool f)
        {
            CheckBoxes = f;
            if (CheckBoxes) ItemTemplate = DataTemplateWithCheckBoxes;
            else ItemTemplate = DataTemplateWithoutCheckBoxes;
        }

    }
}
