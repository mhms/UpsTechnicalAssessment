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
        private DataTable _datatable;
        
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
            
            
            
            var pagination = new Pagination();
            var imageEdit = Image.FromFile(Path.Combine(_startPath, "edit24.png"));
            var imageDelete = Image.FromFile(Path.Combine(_startPath, "delete24.png"));
            var _dataSet = new DataSet();
            try
            {
                var result = await GetUsers("", page);
                if (result.code == 200)
                {
                    
                    _datatable = new DataTable();
                    _datatable = _dataSet.Tables.Add("TableTest");
                    _datatable.Columns.Add(nameof(UserDto.id), typeof(string));
                    _datatable.Columns.Add(nameof(UserDto.name), typeof(string));
                    _datatable.Columns.Add(nameof(UserDto.email), typeof(string));
                    _datatable.Columns.Add(nameof(UserDto.gender), typeof(string));
                    _datatable.Columns.Add(nameof(UserDto.status), typeof(string));
                    _datatable.Columns.Add(nameof(UserDto.created_at), typeof(DateTime));
                    _datatable.Columns.Add(nameof(UserDto.updated_at), typeof(DateTime));
                    _datatable.Columns.Add("edit", typeof(Bitmap));
                    _datatable.Columns.Add("delete", typeof(Bitmap));
                    
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
                        _datatable.Rows.Add(arrObj);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _messages.Add(new ServiceMessage(MessageType.Error, MessageId.InternalError));
                _exception = ex;
            }

            return new ServiceResult<Tuple<DataTable,DataSet,Pagination>>(true, Tuple.Create(_datatable,_dataSet,pagination), _messages,_exception);
        }

        

        public async Task<ServiceResult> AddUser(UserDto userDto)
        {
            try
            {
                var url = Url.Combine(_apiAddress, "/users");
                var result = await url.WithOAuthBearerToken(_token).PostJsonAsync(userDto).ReceiveJson<ResponseDTO<Object>>();
                
                if (result.code == 201)
                {
                    _messages.Add(new ServiceMessage(MessageType.Succeed, MessageId.UserAddition));
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
                else if ( result.code == 422)
                {
                    _messages.Add(new ServiceMessage(MessageType.Error, MessageId.EmailAddressTaken));
                }
                _messages.Add(new ServiceMessage(MessageType.Error, MessageId.None));
                return new ServiceResult(false,_messages);
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
                if (result.code == 200)
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
            return new ServiceResult(true, _messages,_exception);
        }


    }
}
