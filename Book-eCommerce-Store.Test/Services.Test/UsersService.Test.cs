using NUnit.Framework;
using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Services.UsersService;
using Book_eCommerce_Store.Data;
using Moq;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Book_eCommerce_Store.Test.Services.Test
{
    internal class UsersServiceTest
    {
        private Mock<IMapper> _mapper;
        private Mock<DataContext> _context;
        private readonly UsersService usersService;

        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _context = new Mock<DataContext>(MockBehavior.Strict);
        }

        [Test]
        public async Task Get_ReturnsSuccessfully()
        {

        }
    }
}
