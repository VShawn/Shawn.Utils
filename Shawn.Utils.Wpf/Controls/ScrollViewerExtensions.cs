﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shawn.Utils.Wpf.Controls
{
    /***
     * 
                    <ScrollViewer utils:ScrollViewerExtensions.IsHorizontalScrollOnWheelEnabled="true"
     */

    /// <summary>
    /// thanks to https://stackoverflow.com/questions/25366784/wpf-scrollviewer-mousewheel-not-working-with-stackpanel/25375433#25375433
    /// </summary>
    public class ScrollViewerExtensions : DependencyObject
    {
        public static bool GetIsHorizontalScrollOnWheelEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHorizontalScrollOnWheelEnabledProperty);
        }

        public static void SetIsHorizontalScrollOnWheelEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHorizontalScrollOnWheelEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsHorizontalScrollOnWheelEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHorizontalScrollOnWheelEnabledProperty =
            DependencyProperty.RegisterAttached("IsHorizontalScrollOnWheelEnabled", typeof(bool), typeof(ScrollViewerExtensions), new PropertyMetadata(false, OnIsHorizontalScrollOnWheelEnabledChanged));

        private static void OnIsHorizontalScrollOnWheelEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer sv)
                if ((bool)e.NewValue)
                    sv.PreviewMouseWheel += sv_PreviewMouseWheel;
                else
                    sv.PreviewMouseWheel -= sv_PreviewMouseWheel;
        }

        private static void sv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
                if (e.Delta > 0)
                    scrollViewer.LineLeft();
                else
                    scrollViewer.LineRight();
            e.Handled = true;
        }
    }
}