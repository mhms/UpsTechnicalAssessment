using NUnit.Framework;
using UPS.Homework.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using UPS.Homework.CrossCutting;
using UPS.Homework.DTO;

namespace UPS.Homework.Service.Tests
{
    [TestFixture()]
    public class UserServiceTests
    {
        private UserService _userService;
        [SetUp]
        public void SetUp()
        {
            _userService = new UserService("");
        }

        [Test()]
        public async  Task DeleteUserTest()
        {
            var deleteServiceResult = await _userService.DeleteUser(38);

        }

        public UserServiceTests()
        {
            _userService = new UserService("");
        }
        [Test]
        public async Task GetUsersTest()
        {
            var result = await _userService.GetUsers();
            Assert.AreEqual(result.Result.code, 200);
            Assert.IsTrue(result.Result.data.Count > 0);
        }
        [Test()]
        public async Task GetUsersPage1Test()
        {
            var result = await _userService.GetUsers("", 5);
            Assert.AreEqual(result.Result.code, 200);
            Assert.IsTrue(result.Result.data.Count > 0);
        }

        [Test()]
        public async Task AddUserTest()
        {
            var user = new UserDto()
            {
                email = "mm@gmail.com",
                name = "James Cameron",
                gender = "Male",
                status = "Active"

            };
            var result = await _userService.AddUser(user);
            Assert.AreEqual(result, 200);
        }

        [Test()]
        public async Task UpdateUserTest()
        {
            var user = new UserDto()
            {
                id = 1813,
                email = "mm@gmail.com",
                name = "James Cameron11",
                gender = "Male",
                status = "Active"

            };
            var result = await _userService.UpdateUser(user);
            Assert.AreEqual(result.Succeeded,true);
            
        }

        [Test]
        public async Task AddUserDuplicateEmailMockTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var messages = new List<ServiceMessage>();
            messages.Add(new ServiceMessage(MessageType.Error, MessageId.EmailAddressTaken));
            userServiceMock.Setup(p =>  p.AddUser(new UserDto())).Returns(Task.FromResult(new ServiceResult(false, messages)));
            Assert.True(userServiceMock.Object == null);
            var result = await  userServiceMock.Object.AddUser(new UserDto());

            var message = result.Messages[0];
            var messageDescription = message.Message.GetEnumDescription();
            var messageTitle = message.Type.GetEnumDescription();



        }
    }
}