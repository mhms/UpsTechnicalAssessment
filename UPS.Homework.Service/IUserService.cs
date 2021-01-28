using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UPS.Homework.DTO;

namespace UPS.Homework.Service
{
    public interface IUserService
    {
        Task<ResponseDTO<List<UserDto>>> GetUsers(string name="", int page = 0);
        Task<ServiceResult> AddUser(UserDto userDto);
        Task<ServiceResult> UpdateUser(UserDto userDto);
        Task<ServiceResult<Tuple<DataTable, DataSet, Pagination>>> LoadPage(int page);
        Task<ServiceResult> DeleteUser(int id);
        


    }
}