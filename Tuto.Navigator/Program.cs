﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Tuto.Model;
using Editor;
using System.Threading;
using Tuto.BatchWorks;
using Tuto.Publishing.Youtube;
using Tuto.Navigator.ViewModels;
using Tuto.Navigator.Views;

namespace Tuto.Navigator
{
    public static class Program
    {
        static Videotheque videotheque;

		static bool Backdoor()
		{
			return false;
			foreach (var e in videotheque.EditorModels)
				if (e.Montage.Information != null)
				{
					bool found = false;
					foreach (var i in e.Montage.Information.Episodes)
						if (i.OutputType == OutputTypes.Patch)
						{
							found = true;
							i.OutputType = OutputTypes.Output;
						}
					if (found)
						e.Save();
				}
			return true;
		}

        [STAThread]
        public static void Main(string[] args)
        {
            //  NewMain(); return;
           

            string fname = null;
            if (args.Length > 0) fname = args[0];
            var application = new Application();
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;                 
            var wnd = new Tuto.Init.MainWindow();
            Func<Videotheque> start = () => Videotheque.Load(fname, wnd, false);
            var token = start.BeginInvoke(null, null);
            wnd.ShowDialog();
            videotheque = start.EndInvoke(token);
			if (videotheque == null)
			{
				MessageBox.Show("Cannot initialize Tuto");
				return;
			}

			if (Backdoor()) return;


            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //YoutubeApisProcessor.Initialize(videotheque.TempFolder);

            var mainWindow = new MainNavigatorWindow();
            WorkQueue = new WorkQueue(videotheque.Data.WorkSettings);
            WorkQueue.Dispatcher = mainWindow.Dispatcher;
            var globalModel = new MainModel(videotheque);
            globalModel.VideothequeModel.FillQueue();

            mainWindow.DataContext = globalModel;
            mainWindow.WindowState = System.Windows.WindowState.Maximized;

            if (args.Length > 1)
                mainWindow.RequestedModel = args[1];

			string directoryName = args[0];
			if (File.Exists(args[0]))
			{
				directoryName = new FileInfo(args[0]).Directory.FullName;
			}

            application.ShutdownMode = ShutdownMode.OnMainWindowClose;    
            application.Run(mainWindow);
            application.Shutdown();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            StringBuilder message=new StringBuilder();
            message.AppendLine(DateTime.Now.ToString());
            message.AppendLine();
            while(exception!=null)
            {
                message.AppendLine(exception.GetType().FullName);
                message.AppendLine(exception.Message);
                message.AppendLine(exception.StackTrace);
                message.AppendLine();
                exception=exception.InnerException;
            }

            File.WriteAllText(
                System.IO.Path.Combine(videotheque.VideothequeSettingsFile.Directory.FullName, "error.txt"),
                message.ToString());

            MessageBox.Show("An error has occured. Please send the error.txt to the developer.", "Tuto", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public static string MontageFile="montage.editor";
        public static string TimesFile="times.txt";
        public static WorkQueue WorkQueue { get; set; }


        

    }
}
