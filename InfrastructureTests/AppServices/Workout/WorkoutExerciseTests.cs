using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace Infrastructure.AppServices.Workout.Tests
{
    [TestClass()]
    public class WorkoutExerciseTests
    {
        // 1. Declare mocks for all dependencies
        private Mock<IAppRepository<WorkoutEntity>> _mockWorkoutRepository;
        private Mock<IAppRepository<WorkoutExercise>> _mockWorkoutExerciseRepository;
        private Mock<IAppRepository<ExerciseEntity>> _mockExerciseRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private IWorkoutService _workoutService;

        // 2. This method runs before every test to set up the environment
        [TestInitialize]
        public void TestSetup()
        {
            // Initialize all mocks
            _mockWorkoutRepository = new Mock<IAppRepository<WorkoutEntity>>();
            _mockWorkoutExerciseRepository = new Mock<IAppRepository<WorkoutExercise>>();
            _mockExerciseRepository = new Mock<IAppRepository<ExerciseEntity>>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Mock a standard, non-admin user for most tests
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "Trainee"),
            }, "mock"));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

            // Create the service instance with the mocked objects
            _workoutService = new WorkoutService(
                _mockWorkoutRepository.Object,
                _mockWorkoutExerciseRepository.Object,
                _mockExerciseRepository.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object
            );
        }


        #region AddToWorkout Tests

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseIsNotPresent_ShouldAddSuccessfully()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1, Sets = 3, Reps = 10 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exerciseEntity = new ExerciseEntity { Id = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { exerciseEntity });

            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .ReturnsAsync(new List<WorkoutExercise>()); // Simulate exercise not already in workout

            // ACT
            await _workoutService.AddToWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(
                repo => repo.InsertAsync(
                    It.Is<WorkoutExercise>(we => we.WorkoutId == dto.workoutId && we.ExerciseId == dto.exerciseId),
                    It.IsAny<bool>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseIsAlreadyPresent_ShouldThrowException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exerciseEntity = new ExerciseEntity { Id = 1 };
            var existingWorkoutExercise = new WorkoutExercise { WorkoutId = 1, ExerciseId = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { exerciseEntity });

            // Simulate that the exercise already exists in the workout
            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .ReturnsAsync(new List<WorkoutExercise> { existingWorkoutExercise });

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("This exercise already belong to this workout");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenEntryExistsAndUserIsOwner_ShouldUpdateSuccessfully()
        {
            // ARRANGE
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1, Sets = 5, Reps = 5 };
            var workoutExerciseEntity = new WorkoutExercise
            {
                Id = 1,
                Sets = 3,
                Reps = 10,
                Workout = new CustomWorkout { UserId = "test-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExerciseEntity });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(workoutExercises);

            // ACT
            await _workoutService.UpdateExerciseInWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(
                repo => repo.UpdateAsync(
                    It.Is<WorkoutExercise>(we => we.Sets == 5 && we.Reps == 5),
                    It.IsAny<bool>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenEntryExistsAndUserIsOwner_ShouldRemoveSuccessfully()
        {
            // ARRANGE
            var dto = new DeleteFromWorkoutDTO { Id = 1 };
            var workoutExerciseEntity = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "test-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExerciseEntity });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                         .Returns(workoutExercises);

            // ACT
            await _workoutService.DeleteFromWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(repo => repo.RemoveAsync(workoutExerciseEntity, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task AddToWorkout_WhenDataIsValid_ShouldInsertSuccessfully()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exercise = new ExerciseEntity { Id = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity> { exercise });
            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                .ReturnsAsync(new List<WorkoutExercise>()); // Simulate exercise not already in workout

            // ACT
            await _workoutService.AddToWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(r => r.InsertAsync(It.Is<WorkoutExercise>(we => we.WorkoutId == 1 && we.ExerciseId == 1), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task AddToWorkout_WhenWorkoutNotFound_ShouldThrowException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 99, exerciseId = 1 };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity>()); // Simulate workout not found

            // ACT & ASSERT
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("Workout wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseNotFound_ShouldThrowException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 99 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity>()); // Simulate exercise not found

            // ACT & ASSERT
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("Exercise wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenUserDoesNotOwnWorkout_ShouldThrowException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "another-user-id" }; // Owned by someone else
            var exercise = new ExerciseEntity { Id = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity> { exercise });

            // ACT & ASSERT
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("This workout doens't belong to you");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseAlreadyExists_ShouldThrowException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exercise = new ExerciseEntity { Id = 1 };
            var existingLink = new WorkoutExercise { WorkoutId = 1, ExerciseId = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity> { exercise });
            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                .ReturnsAsync(new List<WorkoutExercise> { existingLink }); // Simulate link already exists

            // ACT & ASSERT
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("This exercise already belong to this workout");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenWorkoutNotFound_ThrowsException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 99 };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity>());

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("Workout wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseNotFound_ThrowsException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 99 };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { new CustomWorkout() });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity>());

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("Exercise wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenUnauthorizedUser_ThrowsException()
        {
            // ARRANGE
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "other-user-id" };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { new ExerciseEntity() });

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("This workout doens't belong to you");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenAdmin_IgnoresOwnership()
        {
            // ARRANGE - Setup admin user
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "admin-id"),
        new Claim(ClaimTypes.Role, "Admin"),
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "other-user-id" };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { new ExerciseEntity() });
            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .ReturnsAsync(new List<WorkoutExercise>());

            // ACT
            await _workoutService.AddToWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(repo => repo.InsertAsync(It.IsAny<WorkoutExercise>(), It.IsAny<bool>()), Times.Once);
        }


        #endregion


        #region DeleteFromWorkout Tests

        [TestMethod]
        public async Task DeleteFromWorkout_WhenUserOwnsWorkout_ShouldDeleteSuccessfully()
        {
            // ARRANGE
            var dto = new DeleteFromWorkoutDTO { Id = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var entityToDelete = new WorkoutExercise { Id = 1, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToDelete });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            // ACT
            await _workoutService.DeleteFromWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(r => r.RemoveAsync(entityToDelete, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenLinkNotFound_ShouldThrowKeyNotFoundException()
        {
            // ARRANGE
            var dto = new DeleteFromWorkoutDTO { Id = 99 };
            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()); // Empty list
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.DeleteFromWorkout(dto));
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenUserDoesNotOwnWorkout_ShouldThrowUnauthorizedAccessException()
        {
            // ARRANGE
            var dto = new DeleteFromWorkoutDTO { Id = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "another-user-id" }; // Different owner
            var entityToDelete = new WorkoutExercise { Id = 1, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToDelete });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => _workoutService.DeleteFromWorkout(dto));
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenEntryNotFound_ThrowsException()
        {
            // ARRANGE
            var dto = new DeleteFromWorkoutDTO { Id = 99 };
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()));

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.DeleteFromWorkout(dto));
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenUnauthorizedUser_ThrowsException()
        {
            // ARRANGE
            var workoutExercise = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "other-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(workoutExercises);

            var dto = new DeleteFromWorkoutDTO { Id = 1 };

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => _workoutService.DeleteFromWorkout(dto));
            exception.Message.Should().Be("This workout does not belong to you.");
        }


        #endregion


        #region UpdateExerciseInWorkout Tests

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenUserOwnsWorkout_ShouldUpdateSuccessfully()
        {
            // ARRANGE
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1, Sets = 5, Reps = 5 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var entityToUpdate = new WorkoutExercise { Id = 1, Sets = 3, Reps = 3, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToUpdate });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            // ACT
            await _workoutService.UpdateExerciseInWorkout(dto);

            // ASSERT
            _mockWorkoutExerciseRepository.Verify(r => r.UpdateAsync(It.Is<WorkoutExercise>(we => we.Sets == 5 && we.Reps == 5), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenEntryNotFound_ShouldThrowKeyNotFoundException()
        {
            // ARRANGE
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 99 };
            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()); // Empty list
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.UpdateExerciseInWorkout(dto));
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenUserDoesNotOwnWorkout_ShouldThrowException()
        {
            // ARRANGE
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "another-user-id" }; // Different owner
            var entityToUpdate = new WorkoutExercise { Id = 1, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToUpdate });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            // ACT & ASSERT
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.UpdateExerciseInWorkout(dto));
            ex.Message.Should().Be("This workout doesn't belong to you.");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenEntryNotFound_ThrowsException()
        {
            // ARRANGE
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 99 };
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()));

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.UpdateExerciseInWorkout(dto));
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenUnauthorizedUser_ThrowsException()
        {
            // ARRANGE
            var workoutExercise = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "other-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(workoutExercises);

            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1 };

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.UpdateExerciseInWorkout(dto));
            exception.Message.Should().Be("This workout doesn't belong to you.");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenAdmin_IgnoresOwnership()
        {
            // ARRANGE - Setup admin user
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "admin-id"),
        new Claim(ClaimTypes.Role, "Admin"),
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var workoutExercise = new WorkoutExercise
            {
                Id = 1,
                Sets = 3,
                Reps = 10,
                Workout = new CustomWorkout { UserId = "other-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(workoutExercises);

            var dto = new UpdateWorkoutExerciseDTO
            {
                WorkoutExerciseId = 1,
                Sets = 4,
                Reps = 12
            };

            // ACT
            await _workoutService.UpdateExerciseInWorkout(dto);

            // ASSERT
            Assert.AreEqual(4, workoutExercise.Sets);
            Assert.AreEqual(12, workoutExercise.Reps);
            _mockWorkoutExerciseRepository.Verify(repo => repo.UpdateAsync(workoutExercise, It.IsAny<bool>()), Times.Once);
        }

        #endregion

    }
}