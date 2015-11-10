﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Tuto.BatchWorks;

namespace Tuto.Model
{
    //NEED REFACTORING!

    public enum Options {BeforeEditing, DuringEditing, Skip, WithAssembly};
    public abstract class Settings
    {
        public virtual List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip, Options.WithAssembly }; } }
        public List<string> OptionsAsStrings { get {return PossibleOptions.Select ( x => x.ToString()).ToList(); }}
        public string CurrentAsString { get; set; }
        public Options CurrentOption { get {return (Options)Enum.Parse(typeof(Options), CurrentAsString);} }
    }

    public class ConversionSettings : Settings
    {
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.WithAssembly }; } }
        public ConversionSettings() { CurrentAsString = Options.DuringEditing.ToString(); }
    }

    public class ThumbSettings : Settings
    {
        public ThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
    }

    public class DesktopThumbSettings : Settings
    {
        public DesktopThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip }; } }
    }

    public class FaceThumbSettings : Settings
    {
        public FaceThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip }; } }
    }

    public class AudioCleanSettings : Settings
    {
        public AudioCleanSettings() { CurrentAsString = Options.DuringEditing.ToString(); }
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip, Options.WithAssembly }; } }
    }

    public class WorkSettings
    {
        public ConversionSettings ConversionSettings { get; set; }
        public AudioCleanSettings AudioCleanSettings { get; set; }
        public FaceThumbSettings FaceThumbSettings { get; set; }
        public DesktopThumbSettings DesktopThumbSettings { get; set; }

        public WorkSettings()
        {
            ConversionSettings = new ConversionSettings();
            AudioCleanSettings = new AudioCleanSettings();
            FaceThumbSettings = new FaceThumbSettings();
            DesktopThumbSettings = new DesktopThumbSettings();
        }

        public List<BatchWork> GetDuringWorks(EditorModel model)
        {
            var toDo = new List<BatchWork>();
            if (model.Global.WorkSettings.AudioCleanSettings.CurrentOption == Options.DuringEditing)
                toDo.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model));

            if (model.Global.WorkSettings.ConversionSettings.CurrentOption == Options.DuringEditing)
            {
                toDo.Add(new ConvertVideoWork(model, model.Locations.FaceVideo, true));
                toDo.Add(new ConvertVideoWork(model, model.Locations.DesktopVideo, false));
            }

            if (model.Global.WorkSettings.FaceThumbSettings.CurrentOption == Options.DuringEditing)
            {
                toDo.Add(new CreateThumbWork(model.Locations.FaceVideo, model));
            }

            if (model.Global.WorkSettings.DesktopThumbSettings.CurrentOption == Options.DuringEditing)
            {
                toDo.Add(new CreateThumbWork(model.Locations.DesktopVideo, model));
            }
            return toDo;
        }

        public List<BatchWork> GetBeforeEditingWorks(EditorModel model)
        {
            var works = new List<BatchWork>();
            if (model.Global.WorkSettings.FaceThumbSettings.CurrentOption == Options.BeforeEditing)
                works.Add(new CreateThumbWork(model.Locations.FaceVideo, model));

            if (model.Global.WorkSettings.DesktopThumbSettings.CurrentOption == Options.BeforeEditing)
                works.Add(new CreateThumbWork(model.Locations.DesktopVideo, model));

            if (model.Global.WorkSettings.AudioCleanSettings.CurrentOption == Options.BeforeEditing)
                works.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model));

            if (model.Global.WorkSettings.ConversionSettings.CurrentOption == Options.BeforeEditing)
            {
                works.Add(new ConvertVideoWork(model, model.Locations.DesktopVideo, true));
                works.Add(new ConvertVideoWork(model, model.Locations.FaceVideo, false));
            }
            return works;
        }
    }
}
