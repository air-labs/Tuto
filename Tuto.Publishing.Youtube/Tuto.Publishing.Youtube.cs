﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.Youtube.Views;

namespace Tuto.Publishing.Youtube
{
    public static class Tuto
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var directory = EditorModelIO.SubstituteDebugDirectories(args[0]);
            var passwordFile = new FileInfo(Path.Combine(directory, "password"));
            string password="";
            if (passwordFile.Exists)
                password = File.ReadAllText(passwordFile.FullName);
            else
                password = PasswordWindow.GetPassword();
            
            
            var folder=new DirectoryInfo(directory);
            var globalData = EditorModelIO.ReadGlobalData(folder);
            var youtubeData = PublishingFileContainer.Load(folder);
            var youtubeProcessor = new YoutubeProcessor(youtubeData.Settings, password);
            var clips = new List<ClipData>();
            try
            {
                clips = youtubeProcessor.Load();
            }
            catch
            {
                MessageBox.Show("Loading video from Youtube failed.");
            }
            
            var match = Algorithms.MatchVideos(globalData.VideoData, youtubeData.Videos, clips);
            var root = Algorithms.CreateTree(globalData.TopicsRoot, match, globalData.TopicLevels);

           


            var model = new MainViewModel(folder, globalData, youtubeData.Settings, root, youtubeProcessor);
            var window = new MainWindow();
            window.DataContext = model;
            new Application().Run(window);
        }
    }
}
