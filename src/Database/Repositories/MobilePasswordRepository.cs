using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Core;
using Database.DatabaseModels;
using Database.Models;
using Database.Models.Core;
using Database.Repositories.Interfaces;
using SQLite;

namespace Database.Repositories;

public class MobilePasswordRepository : IPasswordRepository
{
    private readonly SQLiteConnection database;
    
    public MobilePasswordRepository(string databasePath)
    {
        database = new SQLiteConnection(databasePath);
        database.CreateTable<Password>();
    }
    public Task<ActionResultModel<IEnumerable<PasswordModel>>> GetAllPasswordsAsync()
    {
        var result = new ActionResultModel<IEnumerable<PasswordModel>>();
        try
        {
            var passwords = database.Table<Password>().ToList();
            result.IsSuccess = true;
            result.ResultInfos.Add(ActionResultInfo.SuccessGet);
            result.Value = passwords.Select(s => new PasswordModel(s)).ToList();
        }
        catch (Exception e)
        {
            DatabaseLogger.LogError(
                "Error with get all passwords. Message: {Message}.\nInnerException: {InnerException}", e.Message,
                e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailGet);
            result.ResultInfos.Add(ActionResultInfo.InternalError);
        }

        return Task.FromResult(result);
    }

    public async Task<ActionResultModel<PasswordModel>> AddPasswordAsync(PasswordAddModel model)
    {
        var result = new ActionResultModel<PasswordModel>();
        try
        {
            if (string.IsNullOrWhiteSpace(model.EncryptedPassword))
            {
                result.ResultInfos.Add(ActionResultInfo.FailAdd);
            }
            else
            {
                var addModel = new Password
                {
                    Title = model.Title,
                    EncryptedPassword = model.EncryptedPassword,
                    PasswordHash = model.PasswordHash,
                    Iv = model.Iv
                };

                var id = database.Insert(addModel);

                var getResult = await GetAllPasswordsAsync();
                if (!getResult.IsSuccess || getResult.Value == null)
                {
                    result.ResultInfos.Add(ActionResultInfo.SuccessAdd);
                    result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                    result.ResultInfos.Add(ActionResultInfo.FailGet);
                }
                else
                {
                    var passwordFromList = getResult.Value.FirstOrDefault(x => x.Id == id);
                    if (passwordFromList == null)
                    {
                        result.ResultInfos.Add(ActionResultInfo.FailGet);
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.ResultInfos.Add(ActionResultInfo.SuccessAdd);
                        result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                        result.Value = passwordFromList;
                    }
                }
            }
        }
        catch (Exception e)
        {
            DatabaseLogger.LogError("Error with add password. Message: {Message}.\nInnerException: {InnerException}",
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailAdd);
            result.ResultInfos.Add(ActionResultInfo.FailSave);
        }

        return result;
    }

    public Task<ActionResultModel<bool>> RemovePasswordAsync(int id)
    {
        var result = new ActionResultModel<bool>();
        try
        {
            var password = database.Get<Password>(id);
            if (password == null)
            {
                result.ResultInfos.Add(ActionResultInfo.FailGet);
            }
            else
            {
                database.Delete<Password>(id);

                result.IsSuccess = true;
                result.ResultInfos.Add(ActionResultInfo.SuccessRemove);
                result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                result.Value = true;
            }
        }
        catch (Exception e)
        {
            DatabaseLogger.LogError(
                "Error with remove password by Id: {id}. Message: {Message}.\nInnerException: {InnerException}", id,
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailAdd);
            result.ResultInfos.Add(ActionResultInfo.FailSave);
        }

        return Task.FromResult(result);
    }
}