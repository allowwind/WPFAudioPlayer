using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFAudioPlayer.LIstenBack;

namespace WPFAudioPlayer
{
    public partial class ListenBacker : UserControl
    {
       
        /// <summary>
        /// mp3播放功能
        /// </summary>
        MP3PlayerHelper m_mp3Helper = new MP3PlayerHelper(); 
        DispatcherTimer timer;
        public static readonly RoutedEvent OnTickSelectedEvent = EventManager.RegisterRoutedEvent("OnTickSelectedEvent", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(ListenBacker));
        public event RoutedEventHandler OnTickSelected
        {
            add { AddHandler(OnTickSelectedEvent, value); }
            remove { RemoveHandler(OnTickSelectedEvent, value); }
        }
        void FireOnTickSelected()
        {
            RoutedEventArgs eventArgs = new RoutedEventArgs(ListenBacker.OnTickSelectedEvent, this);
            this.RaiseEvent(eventArgs);
        }



        public static readonly RoutedEvent OnMarkChangedEvent = EventManager.RegisterRoutedEvent("OnMarkChangedEvent", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(ListenBacker));
        public event RoutedEventHandler OnMarkChanged
        {
            add { AddHandler(OnMarkChangedEvent, value); }
            remove { RemoveHandler(OnMarkChangedEvent, value); }
        }
        void FireOnMarkChanged()
        {
            RoutedEventArgs eventArgs = new RoutedEventArgs(ListenBacker.OnMarkChangedEvent, this);
            this.RaiseEvent(eventArgs);
        }

        public ListenBacker()
        {
            InitializeComponent();
            this.SizeChanged += ListenBacker_SizeChanged;
            m_mp3Helper.OnPlayStop += M_mp3Helper_OnPlayStop; 
             
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void M_mp3Helper_OnPlayStop()
        {
            IsPlay = false;
            Tick = 0;
            FireOnMarkChanged();

        }

        private bool isPlay;

        public bool IsPlay
        {
            get { return isPlay; }
            set
            {
                isPlay = value;
                if (value == false)
                    this.img.Source = new BitmapImage(new Uri("pack://application:,,,/WPFAudioPlayer;component/play_32x32.png"));
                else {
                    this.img.Source = new BitmapImage(new Uri("pack://application:,,,/WPFAudioPlayer;component/pause_32x32.png"));
                }
            }
        }
         
        private void Timer_Tick(object sender, EventArgs e)
        {
            //获取进度,显示
            if (IsPlay == true)
            {
                var tick = m_mp3Helper.GetPositionInSeconds();
                if ((int)Tick != (int)tick)
                {
                    FireOnMarkChanged();
                }
                Tick = tick;
            }
        }

        public double MinTick
        {
            get { return (double)GetValue(MinTickProperty); }
            set { SetValue(MinTickProperty, value); }
        }
        public static readonly DependencyProperty MinTickProperty =
            DependencyProperty.Register("MinTick", typeof(double), typeof(ListenBacker), new PropertyMetadata(default(double)));


        public double MaxTick
        {
            get { return (double)GetValue(MaxTickProperty); }
            set { SetValue(MaxTickProperty, value); }
        }

        public static readonly DependencyProperty MaxTickProperty =
            DependencyProperty.Register("MaxTick", typeof(double), typeof(ListenBacker), new PropertyMetadata(default(double), OnMaxTickChange));

        private static void OnMaxTickChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (d as ListenBacker);
            var t1 = TimeSpan.FromSeconds(ctrl.MaxTick);
            if (t1 > TimeSpan.FromDays(1))
            {
                ctrl.txtRight.Text = $"{t1.TotalDays.ToString("D2")}天-{t1.Hours.ToString("D2")}:{t1.Minutes.ToString("D2")}:{t1.Seconds.ToString("D2")}";
            }
            else
            {
                ctrl.txtRight.Text = $"{t1.Hours.ToString("D2")}:{t1.Minutes.ToString("D2")}:{t1.Seconds.ToString("D2")}";
            }


        }

        private static void OnTickChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (d as ListenBacker);
            {
                var t1 = TimeSpan.FromSeconds(ctrl.Tick);
                if (t1 > TimeSpan.FromDays(1))
                {
                    ctrl.txtLeft.Text = $"{t1.TotalDays}天-{t1.Hours.ToString("D2")}:{t1.Minutes.ToString("D2")}:{t1.Seconds.ToString("D2")}";
                }
                else
                {
                    ctrl.txtLeft.Text = $"{t1.Hours.ToString("D2")}:{t1.Minutes.ToString("D2")}:{t1.Seconds.ToString("D2")}";
                }
            }
            if (ctrl.isDraging == false)
            {
                ctrl.slider.Value = ctrl.Tick;
            }

        }

        public double Tick
        {
            get { return (double)GetValue(TickProperty); }
            set { SetValue(TickProperty, value); }
        }

        public static readonly DependencyProperty TickProperty =
            DependencyProperty.Register("Tick", typeof(double), typeof(ListenBacker), new PropertyMetadata(0d, OnTickChange));


        public static readonly RoutedEvent OnSelectTickEvent = EventManager.RegisterRoutedEvent("OnSelectTickEvent", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(ListenBacker));
        public event RoutedEventHandler OnSelectTick
        {
            add { AddHandler(OnSelectTickEvent, value); }
            remove { RemoveHandler(OnSelectTickEvent, value); }
        }
        void FireOnSelectTick()
        {
            RoutedEventArgs eventArgs = new RoutedEventArgs(ListenBacker.OnSelectTickEvent, this);
            this.RaiseEvent(eventArgs);
            Debug.Print($"{Tick}-OnSelectTickEvent");
        }

        private void ListenBacker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReLocationMarks();
        }


        void ReLocationMarks()
        {
            foreach (var item in roogCanvas.Children)
            {
                if (item is MarkTicker ticker)
                {

                    ticker.Width = roogCanvas.ActualHeight / 2;
                    ticker.Height = roogCanvas.ActualHeight * 2 / 3;
                    //根据值和长款算出this
                    var startX = (ticker.Tick - MinTick) * 1.0 / (MaxTick - MinTick) * this.ActualWidth;
                    Canvas.SetLeft(ticker, startX - ticker.Width / 2);
                }
            }
        }

        Dictionary<long, MarkTicker> dicThumbs = new Dictionary<long, MarkTicker>();


        public void Mark(long val)
        {
            if (dicThumbs.ContainsKey(val))
                return;
            //if (val < MinTick | val > MaxTick)
            //    return;
            //if (MaxTick <= MinTick)
            //    return;
            MarkTicker ticker = new MarkTicker(val)
            {
                Width = roogCanvas.ActualHeight / 2,
                Height = roogCanvas.ActualHeight * 2 / 3,
                Fill = Brushes.Red,
                ToolTip = $"{val}",
                Cursor = Cursors.Hand
            };
            ticker.Click += Ticker_Click;
            //根据值和长款算出this
            var startX = (val - MinTick) * 1.0 / (MaxTick - MinTick) * roogCanvas.ActualWidth;
            roogCanvas.Children.Add(ticker);
            Canvas.SetLeft(ticker, startX - ticker.Width / 2);
            Canvas.SetTop(ticker, 0);
            dicThumbs.Add(val, ticker);

        }

        private void Ticker_Click(object sender, RoutedEventArgs e)
        {
            SetMarkValue((sender as MarkTicker).Tick);
            FireOnSelectTick();
        }

        public void RemoveMark(long val)
        {
            if (dicThumbs.ContainsKey(val))
            {
                if (roogCanvas.Children.Count > 0)
                    roogCanvas.Children.Remove(dicThumbs[val]);
                dicThumbs.Remove(val);
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print($"{e.NewValue}");
        }

        private void MySlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDraging = false;
            SetMarkValue(slider.Value);
            Debug.Print($"{Tick}");
            FireOnTickSelected();

        }


        #region 播放音频相关
        public void SetMarkValue(double val)
        {
            m_mp3Helper.SetValue(val, true);
            IsPlay = true;
        }

        bool isDraging = false;
        public void PlayMp3Url(string mp3File)
        {
            try
            {
                if (string.IsNullOrEmpty(mp3File)) return;
                MinTick = 0;
                m_mp3Helper.InitURL(mp3File);
                MaxTick = (long)m_mp3Helper.AudioLength;
                ReLocationMarks();
            }
            catch (Exception ex)
            {
                 
            }
        }

        public void Stop()
        {
            m_mp3Helper.Stop();
        }

        #endregion

        private void slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDraging = true;
        }

        private void BtnClick_Stop(object sender, RoutedEventArgs e)
        {
            m_mp3Helper.SetValue(0);
            Tick = 0;
            IsPlay = false;

        }

        private void BtnClick_Play(object sender, RoutedEventArgs e)
        {
            if (IsPlay == false)
            {
                m_mp3Helper.SetValue(Tick, true);
                
            }
            else
            {
                m_mp3Helper.SetValue(Tick, false);
               
            }
            IsPlay = !IsPlay;
        }
    }
}
