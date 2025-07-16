using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES_WPF.Core.Models;
using MES_WPF.Core.Services.BasicInformation;
using MES_WPF.Core.Services.EquipmentManagement;
using MES_WPF.Core.Services.SystemManagement;
using MES_WPF.Model.BasicInformation;
using MES_WPF.Model.EquipmentManagement;
using MES_WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MES_WPF.ViewModels.EquipmentManagement
{
    public partial class MaintenanceOrderViewModel : ObservableObject
    {
        private readonly IMaintenanceOrderService _orderService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private MaintenanceOrder? _selectedOrder;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        [ObservableProperty]
        private byte _selectedOrderType = 0; // 0:全部, 1:计划维护, 2:故障维修, 3:紧急维修

        [ObservableProperty]
        private byte _selectedStatus = 0; // 0:全部, 1:待处理, 2:已分配, 3:处理中, 4:已完成, 5:已取消

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "维护工单管理";
        
        // 新增/编辑维护工单相关属性
        [ObservableProperty]
        private bool _isOrderDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private MaintenanceOrder _editingOrder = new MaintenanceOrder();
        
        [ObservableProperty]
        private ObservableCollection<Equipment> _equipments = new ObservableCollection<Equipment>();
        
        [ObservableProperty]
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        
        [ObservableProperty]
        private ObservableCollection<EquipmentMaintenancePlan> _maintenancePlans = new ObservableCollection<EquipmentMaintenancePlan>();
        
        [ObservableProperty]
        private int? _selectedEquipmentId;
        
        [ObservableProperty]
        private int? _selectedMaintenancePlanId;
        
        [ObservableProperty]
        private int? _selectedAssignedToId;
        
        public ObservableCollection<MaintenanceOrder> Orders { get; } = new();
        
        public ICollectionView? OrdersView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            OrdersView?.Refresh();
        }

        partial void OnSelectedOrderTypeChanged(byte value)
        {
            OrdersView?.Refresh();
        }

        partial void OnSelectedStatusChanged(byte value)
        {
            OrdersView?.Refresh();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            OrdersView?.Refresh();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            OrdersView?.Refresh();
        }
        
        public MaintenanceOrderViewModel(
            IMaintenanceOrderService orderService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载维护工单数据
            _ = LoadOrdersAsync();
            
            // 加载设备、用户和维护计划数据
            _ = LoadEquipmentsAsync();
            _ = LoadUsersAsync();
            _ = LoadMaintenancePlansAsync();
        }
        
        private void SetupFilter()
        {
            OrdersView = CollectionViewSource.GetDefaultView(Orders);
            if (OrdersView != null)
            {
                OrdersView.Filter = OrderFilter;
            }
        }
        
        private bool OrderFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedOrderType == 0 && SelectedStatus == 0 && !StartDate.HasValue && !EndDate.HasValue)
            {
                return true;
            }
            
            if (obj is MaintenanceOrder order)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (order.OrderCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (order.FaultDescription?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesType = SelectedOrderType == 0 || order.OrderType == SelectedOrderType;
                
                bool matchesStatus = SelectedStatus == 0 || order.Status == SelectedStatus;
                
                bool matchesDate = true;
                if (StartDate.HasValue && order.PlanStartTime < StartDate.Value)
                {
                    matchesDate = false;
                }
                if (EndDate.HasValue && order.PlanStartTime > EndDate.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesDate = false;
                }
                
                return matchesKeyword && matchesType && matchesStatus && matchesDate;
            }
            
            return false;
        }
        
        private async Task LoadOrdersAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Orders.Clear();
                
                // 获取所有维护工单
                var orders = await _orderService.GetAllAsync();
                
                // 将维护工单数据添加到集合
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
                
                TotalCount = Orders.Count;
                
                // 刷新视图
                OrdersView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护工单数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadEquipmentsAsync()
        {
            try
            {
                // 清空现有数据
                Equipments.Clear();
                
                // 从服务获取设备数据
                var equipments = await App.GetService<IEquipmentService>().GetAllAsync();
                
                // 将设备数据添加到集合
                foreach (var equipment in equipments)
                {
                    Equipments.Add(equipment);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载设备数据失败: {ex.Message}");
                
                // 加载失败时添加一些模拟设备
                Equipments.Clear();
                // 创建模拟设备数据，包含Resource导航属性
                var mockEquipment1 = new Equipment { Id = 1 };
                mockEquipment1.Resource = new Model.BasicInformation.Resource { ResourceName = "设备1", ResourceCode = "EQ001" };
                Equipments.Add(mockEquipment1);
                
                var mockEquipment2 = new Equipment { Id = 2 };
                mockEquipment2.Resource = new Model.BasicInformation.Resource { ResourceName = "设备2", ResourceCode = "EQ002" };
                Equipments.Add(mockEquipment2);
                
                var mockEquipment3 = new Equipment { Id = 3 };
                mockEquipment3.Resource = new Model.BasicInformation.Resource { ResourceName = "设备3", ResourceCode = "EQ003" };
                Equipments.Add(mockEquipment3);
            }
        }
        
        private async Task LoadUsersAsync()
        {
            try
            {
                // 清空现有数据
                Users.Clear();
                
                // 从服务获取用户数据
                var users = await App.GetService<IUserService>().GetAllAsync();
                
                // 将用户数据添加到集合
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载用户数据失败: {ex.Message}");
                
            }
        }
        
        private async Task LoadMaintenancePlansAsync()
        {
            try
            {
                // 清空现有数据
                MaintenancePlans.Clear();
                
                // 从服务获取维护计划数据
                var plans = await App.GetService<IEquipmentMaintenancePlanService>().GetAllAsync();
                
                // 将维护计划数据添加到集合
                foreach (var plan in plans)
                {
                    MaintenancePlans.Add(plan);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护计划数据失败: {ex.Message}");
                
                // 加载失败时添加一些模拟维护计划
                MaintenancePlans.Clear();
                MaintenancePlans.Add(new EquipmentMaintenancePlan { Id = 1, PlanName = "日常巡检", PlanCode = "MP001" });
                MaintenancePlans.Add(new EquipmentMaintenancePlan { Id = 2, PlanName = "设备保养", PlanCode = "MP002" });
                MaintenancePlans.Add(new EquipmentMaintenancePlan { Id = 3, PlanName = "设备维修", PlanCode = "MP003" });
            }
        }
        
        [RelayCommand]
        private async Task RefreshOrders()
        {
            await LoadOrdersAsync();
        }
        
        [RelayCommand]
        private async Task SearchOrders()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                OrdersView?.Refresh();
                
                // 重置到第一页
                CurrentPage = 1;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task ResetSearch()
        {
            SearchKeyword = string.Empty;
            SelectedOrderType = 0;
            SelectedStatus = 0;
            StartDate = null;
            EndDate = null;
            
            await SearchOrders();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的维护工单
            var selectedOrders = Orders.Where(o => o == SelectedOrder).ToList();
            
            if (selectedOrders.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的维护工单");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedOrders.Count} 个维护工单吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var order in selectedOrders)
                    {
                        await _orderService.DeleteAsync(order);
                        Orders.Remove(order);
                    }
                    
                    TotalCount = Orders.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护工单已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护工单失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportOrders()
        {
            await _dialogService.ShowInfoAsync("导出", "维护工单导出功能尚未实现");
        }
        
        [RelayCommand]
        private async Task GoToPage(int page)
        {
            if (page < 1 || page > (TotalCount + PageSize - 1) / PageSize)
            {
                return;
            }
            
            CurrentPage = page;
        }
        
        [RelayCommand]
        private void AddOrder()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingOrder = new MaintenanceOrder
            {
                Status = 1, // 默认待处理状态
                CreateTime = DateTime.Now,
                OrderType = 1, // 默认计划维护
                Priority = 5, // 默认优先级中等
                PlanStartTime = DateTime.Now.AddDays(1),
                PlanEndTime = DateTime.Now.AddDays(1).AddHours(2),
                OrderCode = GenerateNewOrderCode(),
                ReportBy = 1 // 当前登录用户ID
            };
            SelectedEquipmentId = Equipments.FirstOrDefault()?.Id;
            SelectedMaintenancePlanId = null;
            SelectedAssignedToId = null;
            
            // 打开对话框
            IsOrderDialogOpen = true;
        }
        
        private string GenerateNewOrderCode()
        {
            // 生成新的工单编码，格式：MO + 当前年月日 + 4位序号
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            int count = Orders.Count + 1;
            return $"MO{dateStr}{count:D4}";
        }
        
        [RelayCommand]
        private void EditOrder(MaintenanceOrder? order)
        {
            if (order == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建维护工单对象的副本，避免直接修改原始数据
            EditingOrder = new MaintenanceOrder
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                OrderType = order.OrderType,
                EquipmentId = order.EquipmentId,
                MaintenancePlanId = order.MaintenancePlanId,
                FaultDescription = order.FaultDescription,
                FaultCode = order.FaultCode,
                FaultLevel = order.FaultLevel,
                Priority = order.Priority,
                Status = order.Status,
                PlanStartTime = order.PlanStartTime,
                PlanEndTime = order.PlanEndTime,
                ActualStartTime = order.ActualStartTime,
                ActualEndTime = order.ActualEndTime,
                ReportBy = order.ReportBy,
                AssignedTo = order.AssignedTo,
                CreateTime = order.CreateTime,
                UpdateTime = order.UpdateTime,
                Remark = order.Remark
            };
            
            // 设置选中的设备、维护计划和分配人员
            SelectedEquipmentId = order.EquipmentId;
            SelectedMaintenancePlanId = order.MaintenancePlanId;
            SelectedAssignedToId = order.AssignedTo;
            
            // 打开对话框
            IsOrderDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteOrder(MaintenanceOrder? order)
        {
            if (order == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除维护工单\"{order.OrderCode}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _orderService.DeleteAsync(order);
                    Orders.Remove(order);
                    TotalCount = Orders.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护工单已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护工单失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsOrderDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveOrder()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingOrder.OrderCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入工单编码");
                return;
            }
            
            if (SelectedEquipmentId == null)
            {
                await _dialogService.ShowErrorAsync("错误", "请选择设备");
                return;
            }
            
            // 设置关联ID
            EditingOrder.EquipmentId = SelectedEquipmentId.Value;
            EditingOrder.MaintenancePlanId = SelectedMaintenancePlanId;
            EditingOrder.AssignedTo = SelectedAssignedToId;
            
            // 验证计划开始和结束时间
            if (EditingOrder.PlanEndTime <= EditingOrder.PlanStartTime)
            {
                await _dialogService.ShowErrorAsync("错误", "计划结束时间必须晚于计划开始时间");
                return;
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新维护工单
                    EditingOrder.UpdateTime = DateTime.Now;
                    await _orderService.UpdateAsync(EditingOrder);
                    
                    // 更新列表中的维护工单数据
                    var existingOrder = Orders.FirstOrDefault(o => o.Id == EditingOrder.Id);
                    if (existingOrder != null)
                    {
                        int index = Orders.IndexOf(existingOrder);
                        Orders[index] = EditingOrder;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "维护工单信息已更新");
                }
                else
                {
                    // 创建新维护工单
                    EditingOrder.CreateTime = DateTime.Now;
                    var newOrder = await _orderService.AddAsync(EditingOrder);
                    
                    // 添加到维护工单列表
                    Orders.Add(newOrder);
                    TotalCount = Orders.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "维护工单已创建");
                }
                
                // 关闭对话框
                IsOrderDialogOpen = false;
                
                // 刷新视图
                OrdersView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存维护工单失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task AssignOrder(MaintenanceOrder? order)
        {
            if (order == null) return;
            
            // 打开分配对话框，选择维护人员
            var userId = await ShowUserSelectorAsync("分配工单", "请选择维护人员:");
            
            if (userId.HasValue)
            {
                try
                {
                    // 调用工单服务分配工单
                    var updatedOrder = await _orderService.AssignOrderAsync(order.Id, userId.Value);
                    
                    // 更新UI中的工单数据
                    var index = Orders.IndexOf(order);
                    if (index >= 0)
                    {
                        Orders[index] = updatedOrder;
                    }
                    
                    OrdersView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "工单已分配给维护人员");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"分配工单失败: {ex.Message}");
                }
            }
        }
        
        private async Task<int?> ShowUserSelectorAsync(string title, string message)
        {
            // 简单实现，实际项目中可能需要更复杂的用户选择对话框
            //var userNames = Users.Select(u => u.RealName).ToList();
            //var selectedName = await _dialogService.ShowListAsync(title, message, userNames);
            
            //if (!string.IsNullOrEmpty(selectedName))
            //{
            //    var user = Users.FirstOrDefault(u => u.RealName == selectedName);
            //    return user?.Id;
            //}
            
            return null;
        }
        
        [RelayCommand]
        private async Task UpdateOrderStatus(MaintenanceOrder? order)
        {
            //if (order == null) return;
            
            //var statusOptions = new List<string> { "待处理", "已分配", "处理中", "已完成", "已取消" };
            //var selectedStatus = await _dialogService.ShowListAsync("更新状态", "请选择新状态:", statusOptions);
            
            //if (!string.IsNullOrEmpty(selectedStatus))
            //{
            //    try
            //    {
            //        byte newStatus = 1;
            //        switch (selectedStatus)
            //        {
            //            case "待处理": newStatus = 1; break;
            //            case "已分配": newStatus = 2; break;
            //            case "处理中": newStatus = 3; break;
            //            case "已完成": newStatus = 4; break;
            //            case "已取消": newStatus = 5; break;
            //        }
                    
            //        // 更新工单状态
            //        var updatedOrder = await _orderService.UpdateStatusAsync(order.Id, newStatus);
                    
            //        // 如果状态变为已完成，设置实际完成时间
            //        if (newStatus == 4 && !updatedOrder.ActualEndTime.HasValue)
            //        {
            //            updatedOrder.ActualEndTime = DateTime.Now;
            //            await _orderService.UpdateAsync(updatedOrder);
            //        }
                    
            //        // 更新UI中的工单数据
            //        var index = Orders.IndexOf(order);
            //        if (index >= 0)
            //        {
            //            Orders[index] = updatedOrder;
            //        }
                    
            //        OrdersView?.Refresh();
                    
            //        await _dialogService.ShowInfoAsync("成功", "工单状态已更新");
            //    }
            //    catch (Exception ex)
            //    {
            //        await _dialogService.ShowErrorAsync("错误", $"更新工单状态失败: {ex.Message}");
            //    }
            //}
        }
        
        [RelayCommand]
        private async Task StartOrder(MaintenanceOrder? order)
        {
            if (order == null) return;
            
            if (order.Status != 2) // 只有已分配的工单才能开始
            {
                await _dialogService.ShowInfoAsync("提示", "只有已分配的工单才能开始");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("开始执行", $"确定要开始执行工单\"{order.OrderCode}\"吗？");
            
            if (result)
            {
                try
                {
                    // 更新工单状态为处理中
                    var updatedOrder = await _orderService.UpdateStatusAsync(order.Id, 3);
                    
                    // 设置实际开始时间
                    updatedOrder.ActualStartTime = DateTime.Now;
                    await _orderService.UpdateAsync(updatedOrder);
                    
                    // 更新UI中的工单数据
                    var index = Orders.IndexOf(order);
                    if (index >= 0)
                    {
                        Orders[index] = updatedOrder;
                    }
                    
                    OrdersView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "工单已开始执行");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"开始执行工单失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task CompleteOrder(MaintenanceOrder? order)
        {
            if (order == null) return;
            
            if (order.Status != 3) // 只有处理中的工单才能完成
            {
                await _dialogService.ShowInfoAsync("提示", "只有处理中的工单才能完成");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("完成工单", $"确定要完成工单\"{order.OrderCode}\"吗？");
            
            if (result)
            {
                try
                {
                    // 更新工单状态为已完成
                    var updatedOrder = await _orderService.UpdateStatusAsync(order.Id, 4);
                    
                    // 设置实际结束时间
                    updatedOrder.ActualEndTime = DateTime.Now;
                    await _orderService.UpdateAsync(updatedOrder);
                    
                    // 更新UI中的工单数据
                    var index = Orders.IndexOf(order);
                    if (index >= 0)
                    {
                        Orders[index] = updatedOrder;
                    }
                    
                    OrdersView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "工单已完成");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"完成工单失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task CancelOrder(MaintenanceOrder? order)
        {
            if (order == null) return;
            
            if (order.Status == 4 || order.Status == 5) // 已完成或已取消的工单不能取消
            {
                await _dialogService.ShowInfoAsync("提示", "已完成或已取消的工单不能再次取消");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("取消工单", $"确定要取消工单\"{order.OrderCode}\"吗？");
            
            if (result)
            {
                try
                {
                    // 更新工单状态为已取消
                    var updatedOrder = await _orderService.UpdateStatusAsync(order.Id, 5);
                    
                    // 更新UI中的工单数据
                    var index = Orders.IndexOf(order);
                    if (index >= 0)
                    {
                        Orders[index] = updatedOrder;
                    }
                    
                    OrdersView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "工单已取消");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"取消工单失败: {ex.Message}");
                }
            }
        }
    }
    

}