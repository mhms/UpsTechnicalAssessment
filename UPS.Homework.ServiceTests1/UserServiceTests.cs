using NUnit.Framework;
using UPS.Homework.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.AreEqual(result.code, 200);
            Assert.IsTrue(result.data.Count > 0);
        }
        [Test()]
        public async Task GetUsersPage1Test()
        {
            var result = await _userService.GetUsers("", 5);
            Assert.AreEqual(result.code, 200);
            Assert.IsTrue(result.data.Count > 0);
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
            Assert.Fail();
        }
    }
}