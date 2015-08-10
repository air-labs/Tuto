﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Navigator;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Tuto.Model;
using Tuto.BatchWorks;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        public PatchModel Model;
        public EditorModel EModel;
        public MainWindow()
        {
            InitializeComponent();
            PatchWindow.LoadedBehavior = MediaState.Manual;
        }

        public void LoadModel(PatchModel model, EditorModel em)
        {
            this.DataContext = model;
            Model = model;
            EModel = em;
            scale = model.Scale;
        }


        private int prevoiusTop = 5;
        private DispatcherTimer timer;
        private double mainVideoLength = 0;
        private double volume;
        private TrackInfo currentPatch;
        private bool isPlaying;
        private bool isLoaded;
        private int scale;

        private void Tracks_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                addTrack(files[0]);
                var processMode = EModel.Global.MovePatchOriginInsteadOfCopying;
                var name = System.IO.Path.Combine(EModel.Locations.PatchesDirectory.FullName, new FileInfo(files[0]).Name);
                if (!Directory.Exists(EModel.Locations.PatchesDirectory.FullName))
                    Directory.CreateDirectory(EModel.Locations.PatchesDirectory.FullName);
                if (!File.Exists(name))
                    Program.BatchWorkQueueWindow.Run(new List<BatchWork>() { new MoveCopyWork(files[0], name, processMode) });
            }
        }

        private void addTrack(string path)
        {
            var seconds = ViewTimeline.Position.TotalSeconds * Model.Scale;

            var track = new TrackInfo(path, Model.Scale);
            track.LeftShift = seconds;
            track.TopShift = prevoiusTop;
            track.DurationInPixels = 10;
            Model.MediaTracks.Add(track);
            PatchWindow.MediaOpened += SetPatchDuration;
            PatchWindow.Stop();
            PatchWindow.Source = null;
            PatchWindow.Source = new Uri(path);
            PatchWindow.Play(); //need to fire event to get duration
            PatchWindow.Pause();
        }

        private void doInitialLoad()
        {
            ViewTimeline.Source = new Uri(Model.SourceInfo.FullName);
            ViewTimeline.LoadedBehavior = MediaState.Manual;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (s, a) => { CheckPlayTime(); };
            ViewTimeline.MediaOpened += SetMainVideo;
            ViewTimeline.Play();
            ViewTimeline.Pause();
            isLoaded = true;
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                ViewTimeline.Pause();
                PatchWindow.Pause();
                isPlaying = false;
                return;
            }
            else if (!isLoaded)
            {
                doInitialLoad();
                return;
            }
            else { ViewTimeline.Play(); PatchWindow.Play(); isPlaying = true; }
        }

        private void SetMainVideo(object s, RoutedEventArgs a)
        {
            double length = 0;
            Model.Duration = ViewTimeline.NaturalDuration.TimeSpan.TotalSeconds;
            Model.DurationInPixels = Model.DurationInPixels;
            volume = ViewTimeline.Volume != 0 ? ViewTimeline.Volume : volume;
            timer.Start();
            ViewTimeline.MediaOpened -= SetMainVideo; //should be once
        }

        private void SetPatchDuration(object s, RoutedEventArgs a)
        {
            var track = Model.MediaTracks.Last();
            var elem = (MediaElement)s;
            var duration = elem.NaturalDuration.TimeSpan.TotalSeconds;
            track.StartSecond = 0;
            track.EndSecond = duration;
            track.DurationInSeconds = duration;
            RefreshTracks();
            PatchWindow.MediaOpened -= SetPatchDuration; //should be once
        }




        private void CheckPlayTime()
        {
            var pixelsRelativeToSeconds = ViewTimeline.Position.TotalSeconds * Model.Scale;
            Canvas.SetLeft(CurrentTime, pixelsRelativeToSeconds);
            for (var i = Model.MediaTracks.Count - 1; i >= 0; i--)
                if (InPatchSection(Model.MediaTracks[i], pixelsRelativeToSeconds))
                {
                    if (currentPatch == Model.MediaTracks[i])
                        return;

                    currentPatch = Model.MediaTracks[i];
                    PatchWindow.Source = Model.MediaTracks[i].Path;

                    var shift = currentPatch.LeftShift;
                    var position = pixelsRelativeToSeconds - shift;
                    PatchWindow.Position = TimeSpan.FromSeconds(position);
                    PatchWindow.Play();

                    ViewTimeline.Volume = 0;
                    ViewTimeline.Visibility = System.Windows.Visibility.Collapsed;

                    return;
                }
            PatchWindow.Pause();
            ViewTimeline.Volume = volume;
            ViewTimeline.Visibility = System.Windows.Visibility.Visible;
            currentPatch = null;
        }

        private bool InPatchSection(TrackInfo track, double seconds)
        {
            var leftIn = seconds >= track.LeftShift + track.StartPixel;
            var rightIn = seconds <= track.LeftShift + track.EndPixel;
            return leftIn && rightIn;
        }

        private void TimeLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Tracks);
            var span = TimeSpan.FromSeconds(pos.X / Model.Scale);
            Canvas.SetLeft(CurrentTime, pos.X);
            ViewTimeline.Position = span;
            if (currentPatch != null)
            {
                var shift = currentPatch.LeftShift;
                var seconds = ViewTimeline.Position.TotalSeconds * Model.Scale;
                var position = seconds - shift;
                PatchWindow.Position = TimeSpan.FromSeconds(position / Model.Scale);
            }
        }

        private void Patch_Click(object sender, RoutedEventArgs e)
        {
            Program.BatchWorkQueueWindow.Run(new List<BatchWork>() { new PatchWork(Model, true, EModel) });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            EModel.Save();
        }

        private void mainwindow_Loaded(object sender, RoutedEventArgs e)
        {
            doInitialLoad();
        }

        private void RangeSlider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Tracks).X;
            for (var i = 0; i < Model.MediaTracks.Count; i++)
            {
                var track = Model.MediaTracks[i];
                if (track.LeftShift + track.StartPixel <= pos && track.LeftShift + track.EndPixel >= pos)
                {
                    if (track.Path == PatchWindow.Source)
                    {
                        PatchWindow.Stop();
                        PatchWindow.Source = null;
                    }
                    Model.DeleteTrackAccordingPosition(i, EModel);
                    return;
                }
            }
        }

        private void mainwindow_Closing(object sender, CancelEventArgs e)
        {
            //EModel.Save();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newScale = (int)e.NewValue;
            if (e.OldValue != 0)
            {
                Model.Scale = newScale;
                RefreshTracks();
                SetMainVideo(null, null);
            }
        }

        private void RefreshTracks()
        {
            
            foreach (var track in Model.MediaTracks)
            {
                var oldScale = track.Scale;
                track.Scale = Model.Scale;
                track.DurationInPixels = track.DurationInPixels;
                track.StartPixel = track.StartPixel;
                track.LeftShift = track.LeftShift / oldScale * Model.Scale;
                track.EndPixel = track.EndPixel; //need for redrawing
            }
        }
    }
}
