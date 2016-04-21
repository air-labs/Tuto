﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tuto.BatchWorks
{
    public class StatusConverter : IValueConverter
    {
        public Dictionary<BatchWorkStatus, string> colors = new Dictionary<BatchWorkStatus, string>()
        {
            {BatchWorkStatus.Running, "White" },
            {BatchWorkStatus.Aborted, "Red" },
            {BatchWorkStatus.Cancelled, "LightGray" },
            {BatchWorkStatus.Pending, "Yellow" },
            {BatchWorkStatus.Failure, "Red" },
            {BatchWorkStatus.Success, "White" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return colors[(BatchWorkStatus)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}