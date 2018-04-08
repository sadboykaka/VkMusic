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

        ImageSource pause = ImageSource.FromFile("ic_pause_grey600_36dp.png");
        ImageSource play = ImageSource.FromFile("ic_play_grey600_36dp.png");
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
            slider.Value = e;
        }

        CustomSlider slider = new CustomSlider();
        public Algh.interfaces.IPlayer player { get; private set; }

        ClickableIcon BackImage;
        ClickableIcon NextImage;
        ClickableHashIcon PlayImage;

        StackLayout CommandPanel = new StackLayout { Orientation = StackOrientation.Horizontal };

        public AudioPlayerView(Algh.interfaces.IPlayer p, View menu)
        {
            player = p;
            NextImage = new ClickableIcon(ImageSource.FromFile("ic_skip_next_grey600_36dp.png"), NextClick);
            BackImage = new ClickableIcon(ImageSource.FromFile("ic_skip_previous_grey600_36dp.png"), BackClick);
            PlayImage = new ClickableHashIcon(play, PlayPauseClick);

            Orientation = StackOrientation.Vertical;

            Children.Add(slider);

            CommandPanel.Children.Add(BackImage);
            CommandPanel.Children.Add(PlayImage);
            CommandPanel.Children.Add(NextImage);

            Children.Add(CommandPanel);
            if (menu != null) CommandPanel.Children.Add(menu);
            //------------------------------------------------
            if (player.IsPlay)
            {
                PlayEvent(player, true);
            }
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
