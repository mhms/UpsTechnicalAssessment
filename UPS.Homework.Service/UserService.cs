using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.ServiceModel.Security;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using UPS.Homework.DTO;
using Image = System.Drawing.Image;


namespace UPS.Homework.Service
{
    public class UserService : BaseService,IUserService
    {
        private string _apiAddress;
        private string _token;
        private string _startPath;
        public UserService(string startupPath)
        {
            _apiAddress = WebConfigurationManager.AppSettings["api_address"];
            _token = WebConfigurationManager.AppSettings["token"];
            _startPath = startupPath;

        }
        public async Task<ResponseDTO<List<UserDto>>> GetUsers(string name="", int page = 0)
        {
            try
            {
                var url = Url.Combine(_apiAddress, "users");
                if (page > 0)
                    url = url.SetQueryParam("page", page);
                if (!string.IsNullOrWhiteSpace(name))
                    url = url.SetQueryParam("name", name);
                var result = await url.GetJsonAsync<ResponseDTO<List<UserDto>>>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                var error = await ex.GetResponseStringAsync();
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

            
        }
        public async Task<ServiceResult<Tuple<DataTable,DataSet, Pagination>>> LoadPage(int page)
        {
            
            var dataSet = new DataSet();
            var dataTable = new DataTable();
            var pagination = new Pagination();
            var imageEdit = Image.FromFile(Path.Combine(_startPath, "edit24.png"));
            var imageDelete = Image.FromFile(Path.Combine(_startPath, "delete24.png"));
            dataTable = dataSet.Tables.Add("TableTest");
            dataTable.Columns.Add(nameof(UserDto.id), typeof(string));
            dataTable.Columns.Add(nameof(UserDto.name), typeof(string));
            dataTable.Columns.Add(nameof(UserDto.email), typeof(string));
            dataTable.Columns.Add(nameof(UserDto.gender), typeof(string));
            dataTable.Columns.Add(nameof(UserDto.status), typeof(string));
            dataTable.Columns.Add(nameof(UserDto.created_at), typeof(DateTime));
            dataTable.Columns.Add(nameof(UserDto.updated_at), typeof(DateTime));
            dataTable.Columns.Add("edit", typeof(Bitmap));
            dataTable.Columns.Add("delete", typeof(Bitmap));
            try
            {
                var result = await GetUsers("", page);
                if (result.code == 200)
                {
                    pagination = result.meta.pagination;
                    foreach (var record in result.data)
                    {
                        var arrObj = new object[]
                        {
                        record.id,
                        record.name,
                        record.email,
                        record.gender,
                        record.status,
                        record.created_at,
                        record.updated_at,
                        imageEdit,
                        imageDelete
                        };
                        dataTable.Rows.Add(arrObj);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _messages.Add(new ServiceMessage(MessageType.Error, MessageId.InternalError));
                _exception = ex;
            }

            return new ServiceResult<Tuple<DataTable,DataSet,Pagination>>(true, Tuple.Create(dataTable,dataSet,pagination), _messages,_exception);
        }

        

        public async Task<int> AddUser(UserDto userDto)
        {
            try
            {
                var url = Url.Combine(_apiAddress, "/users");
                var postJsonAsync = await url.WithOAuthBearerToken(_token).PostJsonAsync(userDto);
                return postJsonAsync.StatusCode;
            }
            catch (FlurlHttpException fex)
            {
                var error = fex.StatusCode;
                throw;
            }
            catch (Exception exception)
            {
                var error = exception.Message;
                throw;
            }
        }

        public async Task<ServiceResult> UpdateUser(UserDto userDto)
        {
            try
            {
                var url = Url.Combine(_apiAddress, "/users", userDto.id.ToString());
                var result = await url.WithOAuthBearerToken(_token).PutJsonAsync(userDto).ReceiveJson<ResponseDTO<Object>>();
                if (result.code == 204)
                {
                    return new ServiceResult(true, _messages);
                }
                else if (result.code == 401)
                {
                    _messages.Add(new ServiceMessage(MessageType.Error, MessageId.AccessDenied));
                    return new ServiceResult(false, _messages);
                }
                else if (result.code == 404)
                {
                    _messages.Add(new ServiceMessage(MessageType.Error, MessageId.EntityDoesNotExist));
                    return new ServiceResult(false, _messages);
                }
                _messages.Add(new ServiceMessage(MessageType.Error, MessageId.None));
                return new ServiceResult(false);
            }
            catch (FlurlHttpException fex)
            {
                var error = fex.StatusCode;
                throw;
            }
            catch (Exception exception)
            {
                var error = exception.Message;
                throw;
            }
        }
        public async Task<ServiceResult> DeleteUser(int id)
        {
            
            try
            {

                var url = Url.Combine(_apiAddress, "/users", id.ToString());
                var result = await url.WithOAuthBearerToken(_token).DeleteAsync().ReceiveJson<ResponseDTO<Object>>();
                if (result.code == 204)
                {
                    return new ServiceResult<object>(true, null, _messages);
                }
                else if (result.code == 401)
                {
                    _messages.Add(new ServiceMessage(MessageType.Error, MessageId.AccessDenied));
                    return new ServiceResult<object>(false, null, _messages);
                }
                else if(result.code == 404)
                {
                    _messages.Add(new ServiceMessage(MessageType.Error, MessageId.EntityDoesNotExist));
                    return new ServiceResult<object>(false,null, _messages);
                }
                
            }
            catch (FlurlHttpException fex)
            {
                var error = fex.StatusCode;
                _exception = fex;
            }
            catch (Exception exception)
            {
                var error = exception.Message;
                _exception = exception;
            }
            return new ServiceResult<object>(true, null, messages,_exception);
        }


    }
}
