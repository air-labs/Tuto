﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tuto.Model;

namespace Editor
{
    public interface IEditorMode
    {
        void CheckTime();
        void MouseClick(int SelectedLocation, bool button);
        void ProcessKey(KeyboardCommandData key);

        EditorModel Model { get;  }
    }
}
