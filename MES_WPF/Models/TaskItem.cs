using System;
using System.ComponentModel;

namespace MES_WPF.Models
{
    /// <summary>
    /// 任务项模型
    /// </summary>
    public class TaskItem : INotifyPropertyChanged
    {
        private string _taskName;
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        private string _planDate;
        /// <summary>
        /// 计划日期
        /// </summary>
        public string PlanDate
        {
            get => _planDate;
            set
            {
                _planDate = value;
                OnPropertyChanged(nameof(PlanDate));
            }
        }

        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        public string Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        private string _priorityColor;
        /// <summary>
        /// 优先级颜色
        /// </summary>
        public string PriorityColor
        {
            get => _priorityColor;
            set
            {
                _priorityColor = value;
                OnPropertyChanged(nameof(PriorityColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 