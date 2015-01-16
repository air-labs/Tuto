﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;
using Tuto.Navigator;

namespace Tuto.Publishing
{
	public class YoutubeLectureCommands : NotifierModel, ICommandBlockModel
	{
		readonly LectureWrap Wrap;
		public YoutubeLectureCommands(LectureWrap wrap)
		{
			Wrap = wrap;
			InitializeDueNames();
			Commands = new List<VisualCommand>();
			Commands.Add(new VisualCommand(new RelayCommand(CmGo), "view.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmPush), "upload.png"));
		}


		public List<VisualCommand> Commands { get; private set; }

		public Uri ImageSource
		{
			get { return new Uri("/Img/youtube.png", UriKind.Relative); }
		}

		IEnumerable<YoutubeVideoCommands> VideoBlocks { get { return Wrap.ChildCommandBlocks<YoutubeVideoCommands>(); } }

		public Brush Status
		{
			get
			{
				if (VideoBlocks.Any(z => z.Status == Brushes.Red)) return Brushes.Red;
				if (VideoBlocks.Any(z => z.Status == Brushes.Yellow)) return Brushes.Yellow;
				if (YoutubePlaylist == null) return Brushes.Yellow;
				if (YoutubePlaylist.PlaylistTitle != dueTitle) return Brushes.Yellow;
				return Brushes.Green;
			}
		}

		
		string dueTitle;

		void InitializeDueNames()
		{
			var prefix = "";
			prefix += StaticItems.GlobalData.CourseAbbreviation;
			var path = Wrap.PathFromRoot.Skip(1).ToArray();
			for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
			{

				TopicLevel level = new TopicLevel();
				if (levelNumber < StaticItems.GlobalData.TopicLevels.Count)
					level = StaticItems.GlobalData.TopicLevels[levelNumber];
				prefix += "-";
				prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic + 1);
			}

			dueTitle = prefix + " " + Wrap.Topic.Caption;
		}


		public YoutubePlaylist YoutubePlaylist
		{
			get { return Wrap.Get<YoutubePlaylist>(); }
		}

	

		public void CmPush()
		{
			var playlist = YoutubePlaylist;
			if (playlist == null)
				playlist = StaticItems.YoutubeProcessor.CreatePlaylist(Wrap.Topic.Caption);
			StaticItems.YoutubeProcessor.FillPlaylist(playlist, VideoBlocks.Select(z => z.YoutubeClip).Where(z => z != null));

			foreach (var e in VideoBlocks)
				e.CmPush();

			Wrap.Store<YoutubePlaylist>(playlist);
			this.NotifyByExpression(z => z.Status);

		}

		public void CmGo()
		{
			if (YoutubePlaylist == null) return;
			Process.Start("https://www.youtube.com/playlist?list=" + YoutubePlaylist.PlaylistId);
		}


	}
}