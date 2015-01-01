﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.Youtube.Views;
using YoutubeApiTest;

namespace Tuto.Publishing.Youtube
{
    public static class TutoPublishingYoutubeProgram
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var directory = EditorModelIO.SubstituteDebugDirectories(args[0]);
            
            
            var folder=new DirectoryInfo(directory);
            var model = new MainViewModel(folder, new YoutubeApisProcessor());

            //var clips = new List<ClipData>();


            //try
            //{
            //    clips = youtubeProcessor.LoadVideos();
            //}
            //catch
            //{
            //    MessageBox.Show("Loading video from Youtube failed.");
            //}
            
            //var match = Algorithms.MatchVideos(globalData.VideoData, youtubeData.Videos, clips);
            //var root = Algorithms.CreateTree(globalData.TopicsRoot, match, globalData.TopicLevels);

            //var playlists = youtubeProcessor.FindPlaylists();
            //Algorithms.AddPlaylists(root, youtubeData.Topics, playlists);

            //var model = new MainViewModel(folder, globalData, youtubeData.Settings, root, youtubeProcessor, match);
            var window = new MainWindow();
            window.DataContext = model;
            new Application().Run(window);
        }
    }
}
