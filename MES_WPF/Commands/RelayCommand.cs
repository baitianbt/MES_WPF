using System;
using System.Windows.Input;

namespace MES_WPF.Commands
{
    /// <summary>
    /// 实现ICommand接口的命令类
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// 创建一个始终可执行的命令
        /// </summary>
        /// <param name="execute">执行方法</param>
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        /// <summary>
        /// 创建一个始终可执行的命令（无参数）
        /// </summary>
        /// <param name="execute">执行方法</param>
        public RelayCommand(Action execute) : this(p => execute(), null) { }

        /// <summary>
        /// 创建一个命令
        /// </summary>
        /// <param name="execute">执行方法</param>
        /// <param name="canExecute">可执行条件</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 可执行状态变更事件
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// 判断命令是否可执行
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
} 