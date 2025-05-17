using AutoFixture;
using AutoMapper;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Data.Interfaces;
using WebApi.Managers;
using WebApi.Managers.Intefaces;

namespace WebApi.Tests.Controllers
{
    public class JobsControllerTest
    {
        private readonly Mock<IStorageConnection> _connection;
        private readonly Fixture _fixture;
        private readonly JobsController _controller;
        public JobsControllerTest()
        {
            _fixture = new Fixture();
            _connection = new Mock<IStorageConnection>();
            _controller = new JobsController();
        }

        [Fact]
        public void GetJobStatus_ShouldReturnNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = "non-existent-job-id";
            _connection.Setup(c => c.GetJobData(jobId)).Returns((JobData)null);

            var mockJobStorage = new Mock<JobStorage>();
            mockJobStorage.Setup(s => s.GetConnection()).Returns(_connection.Object);
            JobStorage.Current = mockJobStorage.Object;

            // Act
            var result = _controller.GetJobStatus(jobId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void GetJobStatus_ShouldReturnOk_WithJobStatus()
        {
            // Arrange
            var jobId = "existing-job-id";
            var jobData = new JobData { State = "Succeeded" };

            _connection.Setup(c => c.GetJobData(jobId)).Returns(jobData);

            var mockJobStorage = new Mock<JobStorage>();
            mockJobStorage.Setup(s => s.GetConnection()).Returns(_connection.Object);
            JobStorage.Current = mockJobStorage.Object;

            // Act
            var result = _controller.GetJobStatus(jobId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}
