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


        #region AddToWorkout Tests

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseIsNotPresent_ShouldAddSuccessfully()
        {
            
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1, Sets = 3, Reps = 10 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exerciseEntity = new ExerciseEntity { Id = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { exerciseEntity });

            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .ReturnsAsync(new List<WorkoutExercise>()); 

            await _workoutService.AddToWorkout(dto);

           
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
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exerciseEntity = new ExerciseEntity { Id = 1 };
            var existingWorkoutExercise = new WorkoutExercise { WorkoutId = 1, ExerciseId = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { exerciseEntity });

            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .ReturnsAsync(new List<WorkoutExercise> { existingWorkoutExercise });

            
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("This exercise already belong to this workout");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenEntryExistsAndUserIsOwner_ShouldUpdateSuccessfully()
        {
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

            await _workoutService.UpdateExerciseInWorkout(dto);

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
            var dto = new DeleteFromWorkoutDTO { Id = 1 };
            var workoutExerciseEntity = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "test-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExerciseEntity });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                         .Returns(workoutExercises);

            await _workoutService.DeleteFromWorkout(dto);

            _mockWorkoutExerciseRepository.Verify(repo => repo.RemoveAsync(workoutExerciseEntity, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task AddToWorkout_WhenDataIsValid_ShouldInsertSuccessfully()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exercise = new ExerciseEntity { Id = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity> { exercise });
            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                .ReturnsAsync(new List<WorkoutExercise>());

            await _workoutService.AddToWorkout(dto);

            _mockWorkoutExerciseRepository.Verify(r => r.InsertAsync(It.Is<WorkoutExercise>(we => we.WorkoutId == 1 && we.ExerciseId == 1), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task AddToWorkout_WhenWorkoutNotFound_ShouldThrowException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 99, exerciseId = 1 };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity>()); 

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("Workout wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseNotFound_ShouldThrowException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 99 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity>()); 

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("Exercise wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenUserDoesNotOwnWorkout_ShouldThrowException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "another-user-id" }; 
            var exercise = new ExerciseEntity { Id = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity> { exercise });

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("This workout doens't belong to you");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseAlreadyExists_ShouldThrowException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var exercise = new ExerciseEntity { Id = 1 };
            var existingLink = new WorkoutExercise { WorkoutId = 1, ExerciseId = 1 };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                .ReturnsAsync(new List<ExerciseEntity> { exercise });
            _mockWorkoutExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutExercise, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                .ReturnsAsync(new List<WorkoutExercise> { existingLink });

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("This exercise already belong to this workout");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenWorkoutNotFound_ThrowsException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 99 };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity>());

            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("Workout wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenExerciseNotFound_ThrowsException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 99 };
            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { new CustomWorkout() });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity>());

            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("Exercise wasn't found");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenUnauthorizedUser_ThrowsException()
        {
            var dto = new AddtoWorkoutDTO { workoutId = 1, exerciseId = 1 };
            var workoutEntity = new CustomWorkout { Id = 1, UserId = "other-user-id" };

            _mockWorkoutRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });
            _mockExerciseRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ExerciseEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ExerciseEntity, object>>[]>()))
                                   .ReturnsAsync(new List<ExerciseEntity> { new ExerciseEntity() });

            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.AddToWorkout(dto));
            exception.Message.Should().Be("This workout doens't belong to you");
        }

        [TestMethod]
        public async Task AddToWorkout_WhenAdmin_IgnoresOwnership()
        {
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

            await _workoutService.AddToWorkout(dto);

            _mockWorkoutExerciseRepository.Verify(repo => repo.InsertAsync(It.IsAny<WorkoutExercise>(), It.IsAny<bool>()), Times.Once);
        }


        #endregion


        #region DeleteFromWorkout Tests

        [TestMethod]
        public async Task DeleteFromWorkout_WhenUserOwnsWorkout_ShouldDeleteSuccessfully()
        {
            var dto = new DeleteFromWorkoutDTO { Id = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var entityToDelete = new WorkoutExercise { Id = 1, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToDelete });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            await _workoutService.DeleteFromWorkout(dto);

            _mockWorkoutExerciseRepository.Verify(r => r.RemoveAsync(entityToDelete, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenLinkNotFound_ShouldThrowKeyNotFoundException()
        {
            var dto = new DeleteFromWorkoutDTO { Id = 99 };
            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()); // Empty list
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.DeleteFromWorkout(dto));
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenUserDoesNotOwnWorkout_ShouldThrowUnauthorizedAccessException()
        {
            var dto = new DeleteFromWorkoutDTO { Id = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "another-user-id" }; // Different owner
            var entityToDelete = new WorkoutExercise { Id = 1, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToDelete });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => _workoutService.DeleteFromWorkout(dto));
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenEntryNotFound_ThrowsException()
        {
            var dto = new DeleteFromWorkoutDTO { Id = 99 };
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()));

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.DeleteFromWorkout(dto));
        }

        [TestMethod]
        public async Task DeleteFromWorkout_WhenUnauthorizedUser_ThrowsException()
        {
            var workoutExercise = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "other-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(workoutExercises);

            var dto = new DeleteFromWorkoutDTO { Id = 1 };

            var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => _workoutService.DeleteFromWorkout(dto));
            exception.Message.Should().Be("This workout does not belong to you.");
        }


        #endregion


        #region UpdateExerciseInWorkout Tests

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenUserOwnsWorkout_ShouldUpdateSuccessfully()
        {
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1, Sets = 5, Reps = 5 };
            var workout = new CustomWorkout { Id = 1, UserId = "test-user-id" };
            var entityToUpdate = new WorkoutExercise { Id = 1, Sets = 3, Reps = 3, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToUpdate });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            await _workoutService.UpdateExerciseInWorkout(dto);

            _mockWorkoutExerciseRepository.Verify(r => r.UpdateAsync(It.Is<WorkoutExercise>(we => we.Sets == 5 && we.Reps == 5), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenEntryNotFound_ShouldThrowKeyNotFoundException()
        {
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 99 };
            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()); // Empty list
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.UpdateExerciseInWorkout(dto));
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenUserDoesNotOwnWorkout_ShouldThrowException()
        {
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1 };
            var workout = new CustomWorkout { Id = 1, UserId = "another-user-id" }; // Different owner
            var entityToUpdate = new WorkoutExercise { Id = 1, Workout = workout };

            var queryable = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { entityToUpdate });
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>())).Returns(queryable);

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.UpdateExerciseInWorkout(dto));
            ex.Message.Should().Be("This workout doesn't belong to you.");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenEntryNotFound_ThrowsException()
        {
            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 99 };
            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutExercise>()));

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _workoutService.UpdateExerciseInWorkout(dto));
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenUnauthorizedUser_ThrowsException()
        {
            var workoutExercise = new WorkoutExercise
            {
                Id = 1,
                Workout = new CustomWorkout { UserId = "other-user-id" }
            };
            var workoutExercises = MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise });

            _mockWorkoutExerciseRepository.Setup(r => r.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                                          .Returns(workoutExercises);

            var dto = new UpdateWorkoutExerciseDTO { WorkoutExerciseId = 1 };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _workoutService.UpdateExerciseInWorkout(dto));
            exception.Message.Should().Be("This workout doesn't belong to you.");
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WhenAdmin_IgnoresOwnership()
        {
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

            await _workoutService.UpdateExerciseInWorkout(dto);

            Assert.AreEqual(4, workoutExercise.Sets);
            Assert.AreEqual(12, workoutExercise.Reps);
            _mockWorkoutExerciseRepository.Verify(repo => repo.UpdateAsync(workoutExercise, It.IsAny<bool>()), Times.Once);
        }

        #endregion

    }
}