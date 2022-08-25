using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Shawn.Utils.Wpf;

namespace Shawn.Utils.WpfResources.Theme.Styles
{
    public abstract class WindowBase : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        protected bool NotifyPropertyChangedEnabled = true;

        public void SetNotifyPropertyChangedEnabled(bool isEnabled)
        {
            NotifyPropertyChangedEnabled = isEnabled;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (NotifyPropertyChangedEnabled)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetAndNotifyIfChanged<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (oldValue == null && newValue == null) return false;
            if (oldValue != null && oldValue.Equals(newValue)) return false;
            if (newValue != null && newValue.Equals(oldValue)) return false;
            oldValue = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual bool SetAndNotifyIfChanged<T>(ref T oldValue, T newValue, [CallerMemberName] string? propertyName = null)
        {
            return SetAndNotifyIfChanged(propertyName!, ref oldValue, newValue);
        }

        #endregion INotifyPropertyChanged

        #region DragMove

        protected bool _isLeftMouseDown = false;
        protected bool _isDragging = false;

        public virtual void WinTitleBar_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _isLeftMouseDown = false;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            }
            else
            {
                _isLeftMouseDown = true;
                var th = new Thread(() =>
                {
                    Thread.Sleep(50);
                    if (_isLeftMouseDown)
                    {
                        _isDragging = true;
                    }
                    _isLeftMouseDown = false;
                });
                th.Start();
            }
        }

        public Action? OnDragEnd = null;

        public virtual void WinTitleBar_OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed && _isDragging)
            {
                _isDragging = false;
                OnDragEnd?.Invoke();
                SimpleLogHelper.Debug("OnDragEnd?.Invoke();");
                return;
            }
            if (e.LeftButton != MouseButtonState.Pressed || !_isDragging)
            {
                _isLeftMouseDown = false;
                //SimpleLogHelper.Debug("_isLeftMouseDown = false");
                return;
            }

            if (this.WindowState == WindowState.Maximized)
            {
                var p = ScreenInfoEx.GetMouseVirtualPosition();
                var top = p.Y;
                var left = p.X;
                this.Top = top - 15;
                this.Left = left - this.Width / 2;
                this.WindowState = WindowState.Normal;
                this.Top = top - 15;
                this.Left = left - this.Width / 2;
            }

            try
            {
                this.DragMove();
            }
            catch
            {
                // ignored
            }
        }

        #endregion DragMove

        public virtual void WinTitleBar_OnCloseButtonDown(object s, RoutedEventArgs e)
        {
            this.Close();
        }
        public virtual void WinTitleBar_OnMaximizeButtonDown(object s, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        public virtual void WinTitleBar_OnMinimizeButtonDown(object s, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        protected WindowBase()
        {
#if DEBUG
            SimpleLogHelper.Debug($"{this.GetType().Name} init");
#endif
        }
    }
}