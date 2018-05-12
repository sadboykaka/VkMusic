using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VkMusic2.Views
{
    class AudioPlayerView : StackLayout
    {
        CustomSlider slider = new CustomSlider();
        public Algh.interfaces.IPlayer player { get; private set; }

        ClickableIcon BackImage;
        ClickableIcon NextImage;
        ClickableHashIcon PlayImage;
        ClickableHashIcon RepeatImage;

        StackLayout CommandPanel = new StackLayout { Orientation = StackOrientation.Horizontal };

        ImageSource pause = ImageSource.FromFile("ic_pause_grey600_36dp.png");
        ImageSource play = ImageSource.FromFile("ic_play_grey600_36dp.png");
        ImageSource next = ImageSource.FromFile("ic_skip_next_grey600_36dp.png");
        ImageSource prev = ImageSource.FromFile("ic_skip_previous_grey600_36dp.png");
        ImageSource repeaton = ImageSource.FromFile("ic_repeat_grey600_36dp.png");
        ImageSource repeatoff = ImageSource.FromFile("ic_repeat_off_grey600_36dp.png");
        void BackClick(object sender, EventArgs e)
        {
            player.PlayPrev();
        }

        void NextClick(object sender, EventArgs e)
        {
            player.PlayNext();
        }

        void PlayPauseClick(object sender, EventArgs e)
        {
            player.PauseOrResume();
        }

        void PlayEvent(object sender, bool e)
        {
            if (e)
            {
                slider.Maximum = ((Algh.interfaces.IPlayer)sender).Duration;
                if(PlayImage.Source != pause) PlayImage.Source = pause; 
            }
            else
            {
                if(PlayImage.Source != play) PlayImage.Source = play;
            }
        }

        void SeekChange(object sender, int e)
        {
            player.Seek = e;
        }

        void SeekEvent(object sender, int e)
        {
            if(e == -1)
            {
                RepeatImage.Source = repeatoff;
                return;
            }
            if(e == -2)
            {
                RepeatImage.Source = repeaton;
                return;
            }
            slider.Value = e;
        }

        public AudioPlayerView(Algh.interfaces.IPlayer p, View menu)
        {
            player = p;
            NextImage = new ClickableIcon(next, NextClick);
            BackImage = new ClickableIcon(prev, BackClick);
            PlayImage = new ClickableHashIcon(play, PlayPauseClick);
            RepeatImage = new ClickableHashIcon(repeaton, (i, e) => { player.Repeat = !player.Repeat; }  );

            Orientation = StackOrientation.Vertical;

            Children.Add(slider);

            CommandPanel.Children.Add(BackImage);
            CommandPanel.Children.Add(PlayImage);
            CommandPanel.Children.Add(NextImage);
            CommandPanel.Children.Add(RepeatImage);

            Children.Add(CommandPanel);
            if (menu != null) CommandPanel.Children.Add(menu);
            //------------------------------------------------
            if (player.IsPlay) PlayEvent(player, true);
            if (player.Repeat) RepeatImage.Source = repeatoff;
            //------------------------------------------------
            player.PlayEvent += PlayEvent;
            player.SeekEvent += SeekEvent;
            slider.ValueChangeByUser += SeekChange;

        }

        ~AudioPlayerView()
        {
            player.PlayEvent -= PlayEvent;
            player.SeekEvent -= SeekEvent;
            slider.ValueChangeByUser -= SeekChange;
        }

    }
}
