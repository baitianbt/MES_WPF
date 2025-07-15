using MES_WPF.Core.Services.BasicInformation;
using MES_WPF.Model.BasicInformation;
using MES_WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MES_WPF.ViewModels.BasicInformation
{
    /// <summary>
    /// 工序管理视图模型
    /// </summary>
    public partial class OperationViewModel : ObservableObject
    {
        private readonly IOperationService _operationService;
        private readonly IDialogService _dialogService;

        // 扩展Operation类以添加IsSelected属性
        public partial class OperationWithSelection : Operation
        {
            public bool IsSelected { get; set; }
        }

        #region 属性

        [ObservableProperty]
        private ObservableCollection<Operation> _operations = new();

        public ObservableCollection<Operation> OperationsView => _operations;

        [ObservableProperty]
        private Operation _selectedOperation;

        [ObservableProperty]
        private Operation _editingOperation;

        [ObservableProperty]
        private string _searchKeyword;

        [ObservableProperty]
        private int _selectedOperationType = 0;

        [ObservableProperty]
        private int _selectedStatus = 0;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isOperationDialogOpen;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _currentPage = 1;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public OperationViewModel(IOperationService operationService, IDialogService dialogService)
        {
            _operationService = operationService;
            _dialogService = dialogService;

            // 加载工序数据
            Task.Run(async () => await LoadOperationsAsync());
        }

        // 可执行条件方法
        private bool CanPreviousPage() => CurrentPage > 1;
        private bool CanNextPage() => CurrentPage < TotalPages;

        /// <summary>
        /// 加载工序数据
        /// </summary>
        [RelayCommand]
        private async Task LoadOperationsAsync()
        {
            try
            {
                IsRefreshing = true;
                var operations = await _operationService.GetAllAsync();
                TotalCount = operations.Count();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Operations.Clear();
                    foreach (var operation in operations)
                    {
                        Operations.Add(operation);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载工序数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// 搜索工序
        /// </summary>
        [RelayCommand]
        private async Task SearchOperationsAsync()
        {
            try
            {
                IsRefreshing = true;

                var operations = await _operationService.GetAllAsync();

                // 按关键字筛选
                if (!string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    operations = operations.Where(o => 
                        o.OperationCode.Contains(SearchKeyword) || 
                        o.OperationName.Contains(SearchKeyword));
                }

                // 按工序类型筛选
                if (SelectedOperationType > 0)
                {
                    byte operationType = (byte)SelectedOperationType;
                    operations = operations.Where(o => o.OperationType == operationType);
                }

                // 按状态筛选
                if (SelectedStatus > 0)
                {
                    bool isActive = SelectedStatus == 1;
                    operations = operations.Where(o => o.IsActive == isActive);
                }

                TotalCount = operations.Count();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Operations.Clear();
                    foreach (var operation in operations)
                    {
                        Operations.Add(operation);
                    }
                });

                CurrentPage = 1;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索工序失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// 重置搜索条件
        /// </summary>
        [RelayCommand]
        private void ResetSearch()
        {
            SearchKeyword = string.Empty;
            SelectedOperationType = 0;
            SelectedStatus = 0;
            
            Task.Run(async () => await LoadOperationsAsync());
        }

        /// <summary>
        /// 添加工序
        /// </summary>
        [RelayCommand]
        private void AddOperation()
        {
            EditingOperation = new Operation
            {
                CreateTime = DateTime.Now,
                IsActive = true,
                StandardTime = 0
            };
            
            IsEditMode = false;
            IsOperationDialogOpen = true;
        }

        /// <summary>
        /// 编辑工序
        /// </summary>
        [RelayCommand]
        private void EditOperation(Operation operation)
        {
            if (operation == null) return;

            // 创建编辑对象的副本，以免直接修改原对象
            EditingOperation = new Operation
            {
                Id = operation.Id,
                OperationCode = operation.OperationCode,
                OperationName = operation.OperationName,
                OperationType = operation.OperationType,
                Department = operation.Department,
                Description = operation.Description,
                StandardTime = operation.StandardTime,
                IsActive = operation.IsActive,
                CreateTime = operation.CreateTime,
                UpdateTime = DateTime.Now
            };
            
            IsEditMode = true;
            IsOperationDialogOpen = true;
        }

        /// <summary>
        /// 删除工序
        /// </summary>
        [RelayCommand]
        private async Task DeleteOperationAsync(Operation operation)
        {
            if (operation == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除工序 {operation.OperationName} 吗？");
            if (result)
            {
                try
                {
                    await _operationService.DeleteByIdAsync(operation.Id);
                    Operations.Remove(operation);
                    TotalCount--;
                    await _dialogService.ShowInfoAsync("成功", "工序删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除工序失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 批量删除工序
        /// </summary>
        [RelayCommand]
        private async Task BatchDeleteAsync()
        {
            // 注：这里应该使用OperationWithSelection类型，但当前数据模型不支持，需要调整模型设计
            // 临时解决方案，跳过此方法
            await _dialogService.ShowInfoAsync("提示", "批量删除功能需要模型支持，暂不可用");
            return;
            
            /*
            var selectedOperations = Operations.Where(o => (o as OperationWithSelection)?.IsSelected ?? false).ToList();
            if (selectedOperations.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请选择要删除的工序");
                return;
            }

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除选中的 {selectedOperations.Count} 个工序吗？");
            if (result)
            {
                try
                {
                    foreach (var operation in selectedOperations)
                    {
                        await _operationService.DeleteByIdAsync(operation.Id);
                        Operations.Remove(operation);
                    }
                    TotalCount -= selectedOperations.Count;
                    await _dialogService.ShowInfoAsync("成功", "工序批量删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"批量删除工序失败: {ex.Message}");
                }
            }
            */
        }

        /// <summary>
        /// 导出工序数据
        /// </summary>
        [RelayCommand]
        private void ExportOperations()
        {
            _dialogService.ShowInfoAsync("提示", "导出功能待实现");
        }

        /// <summary>
        /// 保存工序
        /// </summary>
        [RelayCommand]
        private async Task SaveOperationAsync()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingOperation.OperationCode))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入工序编码");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingOperation.OperationName))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入工序名称");
                return;
            }

            if (EditingOperation.StandardTime <= 0)
            {
                await _dialogService.ShowInfoAsync("提示", "标准工时必须大于0");
                return;
            }

            try
            {
                // 检查工序编码是否已存在
                if (!IsEditMode)
                {
                    bool exists = await _operationService.IsOperationCodeExistsAsync(EditingOperation.OperationCode);
                    if (exists)
                    {
                        await _dialogService.ShowInfoAsync("提示", $"工序编码 {EditingOperation.OperationCode} 已存在");
                        return;
                    }
                }

                if (IsEditMode)
                {
                    // 更新
                    EditingOperation.UpdateTime = DateTime.Now;
                    await _operationService.UpdateAsync(EditingOperation);

                    // 更新列表中的对象
                    var existingOperation = Operations.FirstOrDefault(o => o.Id == EditingOperation.Id);
                    if (existingOperation != null)
                    {
                        var index = Operations.IndexOf(existingOperation);
                        Operations[index] = EditingOperation;
                    }

                    await _dialogService.ShowInfoAsync("成功", "工序更新成功");
                }
                else
                {
                    // 新增
                    EditingOperation.CreateTime = DateTime.Now;
                    var newOperation = await _operationService.AddAsync(EditingOperation);
                    Operations.Add(newOperation);
                    TotalCount++;

                    await _dialogService.ShowInfoAsync("成功", "工序添加成功");
                }

                IsOperationDialogOpen = false;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存工序失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        [RelayCommand]
        private void CancelEdit()
        {
            IsOperationDialogOpen = false;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanPreviousPage))]
        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                // 实现分页逻辑
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanNextPage))]
        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                // 实现分页逻辑
            }
        }

        /// <summary>
        /// 跳转到指定页
        /// </summary>
        [RelayCommand]
        private void GoToPage(object pageObj)
        {
            if (pageObj is int pageInt)
            {
                if (pageInt >= 1 && pageInt <= TotalPages)
                {
                    CurrentPage = pageInt;
                    // 实现分页逻辑
                }
            }
            else if (pageObj is string pageStr)
            {
                if (int.TryParse(pageStr, out int pageVal) && pageVal >= 1 && pageVal <= TotalPages)
                {
                    CurrentPage = pageVal;
                    // 实现分页逻辑
                }
            }
        }

        private int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        private int PageSize => 10; // 每页显示数量
    }
} 