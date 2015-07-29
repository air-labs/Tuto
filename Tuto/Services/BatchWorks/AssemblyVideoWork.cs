﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using Tuto.TutoServices;
using Tuto.TutoServices.Montager;
using Tuto.TutoServices.Assembler;

namespace Tuto.BatchWorks
{
    public class AssemblyVideoWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades {get; set;}
        public AssemblyVideoWork (EditorModel model, bool fadeMode)
        {
            Model = model;
            Name = "Assembly Course: " + model.Locations.FaceVideo.Directory.Name;
            filesToDelIfAborted = new List<string>();
            if (!model.Locations.ConvertedDesktopVideo.Exists)
                BeforeWorks.Add(new ConvertDesktopVideoWork(model));
            if (!model.Locations.ConvertedFaceVideo.Exists)
                BeforeWorks.Add(new ConvertFaceVideoWork(model));
            this.crossFades = fadeMode;
        }

        public override void Work()
        {
            var serv = new AssemblerService(crossFades);
            var episodes = serv.GetEpisodesNodes(Model);
            var episodeNumber = 0;
            var count = episodes.Count;

            foreach (var episode in episodes)
            {
                var args = @"-i ""{0}"" -q:v 0 ""{1}""";
                var avsContext = new AvsContext();
                episode.SerializeToContext(avsContext);
                var avsScript = avsContext.Serialize(Model);
                var avsFile = Model.Locations.GetAvsStriptFile(episodeNumber);

                File.WriteAllText(avsFile.FullName, avsScript);

                var videoFile = Model.Locations.GetOutputFile(episodeNumber);
                if (videoFile.Exists) videoFile.Delete();

                args = string.Format(args, avsFile.FullName, videoFile.FullName);
                filesToDelIfAborted.Add(videoFile.FullName);
                episodeNumber++;
                RunProcess(args, Model.Locations.FFmpegExecutable.FullName);
                OnTaskFinished();
            }
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (filesToDelIfAborted.Count > 0)
                try
                {
                    File.Delete(filesToDelIfAborted[0]);
                    filesToDelIfAborted.RemoveAt(0);
                }
                catch { }
        }
    }
}