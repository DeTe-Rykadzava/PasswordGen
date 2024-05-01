using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using MsBox.Avalonia.Enums;
using PasswordGenApp.Services.DialogService;
using PasswordGenApp.Services.NavigationService;
using PasswordGenApp.Services.PassowordService;
using PasswordGenApp.ViewModels.Core;
using PasswordGenApp.ViewModels.Database;
using PasswordGenApp.ViewModels.Password.Factory;
using ReactiveUI;

namespace PasswordGenApp.ViewModels.Password;

public class PasswordsViewModel : RoutableViewModelBase
{
    public override string ViewModelViewPath { get; } = "passwords";
    public override INavigationService RootNavManager { get; protected set; } = null!;
    
    // services
    private readonly IPasswordService _passwordService;
    private readonly IPasswordMoreInfoFactory _passwordMoreInfoFactory;
    private readonly IDialogService _dialogService;
    
    // fields
    public ObservableCollection<PasswordViewModel> Passwords { get; } = new ();

    private bool _passwordsIsEmpty = true;
    public bool PasswordsIsEmpty
    {
        get => _passwordsIsEmpty;
        private set => this.RaiseAndSetIfChanged(ref _passwordsIsEmpty, value);
    }
    
    // commands
    public ICommand AddPasswordCommand { get; }
    public ReactiveCommand<PasswordViewModel, Unit> RemovePasswordCommand { get; }
    public ReactiveCommand<PasswordViewModel, Unit> PasswordMoreInfoCommand { get; }

    public PasswordsViewModel(IPasswordService passwordService, IPasswordMoreInfoFactory passwordMoreInfoFactory, IDialogService dialogService)
    {
        _passwordService = passwordService;
        _passwordMoreInfoFactory = passwordMoreInfoFactory;
        _dialogService = dialogService;

        AddPasswordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await RootNavManager.NavigateTo<AddPasswordViewModel>();
        });

        RemovePasswordCommand = ReactiveCommand.CreateFromTask(async (PasswordViewModel password) =>
        {
            var dialogResult = await _dialogService.ShowWindowDialogAsync("Question","Are you sure?", ButtonEnum.YesNo, Icon.Question);
            if(dialogResult == ButtonResult.No)
                return;

            var removeResult = await _passwordService.RemovePasswordAsync(password.Id);
            if (!removeResult.IsSuccess || !removeResult.Value)
            {
                await _dialogService.ShowWindowDialogAsync("Error",$"The record could not be deleted, the reasons are:\n\t *{string.Join("\n\t *",removeResult.ResultInfos.Select(s=> s.ToString()).ToList())}", icon:Icon.Error);
                return;                
            }

            await _dialogService.ShowWindowDialogAsync("Success", "Success", icon: Icon.Success);
            Passwords.Remove(password);
        });

        PasswordMoreInfoCommand = ReactiveCommand.CreateFromTask(async (PasswordViewModel password) =>
        {
            var vm = _passwordMoreInfoFactory.GetPasswordMoreInfo(password);
            await RootNavManager.NavigateTo(vm);
        });

        Passwords.CollectionChanged += (sender, args) => { PasswordsIsEmpty = !Passwords.Any(); };
    }

    public override Task OnShowed()
    {
        Task.Run(LoadPasswords);
        return Task.FromResult(Unit.Default);
    }

    private async Task LoadPasswords()
    {
        await Dispatcher.UIThread.InvokeAsync(new Action(() =>
        {
            Passwords.Clear();
        }));
        var getResult = await _passwordService.GetAllPasswordsAsync();
        if(!getResult.IsSuccess || getResult.Value == null)
            return;
        foreach (var password in getResult.Value)
        {
            await Dispatcher.UIThread.InvokeAsync(new Action(() =>
            {
                Passwords.Add(password);
            }));
        }
    }
}