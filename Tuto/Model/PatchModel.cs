﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows;

namespace Tuto.Model
{


    [DataContract]
    public class PatchWindowState : NotifierModel
    {

        public event EventHandler PausedChanged;
        [DataMember]
        bool paused;
        public bool Paused
        {
            get { return paused; }
            set
            {
                if (paused != value)
                {
                    paused = value;
                    NotifyPropertyChanged();
                    if (PausedChanged != null) PausedChanged(this, EventArgs.Empty);
                }
            }
        }

        [DataMember]
        public bool DragInProgress;

        [DataMember]
        public double TimeSet;

        [DataMember]
        public Point LastPoint;

        [DataMember]
        public int Top = 5;

        [DataMember]
        public double volume { get; set; }

        [DataMember]
        public MediaTrack currentPatch;

        [DataMember]
        private Subtitle _currentSubtitle;


        [DataMember]
        public Subtitle currentSubtitle
        {
            get { return _currentSubtitle; }
            set { _currentSubtitle = value; NotifyPropertyChanged("currentSubtitle"); }
        }
        [DataMember]
        public bool isPlaying;
        [DataMember]
        public bool isLoaded;
    }

    [DataContract]
    public class PatchModel : NotifierModel
    {
        [DataMember]
        public ObservableCollection<MediaTrack> MediaTracks { get; set; }

        [DataMember]
        public ObservableCollection<Subtitle> Subtitles { get; set; }


        [DataMember]
        public FileInfo SourceInfo { get; set; }

        [DataMember]
        private double duration;


        public double Duration
        {
            get { return duration; }
            set { duration = value; NotifyPropertyChanged(); NotifierModelExtensions.NotifyByExpression(this, x => x.DurationInPixels); }
        }


        public double DurationInPixels
        {
            get { return duration * Scale; }
            set { duration = value / Scale; NotifyPropertyChanged(); }
        }

        [DataMember]
        public double FontCoefficent { get { return 2.18803418; } set { } }

        [DataMember]
        private double workspaceWidth;

        public double WorkspaceWidth
        {
            get { return workspaceWidth; }
            set { workspaceWidth = value; NotifyPropertyChanged(); }
        }


        [DataMember]
        public ScaleInfo ScaleInfo; //to model

        [DataMember]
        public PatchWindowState WindowState { get; set; }

        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double ActualWidth { get; set; }
        [DataMember]
        public double ActualHeight { get; set; }

        [DataMember]
        public int EpisodeNumber { get; set; }

        public int Scale
        {
            get { return ScaleInfo.Scale; }
            set
            {
                ScaleInfo.Scale = value;
                NotifyPropertyChanged();
            }
        } //pixels per sec

        public PatchModel(string sourcePath, int episodeNumber)
        {
            SourceInfo = new FileInfo(sourcePath);
            MediaTracks = new ObservableCollection<MediaTrack>();
            Subtitles = new ObservableCollection<Subtitle>();
            Duration = 10;
            ScaleInfo = new ScaleInfo(1);
            WindowState = new PatchWindowState();
            EpisodeNumber = episodeNumber;
        }

        public void DeleteTrackAccordingPosition(int index, EditorModel m)
        {
            var trackName = MediaTracks[index].FullName.Name;
            MediaTracks.RemoveAt(index);
            var name = System.IO.Path.Combine(m.Locations.TemporalDirectory.FullName, trackName);
            if (File.Exists(name))
                try { File.Delete(name); }
                catch { }
        }

        public void RefreshReferences()
        {
            if (MediaTracks == null)
                MediaTracks = new ObservableCollection<MediaTrack>();

            if (Subtitles == null)
                Subtitles = new ObservableCollection<Subtitle>();

            foreach (var e in MediaTracks)
            {
                e.ScaleInfo = ScaleInfo;
                PropertyChanged += (s, a) => e.NotifyScaleChanged();
            }
            foreach (var e in Subtitles)
            {
                e.ScaleInfo = ScaleInfo;
                PropertyChanged += (s, a) => e.NotifyScaleChanged();
            }
        }

    }

    [DataContract]
    public class ScaleInfo
    {
        [DataMember]
        public int Scale { get; set; }

        public ScaleInfo(int scale)
        {
            Scale = scale;
        }
    }

    [DataContract]
    public class MediaTrack : TrackInfo
    {

        [DataMember]
        public bool IsTutoPatch;

        [DataMember]
        public string ModelHash;

        [DataMember]
        public int EpisodeNumber;

        public MediaTrack(string path, ScaleInfo scale, bool isTutoPatch)
        {
            Path = new Uri(path);
            ScaleInfo = scale;
            IsTutoPatch = isTutoPatch;
        }
    }

    [DataContract]
    public class Subtitle : TrackInfo
    {
        [DataMember]
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; NotifyPropertyChanged(); }
        }

        [DataMember]
        public double HeightShift { get; set; }

        [DataMember]
        private int fontsize { get; set; }
        public int FontSize
        {
            get { return fontsize; }
            set { fontsize = value; NotifyPropertyChanged(); }
        }

        [DataMember]
        public Point Pos { get; set; }

        [DataMember]
        private double x;
        [DataMember]
        private double y;


        public double X { get { return x; } set { x = value; NotifierModelExtensions.NotifyByExpression(this, z => z.X); } }

        public double Y { get { return y; } set { y = value; NotifierModelExtensions.NotifyByExpression(this, z => z.Y); } }

        [DataMember]
        private string foreground;
        public string Foreground
        {
            get { return foreground; }
            set { foreground = value; NotifyPropertyChanged(); }
        }

        [DataMember]
        private string stroke;
        public string Stroke
        {
            get { return stroke; }
            set { stroke = value; NotifyPropertyChanged(); }
        }

        public Subtitle(string content, ScaleInfo scale, double leftShift)
        {
            StartSecond = 0;
            EndSecond = 50;
            DurationInSeconds = 500;
            LeftShiftInSeconds = leftShift;
            Content = content;
            ScaleInfo = scale;
            FontSize = 32;
            X = 100;
            Y = 100;
            Foreground = "White";
            Stroke = "Black";
        }
    }

    [DataContract]
    public abstract class TrackInfo : NotifierModel
    {
        [DataMember]
        public ScaleInfo ScaleInfo;


        public int Scale
        {
            get { return ScaleInfo.Scale; }
            set { ScaleInfo.Scale = value; }
        }

        [DataMember]
        public Uri Path { get; set; }

        public FileInfo FullName { get { return new FileInfo(Path.LocalPath); } }

        [DataMember]
        private double startSecond;


        public double StartSecond
        {
            get { return startSecond; }
            set { startSecond = value; NotifyPropertyChanged(); NotifierModelExtensions.NotifyByExpression(this, x => x.StartPixel); }
        } //left border of chunk


        public double StartPixel
        {
            get { return StartSecond * Scale; }
            set { StartSecond = value / Scale; NotifyPropertyChanged(); }
        }

        [DataMember]
        private double endSecond { get; set; }


        public double EndSecond
        {
            get { return endSecond; }
            set { endSecond = value; NotifyPropertyChanged(); NotifierModelExtensions.NotifyByExpression(this, x => x.EndPixel); }
        } //right border of chunk


        public double EndPixel
        {
            get { return EndSecond * Scale; }
            set { EndSecond = value / Scale; NotifyPropertyChanged(); }
        }

        [DataMember]
        private double durationInSeconds { get; set; }


        public double DurationInSeconds
        {
            get { return durationInSeconds; }
            set { durationInSeconds = value; NotifyPropertyChanged(); NotifierModelExtensions.NotifyByExpression(this, x => x.DurationInPixels); }
        }


        public double DurationInPixels
        {
            get { return DurationInSeconds * Scale; }
            set { DurationInSeconds = value / Scale; NotifyPropertyChanged(); }
        }

        [DataMember]
        private double leftShiftInSeconds { get; set; }


        public double LeftShiftInSeconds
        {
            get { return leftShiftInSeconds; }
            set { leftShiftInSeconds = value; NotifyPropertyChanged(); NotifierModelExtensions.NotifyByExpression(this, x => x.LeftShiftInPixels); }
        }//position of whole track relative to main track


        public double LeftShiftInPixels
        {
            get { return leftShiftInSeconds * Scale; }
            set { leftShiftInSeconds = value / Scale; NotifyPropertyChanged(); }
        }

        [DataMember]
        public double TopShift { get; set; }


        public void NotifyScaleChanged()
        {
            NotifierModelExtensions.NotifyByExpression(this, x => x.DurationInPixels);
            NotifierModelExtensions.NotifyByExpression(this, x => x.StartPixel);
            NotifierModelExtensions.NotifyByExpression(this, x => x.EndPixel);
            NotifierModelExtensions.NotifyByExpression(this, x => x.LeftShiftInPixels);
            NotifierModelExtensions.NotifyByExpression(this, x => x.Scale);
        }

    }
}
