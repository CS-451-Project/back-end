using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Models;
using GivingCircle.Api.Providers;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GivingCircle.Api.UnitTest.Providers
{
    public class TestIdentityRoleProvider
    {
        [Fact]
        public async void TestAddIdentityRoleHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();
            var resourceId = Guid.NewGuid().ToString();

            var identityRoleRepositoryMock = new Mock<IIdentityRoleRepository>();
            identityRoleRepositoryMock
                .Setup(x => x.AddIdentityRoleAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(true);

            var identityRoleProviderMock = new IdentityRoleProvider(identityRoleRepositoryMock.Object);

            // When
            var result = await identityRoleProviderMock.AddIdentityRole(userId, resourceId);

            // Then
            Assert.True(result);
        }

        [Fact]
        public async void TestDeleteIdentityRoleHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();
            var resourceId = Guid.NewGuid().ToString();

            var identityRoleRepositoryMock = new Mock<IIdentityRoleRepository>();
            identityRoleRepositoryMock
                .Setup(x => x.DeleteIdentityRoleAsync(userId, resourceId))
                .ReturnsAsync(true);

            var identityRoleProviderMock = new IdentityRoleProvider(identityRoleRepositoryMock.Object);

            // When
            var result = await identityRoleProviderMock.DeleteIdentityRole(userId, resourceId);

            // Then
            Assert.True(result);
        }

        [Fact]
        public async void TestGetIdentityRolesHappyPath()
        {
            // Given
            var userId = Guid.NewGuid().ToString();

            List<IdentityRole> identityRoles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    ResourceId= Guid.NewGuid().ToString(),
                    UserId= userId
                },
                new IdentityRole()
                {
                    ResourceId= Guid.NewGuid().ToString(),
                    UserId= userId
                },
                new IdentityRole()
                {
                    ResourceId= Guid.NewGuid().ToString(),
                    UserId= userId
                },
            };

            var identityRoleRepositoryMock = new Mock<IIdentityRoleRepository>();
            identityRoleRepositoryMock
                .Setup(x => x.GetIdentityRolesAsync(userId))
                .ReturnsAsync(identityRoles);

            var identityRoleProviderMock = new IdentityRoleProvider(identityRoleRepositoryMock.Object);

            // When
            var result = await identityRoleProviderMock.GetIdentityRoles(userId);

            // Then
            Assert.Equal(identityRoles, result);
        }
    }
}
