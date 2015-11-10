﻿//using Editor;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using Tuto.Model;

//namespace Tuto.TutoServices
//{
//	public class PraatService : Service
//	{
//		const string SilentLabel = "--";
//		const string SoundLabel = "++";

//		const double MinPitch = 100;
//		const double TimeStep = 0;

//		const double SilenceThreshold = -27;
//		const double MinSilentInterval = 0.4;
//		const double MinSoundInterval = 0.1;

//		public override string Name
//		{
//			get { return "praat"; }
//		}

//		public bool Forced
//		{
//			get;
//			set;
//		}

//		public override string Description
//		{
//			get { return DescriptionString; }
//		}

//		public override string Help
//		{
//			get { return HelpString; }
//		}

//		public override void DoWork(string[] args)
//		{
//			var folder = args[1];

//			var model = EditorModelIO.Load(folder);
//			DoWork(model);
//			EditorModelIO.Save(model);
//		}

//		public void DoWork(EditorModel model)
//		{
//			// model.Locations.PraatVoice.Delete();
//			model.Locations.PraatOutput.Delete();

//			if (!model.Locations.PraatVoice.Exists || Forced)
//				Shell.FFMPEG(false, "-i \"{0}\" -vn -q:a 0 \"{1}\"", model.Locations.FaceVideo, model.Locations.PraatVoice);

			
//				Shell.Exec(false, model.Locations.PraatExecutable,
//					String.Format(
//						CultureInfo.InvariantCulture,
//						"\"{0}\" \"{1}\" \"{2}\" {3} {4} {5} {6} {7} {8} {9}",
//						model.Locations.PraatScriptSource,
//						model.Locations.PraatVoice,
//						model.Locations.PraatOutput,
//						SilentLabel,
//						SoundLabel,
//						MinPitch,
//						TimeStep,
//						SilenceThreshold,
//						MinSilentInterval,
//						MinSoundInterval));
			

//			model.Montage.SoundIntervals.Clear();
//			using (var reader = new StreamReader(model.Locations.PraatOutput.FullName))
//			{

//				for (var i = 0; i < 11; i++)
//					reader.ReadLine();

//				var intervalCount = int.Parse(reader.ReadLine());
//				for (int i = 0; i < intervalCount; i++)
//				{
//					var startTime = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
//					var endTime = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
//					var hasVoice = reader.ReadLine() == '"' + SoundLabel + '"';
//					model.Montage.SoundIntervals.Add(
//						new SoundInterval(
//							(int)Math.Round(startTime*1000), 
//							(int)Math.Round(1000*endTime), 
//							hasVoice));
//				}
//			}

            
//			//  model.Locations.PraatVoice.Delete();
//			model.Locations.PraatOutput.Delete();

//			if (WorkFinished != null) WorkFinished();
//		}

//		public event Action WorkFinished;

//		const string DescriptionString =
//@"Analyzes input video and searches intervals of silence and speech.";
//		const string HelpString =
//@"<folder>
//
//folder: directory containing video";
//	}
//}
