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
    }
}