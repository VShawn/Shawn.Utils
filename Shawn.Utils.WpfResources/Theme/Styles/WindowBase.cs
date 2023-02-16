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

        protected bool _isDragging = false;
        private bool _doubleClickEnd = false;
        private Point _mousePosition = new Point(-1, -1);
        public virtual void WinTitleBar_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _doubleClickEnd = false;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                SimpleLogHelper.Debug("e.LeftButton != MouseButtonState.Pressed");
                return;
            }

            if (e.ClickCount == 2)
            {
                _doubleClickEnd = true;
                _isDragging = false;
                this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
                _mousePosition = new Point(-1, -1);
            }
            else
            {
                _mousePosition = e.GetPosition(this);
            }
        }

        public Action? OnDragEnd = null;

        public virtual void WinTitleBar_OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed
                || _doubleClickEnd)
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    OnDragEnd?.Invoke();
                    SimpleLogHelper.Debug("OnDragEnd?.Invoke();");
                }
                return;
            }

            var cmp = e.GetPosition(this);
            if (Math.Abs(cmp.X - _mousePosition.X) > 2 || Math.Abs(cmp.Y - _mousePosition.Y) > 2)
            {
                _isDragging = true;
            }


            if (_isDragging)
            {
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
        }

        #endregion DragMove

        public bool IsClosing { get; private set; } = false;
        public bool IsClosed { get; private set; } = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            IsClosing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosing = true;
            IsClosed = true;
        }

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