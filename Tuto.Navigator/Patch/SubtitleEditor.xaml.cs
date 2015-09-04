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
using System.Windows.Shapes;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for SubtitleEditor.xaml
    /// </summary>
    public partial class SubtitleEditor : Window
    {
        public SubtitleEditor()
        {
            InitializeComponent();
            var aviableColors = new string[] { "Black", "White", "Yellow", "Red", "Green" };
            ForegroundPicker.ItemsSource = aviableColors;
            StrokePicker.ItemsSource = aviableColors;
        }
    }
}
