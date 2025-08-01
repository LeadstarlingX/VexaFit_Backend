﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Application.IAppServices.Workout;
using Application.IRepository;
using Domain.Entities.AppEntities;
using Infrastructure.AppServices.Workout;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs.Workout;
using System;
using System.Linq.Expressions;

using WorkoutEntity = Domain.Entities.AppEntities.Workout;
using ExerciseEntity = Domain.Entities.AppEntities.Exercise;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.AppServices.Workout.Tests
{
    [TestClass()]
    public class WorkoutServiceTests
    {
        private Mock<IAppRepository<WorkoutEntity>> _mockWorkoutRepository;
        private Mock<IAppRepository<WorkoutExercise>> _mockWorkoutExerciseRepository;
        private Mock<IAppRepository<ExerciseEntity>> _mockExerciseRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private IWorkoutService _workoutService;

        [TestInitialize]
        public void TestSetup()
        {
            _mockWorkoutRepository = new Mock<IAppRepository<WorkoutEntity>>();
            _mockWorkoutExerciseRepository = new Mock<IAppRepository<WorkoutExercise>>();
            _mockExerciseRepository = new Mock<IAppRepository<ExerciseEntity>>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "Trainee"),
            }, "mock"));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

            _workoutService = new WorkoutService(
                _mockWorkoutRepository.Object,
                _mockWorkoutExerciseRepository.Object,
                _mockExerciseRepository.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object
            );
        }

        #region CRUD Tests


        [TestMethod()]
        public async Task GetByIdAsync_WhenWorkoutExists_ShouldReturnCorrectWorkout()
        {
            
            var workoutId = 1;
            var userId = "test-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Test Workout", UserId = userId };
            var expectedDto = new WorkoutDTO { Id = workoutId, Name = "Test Workout" };

            
            var workouts = MockDb.CreateAsyncQueryable(new List<WorkoutEntity> { workoutEntity });

            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()
            )).Returns(workouts);

            _mockMapper.Setup(m => m.Map<WorkoutDTO>(workoutEntity)).Returns(expectedDto);

           
            var result = await _workoutService.GetByIdAsync(workoutId);

            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
        }

        [TestMethod()]
        public async Task GetByIdAsync_WhenWorkoutDoesNotExist_ShouldThrowException()
        {
            
            var workoutId = 99;

            
            var emptyList = MockDb.CreateAsyncQueryable(new List<WorkoutEntity>());

            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()
            )).Returns(emptyList);

            
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                async () => await _workoutService.GetByIdAsync(workoutId)
            );
            exception.Message.Should().Be("Workout not found");
        }

        [TestMethod]
        public async Task CreateAsync_WhenCalled_AssignsCorrectUserIdAndSaves()
        {
            
            var createDto = new CreateWorkoutDTO { Name = "New Workout", Description = "A test." };
            var userId = "test-user-id"; 

            var workoutEntity = new CustomWorkout { Name = createDto.Name, Description = createDto.Description };

            _mockMapper.Setup(m => m.Map<CustomWorkout>(createDto)).Returns(workoutEntity);



           
            await _workoutService.CreateAsync(createDto);

            
            _mockWorkoutRepository.Verify(
                repo => repo.InsertAsync(
                    It.Is<CustomWorkout>(w => w.UserId == userId && w.Name == createDto.Name),
                    It.IsAny<bool>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task UpdateAsync_WhenUserOwnsWorkout_ShouldUpdateSuccessfully()
        {
            
            var workoutId = 1;
            var userId = "test-user-id";
            var updateDto = new UpdateWorkoutDTO { Id = workoutId, Name = "Updated Name" };
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Original Name", UserId = userId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

           
            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateWorkoutDTO>(), It.IsAny<WorkoutEntity>()))
                       .Callback<UpdateWorkoutDTO, WorkoutEntity>((dto, entity) =>
                       {
                           
                           entity.Name = dto.Name;
                       });

           
            await _workoutService.UpdateAsync(updateDto);

            _mockWorkoutRepository.Verify(
                repo => repo.UpdateAsync(It.Is<WorkoutEntity>(w => w.Name == "Updated Name"), It.IsAny<bool>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task UpdateAsync_WhenUserDoesNotOwnWorkout_ShouldThrowUnauthorizedAccessException()
        {
            
            var workoutId = 2;
            var ownerUserId = "another-user-id"; 
            var updateDto = new UpdateWorkoutDTO { Id = workoutId, Name = "Updated Name" };
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Original Name", UserId = ownerUserId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
                () => _workoutService.UpdateAsync(updateDto)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenUserOwnsWorkout_ShouldDeleteSuccessfully()
        {
            
            var workoutId = 1;
            var userId = "test-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, UserId = userId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                 .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            
            await _workoutService.DeleteAsync(workoutId);

           
            _mockWorkoutRepository.Verify(repo => repo.RemoveAsync(workoutEntity, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenUserDoesNotOwnWorkout_ShouldThrowException()
        {
            
            var workoutId = 2;
            var ownerUserId = "another-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, UserId = ownerUserId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            
            await Assert.ThrowsExceptionAsync<Exception>(
                () => _workoutService.DeleteAsync(workoutId)
            );
        }


        [TestMethod]
        public async Task UpdateAsync_WhenUserIsAdmin_ShouldUpdateAnyWorkout()
        {
            
            var workoutId = 2;
            var ownerUserId = "another-user-id"; 
            var updateDto = new UpdateWorkoutDTO { Id = workoutId, Name = "Admin Updated Name" };
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Original Name", UserId = ownerUserId };

            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "admin-user-id"),
        new Claim(ClaimTypes.Role, "Admin"), 
            }, "mock"));

            
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

           
            await _workoutService.UpdateAsync(updateDto);

            _mockWorkoutRepository.Verify(
                repo => repo.UpdateAsync(workoutEntity, It.IsAny<bool>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenUserIsAdmin_ShouldDeleteAnyWorkout()
        {
            
            var workoutId = 2;
            var ownerUserId = "another-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, UserId = ownerUserId };

            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "admin-user-id"),
        new Claim(ClaimTypes.Role, "Admin"),
            }, "mock"));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            
            await _workoutService.DeleteAsync(workoutId);

           
            _mockWorkoutRepository.Verify(repo => repo.RemoveAsync(workoutEntity, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenWorkoutNotFound_ShouldThrowKeyNotFoundException()
        {
            
            var nonExistentWorkoutId = 999;
            var updateDto = new UpdateWorkoutDTO { Id = nonExistentWorkoutId, Name = "Doesn't Matter" };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity>());

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _workoutService.UpdateAsync(updateDto)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenWorkoutNotFound_ShouldThrowKeyNotFoundException()
        {
            var nonExistentWorkoutId = 999;

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity>());

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _workoutService.DeleteAsync(nonExistentWorkoutId)
            );
        }


        [TestMethod]
        public async Task UpdateAsync_WithPredefinedWorkoutAsAdmin_ShouldUpdateSuccessfully()
        {
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var dto = new UpdateWorkoutDTO { Id = 1, Name = "Updated Predefined" };
            var workout = new PredefinedWorkout { Id = 1, Name = "Original" };

            _mockWorkoutRepository.Setup(repo =>
                repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });

            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateWorkoutDTO>(), It.IsAny<WorkoutEntity>()))
                .Callback<UpdateWorkoutDTO, WorkoutEntity>((src, dest) => dest.Name = src.Name);

            await _workoutService.UpdateAsync(dto);

            _mockWorkoutRepository.Verify(repo =>
                repo.UpdateAsync(It.Is<WorkoutEntity>(w => w.Name == "Updated Predefined"),
                It.IsAny<bool>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenWorkoutIsPredefined_ShouldReturnForNonAdmin()
        {
            var workoutId = 1;
            var workout = new PredefinedWorkout { Id = workoutId, Name = "Beginner Routine" };
            var expectedDto = new WorkoutDTO { Id = workoutId, Name = "Beginner Routine" };

            var workouts = MockDb.CreateAsyncQueryable(new List<WorkoutEntity> { workout });
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(workouts);

            _mockMapper.Setup(m => m.Map<WorkoutDTO>(workout)).Returns(expectedDto);

            var result = await _workoutService.GetByIdAsync(workoutId);

            result.Should().NotBeNull();
            result.Id.Should().Be(workoutId);
            result.Name.Should().Be("Beginner Routine");
        }


        [TestMethod]
        public async Task UpdateAsync_WhenDtoIsValid_ShouldReturnUpdatedDTO()
        {
            var dto = new UpdateWorkoutDTO { Id = 1, Name = "Updated" };
            var workout = new CustomWorkout { Id = 1, Name = "Original", UserId = "test-user-id" };
            var expectedDto = new WorkoutDTO { Id = 1, Name = "Updated" };

            _mockWorkoutRepository.Setup(repo =>
                repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });

            _mockMapper.Setup(m => m.Map<WorkoutDTO>(It.IsAny<WorkoutEntity>()))
                .Returns(expectedDto);

            var result = await _workoutService.UpdateAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Updated");
        }

        [TestMethod]
        public async Task CreateAsync_WhenDtoIsValid_ShouldReturnMappedDTO()
        {
            var createDto = new CreateWorkoutDTO { Name = "New Workout" };
            var createdWorkout = new CustomWorkout { Id = 1, Name = "New Workout", UserId = "test-user-id" };
            var expectedDto = new WorkoutDTO { Id = 1, Name = "New Workout" };

            _mockMapper.Setup(m => m.Map<CustomWorkout>(createDto)).Returns(createdWorkout);
            _mockMapper.Setup(m => m.Map<WorkoutDTO>(createdWorkout)).Returns(expectedDto);

            var result = await _workoutService.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Workout");
        }

        #endregion


        #region Query and Filter Tests

        [TestMethod]
        public async Task GetAllAsync_WithDescriptionFilter_ShouldReturnFilteredResults()
        {
            
            var dto = new GetWorkoutDTO { Description = "cardio" };
            var workouts = new List<WorkoutEntity>
    {
        new CustomWorkout { Id = 1, Description = "Morning cardio session", UserId = "test-user-id" },
        new CustomWorkout { Id = 2, Description = "Strength training", UserId = "test-user-id" },
        new PredefinedWorkout { Id = 3, Description = "Advanced cardio" }
    };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(queryable);

            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns((IEnumerable<WorkoutEntity> entities) =>
                    entities.Select(e => new WorkoutDTO { Id = e.Id, Description = e.Description }));

            
            var result = await _workoutService.GetAllAsync(dto);

            
            result.Should().HaveCount(2);
            result.Should().Contain(d => d.Description.Contains("cardio"));
            result.Should().NotContain(d => d.Description == "Strength training");
        }


        [TestMethod]
        public async Task GetAllAsync_WhenNoWorkoutsMatch_ShouldReturnEmptyList()
        {
            
            var dto = new GetWorkoutDTO { Name = "NonExistentWorkout" };
            var workouts = new List<WorkoutEntity>(); 

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(queryable);

            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns(Enumerable.Empty<WorkoutDTO>());

           
            var result = await _workoutService.GetAllAsync(dto);

           
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllAsync_WithUserIdFilterForNonAdmin_ShouldIgnoreFilter()
        {
            
            var dto = new GetWorkoutDTO { UserId = "other-user" };
            var workouts = new List<WorkoutEntity>
    {
        new CustomWorkout { Id = 1, UserId = "test-user-id", Name = "Mine" },
        new CustomWorkout { Id = 2, UserId = "other-user", Name = "Theirs" },
        new PredefinedWorkout { Id = 3, Name = "Shared" }
    };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(queryable);

            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns((IEnumerable<WorkoutEntity> entities) =>
                    entities.Select(e => new WorkoutDTO { Id = e.Id, Name = e.Name }));

            
            var result = await _workoutService.GetAllAsync(dto);

            
            result.Should().HaveCount(2); 
            result.Should().Contain(d => d.Name == "Mine");
            result.Should().Contain(d => d.Name == "Shared");
            result.Should().NotContain(d => d.Name == "Theirs");
        }

        [TestMethod]
        public async Task GetAllAsync_WithNameFilter_ShouldReturnFilteredResults()
        {
            
            var dto = new GetWorkoutDTO { Name = "Cardio" };
            var workouts = new List<WorkoutEntity>
                {   new CustomWorkout { Id = 1, Name = "Morning Cardio", UserId = "test-user-id" },
                    new CustomWorkout { Id = 2, Name = "Strength", UserId = "test-user-id" } };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            
            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                       .Returns((IEnumerable<WorkoutEntity> entities) =>
                           entities.Select(e => new WorkoutDTO { Id = e.Id, Name = e.Name })
                       );

           
            var result = await _workoutService.GetAllAsync(dto);

            
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Morning Cardio");
        }

        [TestMethod]
        public async Task GetAllAsync_WithPredefinedFilter_ShouldReturnOnlyPredefined()
        {
            
            var dto = new GetWorkoutDTO { Discriminator = WorkoutEnum.Predefined };
            var workouts = new List<WorkoutEntity>
                {   new PredefinedWorkout { Id = 1, Name = "Beginner" },
                    new CustomWorkout { Id = 2, Name = "Custom", UserId = "test-user-id" } };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            
            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                       .Returns((IEnumerable<WorkoutEntity> entities) =>
                           entities.Select(e => new WorkoutDTO { Id = e.Id, Name = e.Name })
                       );

           
            var result = await _workoutService.GetAllAsync(dto);

            
            result.Should().HaveCount(1);
            result.First().Should().BeOfType<WorkoutDTO>();
        }

        [TestMethod]
        public async Task GetAllAsync_WithAdminUser_ShouldReturnAllWorkouts()
        {
            
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var workouts = new List<WorkoutEntity>
            {
                new CustomWorkout { Id = 1, UserId = "user1" },
                new CustomWorkout { Id = 2, UserId = "user2" },
                new PredefinedWorkout { Id = 3 }
            };

            
            var asyncQueryableWorkouts = MockDb.CreateAsyncQueryable(workouts);

            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                    It.IsAny<bool>(),
                    It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(asyncQueryableWorkouts);

            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns((IEnumerable<WorkoutEntity> entities) =>
                    entities.Select(e => new WorkoutDTO { Id = e.Id }));

            
            var result = await _workoutService.GetAllAsync(new GetWorkoutDTO());

            result.Should().HaveCount(3);
        }

        #endregion


        #region Security and Authorization Tests

        [TestMethod]
        public async Task CreateAsync_WhenUserNotAuthenticated_ShouldThrow()
        {
            
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            var createDto = new CreateWorkoutDTO { Name = "Should Fail" };

            
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _workoutService.CreateAsync(createDto)
            );
        }

        [TestMethod]
        public async Task GetByIdAsync_UnauthorizedAccessAttempt_ShouldThrowNotFoundException()
        {
            
            var workoutIdToAccess = 5;
            var ownerUserId = "another-user-id";
            var workoutOfOtherUser = new CustomWorkout { Id = workoutIdToAccess, Name = "Secret Workout", UserId = ownerUserId };

            
            var workoutsInDb = MockDb.CreateAsyncQueryable(new List<WorkoutEntity> { workoutOfOtherUser });
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()
            )).Returns(workoutsInDb);

           
            var ex = await Assert.ThrowsExceptionAsync<Exception>(
                () => _workoutService.GetByIdAsync(workoutIdToAccess)
            );
            ex.Message.Should().Be("Workout not found");
        }

        [TestMethod]
        public async Task UpdateAsync_WithPredefinedWorkoutAsNonAdmin_ShouldThrow()
        {
            
            var dto = new UpdateWorkoutDTO { Id = 1 };
            var workout = new PredefinedWorkout { Id = 1 };

            
            _mockWorkoutRepository.Setup(repo =>
                    repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });

            
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _workoutService.UpdateAsync(dto));
        }

        [TestMethod]
        public async Task DeleteAsync_WithPredefinedWorkoutAsAdmin_ShouldSucceed()
        {
            
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            { new Claim(ClaimTypes.Role, "Admin")
                    }, "mock")); 
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var workout = new PredefinedWorkout { Id = 1 };

            
            _mockWorkoutRepository.Setup(repo =>
                    repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });

            
            await _workoutService.DeleteAsync(1);

            
            _mockWorkoutRepository.Verify(repo =>
                repo.RemoveAsync(It.Is<WorkoutEntity>(w => w.Id == 1), It.IsAny<bool>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_ForNonAdmin_ShouldReturnOnlyOwnedAndPredefinedWorkouts()
        {
            
            var mockWorkoutRepository = new Mock<IAppRepository<WorkoutEntity>>();
            var mockMapper = new Mock<IMapper>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            
            var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        new Claim(ClaimTypes.Role, "Trainee"),
            }, "mock"));

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = nonAdminUser });

            var workoutService = new WorkoutService(
                mockWorkoutRepository.Object,
                _mockWorkoutExerciseRepository.Object, 
                _mockExerciseRepository.Object,     
                mockMapper.Object,
                mockHttpContextAccessor.Object
            );

            var workouts = new List<WorkoutEntity>
    {
        new CustomWorkout { Id = 1, Name = "My Workout", UserId = "test-user-id" },
        new CustomWorkout { Id = 2, Name = "Someone Else's Workout", UserId = "other-user" },
        new PredefinedWorkout { Id = 3, Name = "Beginner Routine" }
    };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                       .Returns((IEnumerable<WorkoutEntity> entities) =>
                           entities.Select(e => new WorkoutDTO { Id = e.Id, Name = e.Name })
                       );

           
            var result = await workoutService.GetAllAsync(new GetWorkoutDTO());

           
            result.Should().HaveCount(2);
            result.Should().Contain(dto => dto.Name == "My Workout");
            result.Should().Contain(dto => dto.Name == "Beginner Routine");
            result.Should().NotContain(dto => dto.Name == "Someone Else's Workout");
        }

        #endregion


        #region Validation Tests

        [TestMethod]
        public async Task CreateAsync_WithNullDto_ShouldThrow()
        {
           
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _workoutService.CreateAsync(null));
        }

        [TestMethod]
        public async Task DeleteBulkAsync_WithEmptyList_ShouldNotCallRepository()
        {
            
            await _workoutService.DeleteBulkAsync(new List<int>());

            
            _mockWorkoutRepository.Verify(repo =>
                repo.BulkRemoveAsync(It.IsAny<IEnumerable<int>>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithInvalidId_ShouldThrowProperException()
        {
            
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutEntity>().AsQueryable()));

          
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _workoutService.GetByIdAsync(999));
            ex.Message.Should().Be("Workout not found");
        }

        [TestMethod]
        public async Task AddToWorkout_WithNonExistentWorkout_ShouldThrow()
        {
            
            var dto = new AddtoWorkoutDTO { workoutId = 99 };

            _mockWorkoutRepository.Setup(repo =>
                    repo.FindAsync(
                        It.IsAny<Expression<Func<WorkoutEntity, bool>>>(),
                        It.IsAny<bool>(),
                        It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity>());

            
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("Workout wasn't found");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WithValidData_ShouldUpdateProperties()
        {
            
            var dto = new UpdateWorkoutExerciseDTO
            {
                WorkoutExerciseId = 1,
                Sets = 4,
                Reps = 12,
                WeightKg = 50
            };

            var workoutExercise = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "test-user-id" }
            };

            _mockWorkoutExerciseRepository.Setup(repo =>
                    repo.GetAll(
                        It.IsAny<bool>(),
                        It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise }));

            
            await _workoutService.UpdateExerciseInWorkout(dto);

            
            workoutExercise.Sets.Should().Be(4);
            workoutExercise.Reps.Should().Be(12);
            workoutExercise.WeightKg.Should().Be(50);
            _mockWorkoutExerciseRepository.Verify(repo =>
                repo.UpdateAsync(It.Is<WorkoutExercise>(we => we.Id == 1), It.IsAny<bool>()), 
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_WithFullFilter_ShouldApplyAllFilters()
        {
            var dto = new GetWorkoutDTO
            {
                Discriminator = WorkoutEnum.Custom,
                Name = "Morning",
                UserId = "specific-user"
            };

            var workouts = new List<WorkoutEntity>
    {
        new CustomWorkout { Name = "Morning Routine", UserId = "specific-user" },
        new CustomWorkout { Name = "Evening Routine", UserId = "specific-user" },
        new CustomWorkout { Name = "Morning Jog", UserId = "other-user" }
    };

            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns((IEnumerable<WorkoutEntity> entities) =>
                   
                    entities.Select(e => new WorkoutDTO { Name = e.Name })
                );

            var result = await _workoutService.GetAllAsync(dto);

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Morning Routine");
        }

        #endregion



    }
}