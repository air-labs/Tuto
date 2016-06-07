﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Model;

namespace Tuto.Navigator.Editor
{
    /// <summary>
    /// Interaction logic for VideoPatchPanel.xaml
    /// </summary>
    public partial class VideoPatchPanel : UserControl
    {
        public VideoPatchPanel()
        {
            InitializeComponent();
            overlay.Items.Add(VideoPatchOverlayType.Replace);
            overlay.Items.Add(VideoPatchOverlayType.KeepSoundAddSilence); ;
            overlay.Items.Add(VideoPatchOverlayType.KeepSoundTruncateVideo);

        }
    }
}
