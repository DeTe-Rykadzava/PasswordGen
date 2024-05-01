using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Models.Core;
using Database.Repositories.Interfaces;
using PasswordGenApp.Services.CoreService;
using PasswordGenApp.ViewModels.Database;
using PasswordGenApp.ViewModels.Password;

namespace PasswordGenApp.Services.PassowordService;

public class PasswordService : IPasswordService
{
    private readonly IPasswordRepository _repository;

    public PasswordService(IPasswordRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ActionResultViewModel<IEnumerable<PasswordViewModel>>> GetAllPasswordsAsync()
    {
        var result = new ActionResultViewModel<IEnumerable<PasswordViewModel>>();
        try
        {
            var getResult = await _repository.GetAllPasswordsAsync();

            if (!getResult.IsSuccess || getResult.Value == null)
            {
                result.ResultInfos.Add(ActionResultInfo.FailGet);
            }
            else
            {
                result.IsSuccess = true;
                result.ResultInfos.Add(ActionResultInfo.SuccessGet);
                result.Value = getResult.Value.Select(s => new PasswordViewModel(s)).ToList();
            }
        }
        catch (Exception e)
        {
            AppLogger.LogError("Error with GetAllPasswordsAsync. Message: {Message}. InnerException: {InnerException}",
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailGet);
            result.ResultInfos.Add(ActionResultInfo.InternalError);
        }
        return result;
    }

    public async Task<ActionResultViewModel<PasswordViewModel>> AddPasswordAsync(string title, string encryptedPassword, string passwordHash, byte[] iv)
    {
        var result = new ActionResultViewModel<PasswordViewModel>();
        try
        {
            var addModel = new PasswordAddModel
            {
                Title = title,
                EncryptedPassword = encryptedPassword,
                PasswordHash = passwordHash,
                Iv = iv
            };
            
            var addResult = await _repository.AddPasswordAsync(addModel);

            if (!addResult.IsSuccess || addResult.Value == null)
            {
                result.ResultInfos.Add(ActionResultInfo.FailAdd);
                result.ResultInfos.Add(ActionResultInfo.FailSave);
            }
            else
            {
                result.IsSuccess = true;
                result.ResultInfos.Add(ActionResultInfo.SuccessAdd);
                result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                result.Value = new PasswordViewModel(addResult.Value);
            }
        }
        catch (Exception e)
        {
            AppLogger.LogError("Error with AddPasswordAsync. Message: {Message}. InnerException: {InnerException}",
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailAdd);
            result.ResultInfos.Add(ActionResultInfo.FailSave);
            result.ResultInfos.Add(ActionResultInfo.InternalError);
        }
        return result;
    }

    public async Task<ActionResultViewModel<bool>> RemovePasswordAsync(int id)
    {
        var result = new ActionResultViewModel<bool>();
        try
        {
            var removeResult = await _repository.RemovePasswordAsync(id);

            if (!removeResult.IsSuccess || !removeResult.Value)
            {
                result.ResultInfos.Add(ActionResultInfo.FailRemove);
                result.ResultInfos.Add(ActionResultInfo.FailSave);
            }
            else
            {
                result.IsSuccess = true;
                result.ResultInfos.Add(ActionResultInfo.SuccessRemove);
                result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                result.Value = true;
            }
        }
        catch (Exception e)
        {
            AppLogger.LogError("Error with AddPasswordAsync. Message: {Message}. InnerException: {InnerException}",
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailAdd);
            result.ResultInfos.Add(ActionResultInfo.FailSave);
            result.ResultInfos.Add(ActionResultInfo.InternalError);
        }
        return result;
    }
}