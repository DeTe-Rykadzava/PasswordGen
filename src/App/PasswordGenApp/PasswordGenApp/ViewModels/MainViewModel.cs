using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Database.Context;
using Database.Core;
using Database.Repositories;
using Database.Repositories.Interfaces;
using PasswordGenApp.Services.ClipboardService;
using PasswordGenApp.Services.CoreService;
using PasswordGenApp.Services.DialogService;
using PasswordGenApp.Services.NavigationService;
using PasswordGenApp.Services.PassowordService;
using PasswordGenApp.Services.UserKeyStorageService;
using PasswordGenApp.ViewModels.Core;
using PasswordGenApp.ViewModels.Password;
using PasswordGenApp.ViewModels.Password.Factory;
using PasswordGenApp.Views.Password;
using PasswordGenApp.Views.User;
using ReactiveUI;
using Splat;

namespace PasswordGenApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    // services
    private INavigationService? _navigationService = null;
    public INavigationService? NavigationService
    {
        get => _navigationService;
        private set => this.RaiseAndSetIfChanged(ref _navigationService, value);
    }
    
    private IUserKeyStorageService? _userKeyStorage = null;
    public IUserKeyStorageService? UserKeyStorage
    {
        get => _userKeyStorage;
        private set => this.RaiseAndSetIfChanged(ref _userKeyStorage, value);
    }

    // fields
    
    private string _status = String.Empty;
    public string Status
    {
        get => _status;
        private set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    // interactions
    public Interaction<Unit, Unit> RemoveLoadPanel { get; }

    public MainViewModel()
    {
        RemoveLoadPanel = new Interaction<Unit, Unit>();
        Task.Run(InitApp);
    }

    private async Task InitApp()
    {
        var locator = Locator.GetLocator();
        try
        {
            var lifeRuntime = Application.Current?.ApplicationLifetime;
            if (lifeRuntime == null)
            {
                Status = "The application lifetime cannot be obtained, please restart the program and contact your administrator.";
                return;
            }
            
            Status = "init database";
            locator.RegisterConstant(DatabaseSettings.GetBdContext(), typeof(IDatabaseContext));

            var database = locator.GetService<IDatabaseContext>();
            if (database == null)
            {
                Status = "The database could not be found";
                return;
            }

            if (lifeRuntime is IClassicDesktopStyleApplicationLifetime)
            {
                await database.CheckInit();
            }

            Status = "init database repositories";
            if (lifeRuntime is IClassicDesktopStyleApplicationLifetime)
            {
                locator.RegisterConstant(new PasswordRepository(locator.GetService<IDatabaseContext>()!), typeof(IPasswordRepository));
            }
            else if (lifeRuntime is ISingleViewApplicationLifetime)
            {
                locator.RegisterConstant(new MobilePasswordRepository(DatabaseSettings.GetPathToDatabase()), typeof(IPasswordRepository));
            }

            Status = "init services";
            locator.RegisterConstant(new PasswordService(locator.GetService<IPasswordRepository>()!), typeof(IPasswordService));
            locator.RegisterConstant(new NavigationService(locator), typeof(INavigationService));
            locator.RegisterConstant(new UserKeyStorageService(), typeof(IUserKeyStorageService));
            if (lifeRuntime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var topLevel = TopLevel.GetTopLevel(desktopLifetime.MainWindow);
                locator.RegisterConstant(new ClipboardService(topLevel?.Clipboard), typeof(IClipboardService));
                locator.RegisterConstant(new DialogService(desktopLifetime.MainWindow!), typeof(IDialogService));
            }
            else if (lifeRuntime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                var topLevel = TopLevel.GetTopLevel(singleViewLifetime.MainView);
                locator.RegisterConstant(new ClipboardService(topLevel?.Clipboard), typeof(IClipboardService));
                locator.RegisterConstant(new DialogService((ContentControl)singleViewLifetime.MainView!), typeof(IDialogService));
            }
            
            locator.RegisterConstant(new PasswordMoreInfoFactory(locator.GetService<IDialogService>()!, locator.GetService<IUserKeyStorageService>()!, locator.GetService<IClipboardService>()!), typeof(IPasswordMoreInfoFactory));
            
            Status = "init views";
            locator.Register(() => new PasswordsViewModel(locator.GetService<IPasswordService>()!, locator.GetService<IPasswordMoreInfoFactory>()!, locator.GetService<IDialogService>()!));
            locator.Register(() => new AddPasswordViewModel(locator.GetService<IPasswordService>()!, locator.GetService<IDialogService>()!, locator.GetService<IUserKeyStorageService>()!));
            locator.Register(() => new AddUserKeyViewModel(locator.GetService<IUserKeyStorageService>()!));
            
            
            locator.Register(() => new AddUserKeyView(), typeof(IViewFor<AddUserKeyViewModel>));
            locator.Register(() => new AddPasswordView(), typeof(IViewFor<AddPasswordViewModel>));
            locator.Register(() => new PasswordsView(), typeof(IViewFor<PasswordsViewModel>));
            locator.Register(() => new PasswordMoreInfoView(), typeof(IViewFor<PasswordMoreInfoViewModel>));

            Status = "Success init app";

            NavigationService = locator.GetService<INavigationService>();
            UserKeyStorage = locator.GetService<IUserKeyStorageService>();
            
            Task.Run(LoadMainView);
        }
        catch (Exception e)
        {
            Status = $"Something error with launch App. {e.Message}\n {e.InnerException}";
            AppLogger.LogError("Error with init app. Message: {Message}. InnerException: {InnerException}", e.Message, e.InnerException);
        }
    }

    private async Task LoadMainView()
    {
        await Dispatcher.UIThread.InvokeAsync(new Action(RemoveLoadPanelMethod));
        await LoadFirstPage();
    }

    private async Task LoadFirstPage()
    {
        if(NavigationService == null || UserKeyStorage == null)
            return;
        
        if (await UserKeyStorage.GetKeyAsync() == null)
        {
            await NavigationService.NavigateTo<AddUserKeyViewModel>();
        }
        else
        {
            await NavigationService.NavigateTo<PasswordsViewModel>();
        }
    }

    private async void RemoveLoadPanelMethod()
    {
        await RemoveLoadPanel.Handle(Unit.Default);
    }
}