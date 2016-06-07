﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class ConvertDesktopWork : ConvertVideoWork
    {
        public ConvertDesktopWork(EditorModel model, bool forced)
        {
            Model = model;
            Name = "Converting Desktop video: " + model.RawLocation.Name;
            Source = Model.Locations.DesktopVideo;
            Forced = forced;
        }

        public override bool Finished()
        {
            return Model.Locations.ConvertedDesktopVideo.Exists;
        }

        public override void Clean()
        {
            FinishProcess();
            TryToDelete(TempFile.FullName);
        }
    }
}
