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
    public partial class ProcessRouteViewModel : ObservableObject
    {
        private readonly IProcessRouteService _processRouteService;
        private readonly IProductService _productService;
        private readonly IRouteStepService _routeStepService;
        private readonly IOperationService _operationService;
        private readonly IDialogService _dialogService;

        // 工艺路线列表
        [ObservableProperty]
        private ObservableCollection<ProcessRoute> _processRoutes = new();

        // 产品列表（用于下拉选择）
        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        // 工序列表（用于工艺步骤选择）
        [ObservableProperty]
        private ObservableCollection<Operation> _operations = new();

        // 选中的工艺路线
        [ObservableProperty]
        private ProcessRoute _selectedProcessRoute;

        partial void OnSelectedProcessRouteChanged(ProcessRoute value)
        {
            if (value != null)
            {
                Task.Run(() => LoadRouteStepsAsync(value.Id));
            }
        }

        // 工艺路线步骤列表
        [ObservableProperty]
        private ObservableCollection<RouteStep> _routeSteps = new();

        // 选中的工艺步骤
        [ObservableProperty]
        private RouteStep _selectedRouteStep;

        // 新建/编辑的工艺路线
        [ObservableProperty]
        private ProcessRoute _editingProcessRoute;

        // 新建/编辑的工艺步骤
        [ObservableProperty]
        private RouteStep _editingRouteStep;

        // 搜索条件
        [ObservableProperty]
        private string _searchText;

        // 状态筛选
        [ObservableProperty]
        private byte _statusFilter;

        partial void OnStatusFilterChanged(byte value)
        {
            Task.Run(() => LoadProcessRoutesAsync());
        }

        // 选中的产品ID（筛选）
        [ObservableProperty]
        private int _selectedProductId;

        partial void OnSelectedProductIdChanged(int value)
        {
            if (value > 0)
            {
                Task.Run(() => LoadProcessRoutesByProductAsync(value));
            }
        }

        // 是否编辑模式
        [ObservableProperty]
        private bool _isEditing;

        // 是否编辑工艺步骤
        [ObservableProperty]
        private bool _isEditingStep;

        // 是否忙碌
        [ObservableProperty]
        private bool _isBusy;

        public ProcessRouteViewModel(
            IProcessRouteService processRouteService,
            IProductService productService,
            IRouteStepService routeStepService,
            IOperationService operationService,
            IDialogService dialogService)
        {
            _processRouteService = processRouteService;
            _productService = productService;
            _routeStepService = routeStepService;
            _operationService = operationService;
            _dialogService = dialogService;

            // 初始化时加载数据
            Task.Run(async () =>
            {
                await LoadProductsAsync();
                await LoadOperationsAsync();
                await LoadProcessRoutesAsync();
            });
        }

        // 加载工艺路线列表
        [RelayCommand]
        private async Task LoadProcessRoutesAsync()
        {
            try
            {
                IsBusy = true;
                var routes = StatusFilter > 0 
                    ? await _processRouteService.GetByStatusAsync(StatusFilter)
                    : await _processRouteService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProcessRoutes.Clear();
                    foreach (var route in routes)
                    {
                        ProcessRoutes.Add(route);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载工艺路线失败：{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 根据产品ID加载工艺路线
        private async Task LoadProcessRoutesByProductAsync(int productId)
        {
            try
            {
                IsBusy = true;
                var routes = await _processRouteService.GetByProductIdAsync(productId);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProcessRoutes.Clear();
                    foreach (var route in routes)
                    {
                        ProcessRoutes.Add(route);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载工艺路线失败：{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 加载产品列表
        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _productService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载产品信息失败：{ex.Message}");
            }
        }

        // 加载工序列表
        private async Task LoadOperationsAsync()
        {
            try
            {
                var operations = await _operationService.GetAllAsync();

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
                await _dialogService.ShowErrorAsync("错误", $"加载工序信息失败：{ex.Message}");
            }
        }

        // 加载工艺步骤
        private async Task LoadRouteStepsAsync(int routeId)
        {
            try
            {
                var steps = await _routeStepService.GetByRouteIdAsync(routeId);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    RouteSteps.Clear();
                    foreach (var step in steps.OrderBy(s => s.StepNo))
                    {
                        RouteSteps.Add(step);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载工艺步骤失败：{ex.Message}");
            }
        }

        // 搜索工艺路线
        [RelayCommand]
        private async Task SearchProcessRoutesAsync()
        {
            try
            {
                IsBusy = true;
                var routes = await _processRouteService.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    routes = routes.Where(r => 
                        r.RouteCode?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                        r.RouteName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                        r.Version?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                        r.Product?.ProductName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    );
                }

                if (StatusFilter > 0)
                {
                    routes = routes.Where(r => r.Status == StatusFilter);
                }

                if (SelectedProductId > 0)
                {
                    routes = routes.Where(r => r.ProductId == SelectedProductId);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProcessRoutes.Clear();
                    foreach (var route in routes)
                    {
                        ProcessRoutes.Add(route);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索工艺路线失败：{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 添加工艺路线
        [RelayCommand]
        private void AddProcessRoute()
        {
            EditingProcessRoute = new ProcessRoute
            {
                Status = 1, // 草稿状态
                IsDefault = false,
                CreateTime = DateTime.Now,
                Version = "1.0"
            };
            IsEditing = true;
        }

        // 编辑工艺路线
        [RelayCommand]
        private void EditProcessRoute()
        {
            if (SelectedProcessRoute == null) return;

            // 创建副本以便取消编辑时恢复
            EditingProcessRoute = new ProcessRoute
            {
                Id = SelectedProcessRoute.Id,
                RouteCode = SelectedProcessRoute.RouteCode,
                RouteName = SelectedProcessRoute.RouteName,
                ProductId = SelectedProcessRoute.ProductId,
                Version = SelectedProcessRoute.Version,
                Status = SelectedProcessRoute.Status,
                IsDefault = SelectedProcessRoute.IsDefault,
                CreateTime = SelectedProcessRoute.CreateTime,
                UpdateTime = DateTime.Now
            };
            IsEditing = true;
        }

        // 保存工艺路线
        [RelayCommand]
        private async Task SaveProcessRouteAsync()
        {
            if (EditingProcessRoute == null) return;

            try
            {
                // 验证必填字段
                if (string.IsNullOrWhiteSpace(EditingProcessRoute.RouteCode))
                {
                    await _dialogService.ShowErrorAsync("验证错误", "工艺路线编码不能为空");
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingProcessRoute.RouteName))
                {
                    await _dialogService.ShowErrorAsync("验证错误", "工艺路线名称不能为空");
                    return;
                }

                if (EditingProcessRoute.ProductId <= 0)
                {
                    await _dialogService.ShowErrorAsync("验证错误", "请选择关联产品");
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingProcessRoute.Version))
                {
                    await _dialogService.ShowErrorAsync("验证错误", "版本号不能为空");
                    return;
                }

                // 检查编码是否已存在（如果不是编辑现有工艺路线）
                if (EditingProcessRoute.Id == 0 &&
                    await _processRouteService.IsRouteCodeExistsAsync(EditingProcessRoute.RouteCode))
                {
                    await _dialogService.ShowErrorAsync("验证错误", "该工艺路线编码已存在");
                    return;
                }

                // 保存
                ProcessRoute savedRoute;
                if (EditingProcessRoute.Id == 0)
                {
                    EditingProcessRoute.CreateTime = DateTime.Now;
                    savedRoute = await _processRouteService.AddAsync(EditingProcessRoute);
                    await _dialogService.ShowInfoAsync("成功", "工艺路线添加成功");
                }
                else
                {
                    EditingProcessRoute.UpdateTime = DateTime.Now;
                    savedRoute = await _processRouteService.UpdateAsync(EditingProcessRoute);
                    await _dialogService.ShowInfoAsync("成功", "工艺路线更新成功");
                }

                // 刷新列表
                await LoadProcessRoutesAsync();
                
                // 选中保存的工艺路线
                SelectedProcessRoute = ProcessRoutes.FirstOrDefault(r => r.Id == savedRoute.Id);
                
                IsEditing = false;
                EditingProcessRoute = null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存工艺路线失败：{ex.Message}");
            }
        }

        // 删除工艺路线
        [RelayCommand]
        private async Task DeleteProcessRouteAsync()
        {
            if (SelectedProcessRoute == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", "确定要删除此工艺路线吗？此操作将同时删除所有相关的工艺步骤。");
            if (!result) return;

            try
            {
                await _processRouteService.DeleteByIdAsync(SelectedProcessRoute.Id);
                await _dialogService.ShowInfoAsync("成功", "工艺路线删除成功");
                await LoadProcessRoutesAsync();
                SelectedProcessRoute = null;
                RouteSteps.Clear();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除工艺路线失败：{ex.Message}");
            }
        }

        // 设置默认工艺路线
        [RelayCommand]
        private async Task SetDefaultRouteAsync()
        {
            if (SelectedProcessRoute == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", "确定要将此工艺路线设为默认吗？这将取消当前产品的其他默认工艺路线。");
            if (!result) return;

            try
            {
                await _processRouteService.SetDefaultRouteAsync(SelectedProcessRoute.Id, SelectedProcessRoute.ProductId);
                await _dialogService.ShowInfoAsync("成功", "已将此工艺路线设为默认");
                await LoadProcessRoutesAsync();
                SelectedProcessRoute = ProcessRoutes.FirstOrDefault(r => r.Id == SelectedProcessRoute.Id);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"设置默认工艺路线失败：{ex.Message}");
            }
        }

        // 更新工艺路线状态
        [RelayCommand]
        private async Task UpdateStatusAsync(byte status)
        {
            if (SelectedProcessRoute == null) return;

            string statusText = status switch
            {
                1 => "草稿",
                2 => "审核中",
                3 => "已发布",
                4 => "已作废",
                _ => "未知状态"
            };

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要将此工艺路线状态更改为\"{statusText}\"吗？");
            if (!result) return;

            try
            {
                await _processRouteService.UpdateStatusAsync(SelectedProcessRoute.Id, status);
                await _dialogService.ShowInfoAsync("成功", "工艺路线状态更新成功");
                await LoadProcessRoutesAsync();
                SelectedProcessRoute = ProcessRoutes.FirstOrDefault(r => r.Id == SelectedProcessRoute.Id);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"更新工艺路线状态失败：{ex.Message}");
            }
        }

        // 取消编辑
        [RelayCommand]
        private void CancelEdit()
        {
            EditingProcessRoute = null;
            IsEditing = false;
        }

        #region 工艺步骤管理

        // 添加工艺步骤
        [RelayCommand]
        private void AddRouteStep()
        {
            if (SelectedProcessRoute == null) return;

            int nextOrder = 10;
            if (RouteSteps.Any())
            {
                nextOrder = RouteSteps.Max(s => s.StepNo) + 10;
            }

            EditingRouteStep = new RouteStep
            {
                RouteId = SelectedProcessRoute.Id,
                StepNo = nextOrder,
                IsKeyOperation = true,
                IsQualityCheckPoint = false,
                SetupTime = 0,
                ProcessTime = 0,
                WaitTime = 0
            };
            IsEditingStep = true;
        }

        // 编辑工艺步骤
        [RelayCommand]
        private void EditRouteStep()
        {
            if (SelectedRouteStep == null) return;

            // 创建副本以便取消编辑时恢复
            EditingRouteStep = new RouteStep
            {
                Id = SelectedRouteStep.Id,
                RouteId = SelectedRouteStep.RouteId,
                StepNo = SelectedRouteStep.StepNo,
                OperationId = SelectedRouteStep.OperationId,
                Description = SelectedRouteStep.Description,
                IsKeyOperation = SelectedRouteStep.IsKeyOperation,
                IsQualityCheckPoint = SelectedRouteStep.IsQualityCheckPoint,
                SetupTime = SelectedRouteStep.SetupTime,
                ProcessTime = SelectedRouteStep.ProcessTime,
                WaitTime = SelectedRouteStep.WaitTime,
                WorkstationTypeId = SelectedRouteStep.WorkstationTypeId
            };
            IsEditingStep = true;
        }

        // 保存工艺步骤
        [RelayCommand]
        private async Task SaveRouteStepAsync()
        {
            if (EditingRouteStep == null) return;

            try
            {
                // 验证必填字段
                if (EditingRouteStep.OperationId <= 0)
                {
                    await _dialogService.ShowErrorAsync("验证错误", "请选择工序");
                    return;
                }

                // 保存
                RouteStep savedStep;
                if (EditingRouteStep.Id == 0)
                {
                    savedStep = await _routeStepService.AddAsync(EditingRouteStep);
                    await _dialogService.ShowInfoAsync("成功", "工艺步骤添加成功");
                }
                else
                {
                    savedStep = await _routeStepService.UpdateAsync(EditingRouteStep);
                    await _dialogService.ShowInfoAsync("成功", "工艺步骤更新成功");
                }

                // 刷新列表
                await LoadRouteStepsAsync(SelectedProcessRoute.Id);
                
                // 选中保存的步骤
                SelectedRouteStep = RouteSteps.FirstOrDefault(s => s.Id == savedStep.Id);
                
                IsEditingStep = false;
                EditingRouteStep = null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存工艺步骤失败：{ex.Message}");
            }
        }

        // 删除工艺步骤
        [RelayCommand]
        private async Task DeleteRouteStepAsync()
        {
            if (SelectedRouteStep == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", "确定要删除此工艺步骤吗？");
            if (!result) return;

            try
            {
                await _routeStepService.DeleteByIdAsync(SelectedRouteStep.Id);
                await _dialogService.ShowInfoAsync("成功", "工艺步骤删除成功");
                await LoadRouteStepsAsync(SelectedProcessRoute.Id);
                SelectedRouteStep = null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除工艺步骤失败：{ex.Message}");
            }
        }

        // 上移工艺步骤
        [RelayCommand(CanExecute = nameof(CanMoveStepUp))]
        private async Task MoveStepUpAsync()
        {
            if (SelectedRouteStep == null || !CanMoveStepUp()) return;

            try
            {
                var currentIndex = RouteSteps.IndexOf(SelectedRouteStep);
                var previousStep = RouteSteps[currentIndex - 1];

                // 交换步骤顺序
                int tempOrder = SelectedRouteStep.StepNo;
                SelectedRouteStep.StepNo = previousStep.StepNo;
                previousStep.StepNo = tempOrder;

                // 更新数据库
                await _routeStepService.UpdateAsync(SelectedRouteStep);
                await _routeStepService.UpdateAsync(previousStep);

                // 重新加载工艺步骤
                await LoadRouteStepsAsync(SelectedProcessRoute.Id);

                // 选中当前步骤
                SelectedRouteStep = RouteSteps.FirstOrDefault(s => s.Id == SelectedRouteStep.Id);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动工艺步骤失败：{ex.Message}");
            }
        }

        // 下移工艺步骤
        [RelayCommand(CanExecute = nameof(CanMoveStepDown))]
        private async Task MoveStepDownAsync()
        {
            if (SelectedRouteStep == null || !CanMoveStepDown()) return;

            try
            {
                var currentIndex = RouteSteps.IndexOf(SelectedRouteStep);
                var nextStep = RouteSteps[currentIndex + 1];

                // 交换步骤顺序
                int tempOrder = SelectedRouteStep.StepNo;
                SelectedRouteStep.StepNo = nextStep.StepNo;
                nextStep.StepNo = tempOrder;

                // 更新数据库
                await _routeStepService.UpdateAsync(SelectedRouteStep);
                await _routeStepService.UpdateAsync(nextStep);

                // 重新加载工艺步骤
                await LoadRouteStepsAsync(SelectedProcessRoute.Id);

                // 选中当前步骤
                SelectedRouteStep = RouteSteps.FirstOrDefault(s => s.Id == SelectedRouteStep.Id);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动工艺步骤失败：{ex.Message}");
            }
        }

        // 判断是否可以上移步骤
        private bool CanMoveStepUp()
        {
            if (SelectedRouteStep == null || RouteSteps.Count <= 1) return false;
            int currentIndex = RouteSteps.IndexOf(SelectedRouteStep);
            return currentIndex > 0;
        }

        // 判断是否可以下移步骤
        private bool CanMoveStepDown()
        {
            if (SelectedRouteStep == null || RouteSteps.Count <= 1) return false;
            int currentIndex = RouteSteps.IndexOf(SelectedRouteStep);
            return currentIndex < RouteSteps.Count - 1;
        }

        // 取消编辑工艺步骤
        [RelayCommand]
        private void CancelEditStep()
        {
            EditingRouteStep = null;
            IsEditingStep = false;
        }

        #endregion
    }
}