using System;
using System.ComponentModel;

namespace MES_WPF.Models
{
    /// <summary>
    /// 通知项模型
    /// </summary>
    public class NotificationItem : INotifyPropertyChanged
    {
        private string _title;
        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _time;
        /// <summary>
        /// 通知时间
        /// </summary>
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        private string _iconKind;
        /// <summary>
        /// 图标类型
        /// </summary>
        public string IconKind
        {
            get => _iconKind;
            set
            {
                _iconKind = value;
                OnPropertyChanged(nameof(IconKind));
            }
        }

        private string _iconBackground;
        /// <summary>
        /// 图标背景色
        /// </summary>
        public string IconBackground
        {
            get => _iconBackground;
            set
            {
                _iconBackground = value;
                OnPropertyChanged(nameof(IconBackground));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 