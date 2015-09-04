﻿using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using Path = System.IO.Path;


namespace Tuto.Navigator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainNavigatorWindow : Window
    {
        public MainNavigatorWindow()
        {
            InitializeComponent();
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            var mode = BatchWithOptions.Visibility == System.Windows.Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            BatchWithOptions.Visibility = mode;
        }

    }
}
