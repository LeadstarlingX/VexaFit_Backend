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
            // ARRANGE
            var workoutId = 1;
            var userId = "test-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Test Workout", UserId = userId };
            var expectedDto = new WorkoutDTO { Id = workoutId, Name = "Test Workout" };

            // **THE FIX:** Use the helper to create an async-compatible queryable
            var workouts = MockDb.CreateAsyncQueryable(new List<WorkoutEntity> { workoutEntity });

            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()
            )).Returns(workouts);

            _mockMapper.Setup(m => m.Map<WorkoutDTO>(workoutEntity)).Returns(expectedDto);

            // ACT
            var result = await _workoutService.GetByIdAsync(workoutId);

            // ASSERT
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
        }

        [TestMethod()]
        public async Task GetByIdAsync_WhenWorkoutDoesNotExist_ShouldThrowException()
        {
            // ARRANGE
            var workoutId = 99;

            // **THE FIX:** Use the helper here as well
            var emptyList = MockDb.CreateAsyncQueryable(new List<WorkoutEntity>());

            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()
            )).Returns(emptyList);

            // ACT & ASSERT
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                async () => await _workoutService.GetByIdAsync(workoutId)
            );
            exception.Message.Should().Be("Workout not found");
        }

        [TestMethod]
        public async Task CreateAsync_WhenCalled_AssignsCorrectUserIdAndSaves()
        {
            // ARRANGE
            var createDto = new CreateWorkoutDTO { Name = "New Workout", Description = "A test." };
            var userId = "test-user-id"; // This comes from the mocked user in TestSetup

            var workoutEntity = new CustomWorkout { Name = createDto.Name, Description = createDto.Description };

            // **THE FIX:** Tell the mock mapper what to do when it's called.
            // When Map is called with the createDto, return our workoutEntity.
            _mockMapper.Setup(m => m.Map<CustomWorkout>(createDto)).Returns(workoutEntity);



            // ACT
            await _workoutService.CreateAsync(createDto);

            // ASSERT
            // Verify that the repository's InsertAsync method was called exactly once
            // and that the entity passed to it has the correct UserId assigned.
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
            // ARRANGE
            var workoutId = 1;
            var userId = "test-user-id";
            var updateDto = new UpdateWorkoutDTO { Id = workoutId, Name = "Updated Name" };
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Original Name", UserId = userId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            // **THE FIX:** Setup the mock mapper to simulate the update.
            // When Map is called with any DTO and our entity, perform an action.
            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateWorkoutDTO>(), It.IsAny<WorkoutEntity>()))
                       .Callback<UpdateWorkoutDTO, WorkoutEntity>((dto, entity) =>
                       {
                           // This simulates what AutoMapper does: updates the entity's properties.
                           entity.Name = dto.Name;
                       });

            // ACT
            await _workoutService.UpdateAsync(updateDto);

            // ASSERT
            // Now your original verification will pass, because the Callback updated the entity's name.
            _mockWorkoutRepository.Verify(
                repo => repo.UpdateAsync(It.Is<WorkoutEntity>(w => w.Name == "Updated Name"), It.IsAny<bool>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task UpdateAsync_WhenUserDoesNotOwnWorkout_ShouldThrowUnauthorizedAccessException()
        {
            // ARRANGE
            var workoutId = 2;
            var ownerUserId = "another-user-id"; // A different user
            var updateDto = new UpdateWorkoutDTO { Id = workoutId, Name = "Updated Name" };
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Original Name", UserId = ownerUserId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
                () => _workoutService.UpdateAsync(updateDto)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenUserOwnsWorkout_ShouldDeleteSuccessfully()
        {
            // ARRANGE
            var workoutId = 1;
            var userId = "test-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, UserId = userId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                 .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            // ACT
            await _workoutService.DeleteAsync(workoutId);

            // ASSERT
            _mockWorkoutRepository.Verify(repo => repo.RemoveAsync(workoutEntity, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenUserDoesNotOwnWorkout_ShouldThrowException()
        {
            // ARRANGE
            var workoutId = 2;
            var ownerUserId = "another-user-id";
            var workoutEntity = new CustomWorkout { Id = workoutId, UserId = ownerUserId };

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<Exception>(
                () => _workoutService.DeleteAsync(workoutId)
            );
        }


        [TestMethod]
        public async Task UpdateAsync_WhenUserIsAdmin_ShouldUpdateAnyWorkout()
        {
            // ARRANGE
            var workoutId = 2;
            var ownerUserId = "another-user-id"; // A workout owned by someone else
            var updateDto = new UpdateWorkoutDTO { Id = workoutId, Name = "Admin Updated Name" };
            var workoutEntity = new CustomWorkout { Id = workoutId, Name = "Original Name", UserId = ownerUserId };

            // Create a new ClaimsPrincipal specifically for an Admin user for this test
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "admin-user-id"),
        new Claim(ClaimTypes.Role, "Admin"), // The important part
            }, "mock"));

            // Setup the HttpContextAccessor to return the Admin user
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity> { workoutEntity });

            // ACT
            await _workoutService.UpdateAsync(updateDto);

            // ASSERT
            // Verify the update was called, proving the admin check was successful
            _mockWorkoutRepository.Verify(
                repo => repo.UpdateAsync(workoutEntity, It.IsAny<bool>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenUserIsAdmin_ShouldDeleteAnyWorkout()
        {
            // ARRANGE
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

            // ACT
            await _workoutService.DeleteAsync(workoutId);

            // ASSERT
            _mockWorkoutRepository.Verify(repo => repo.RemoveAsync(workoutEntity, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenWorkoutNotFound_ShouldThrowKeyNotFoundException()
        {
            // ARRANGE
            var nonExistentWorkoutId = 999;
            var updateDto = new UpdateWorkoutDTO { Id = nonExistentWorkoutId, Name = "Doesn't Matter" };

            // Setup FindAsync to return an empty list, simulating the workout not being found.
            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity>());

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _workoutService.UpdateAsync(updateDto)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenWorkoutNotFound_ShouldThrowKeyNotFoundException()
        {
            // ARRANGE
            var nonExistentWorkoutId = 999;

            // Setup FindAsync to return an empty list.
            _mockWorkoutRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                                  .ReturnsAsync(new List<WorkoutEntity>());

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _workoutService.DeleteAsync(nonExistentWorkoutId)
            );
        }


        [TestMethod]
        public async Task UpdateAsync_WithPredefinedWorkoutAsAdmin_ShouldUpdateSuccessfully()
        {
            // Arrange
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

            // Act
            await _workoutService.UpdateAsync(dto);

            // Assert
            _mockWorkoutRepository.Verify(repo =>
                repo.UpdateAsync(It.Is<WorkoutEntity>(w => w.Name == "Updated Predefined"),
                It.IsAny<bool>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenWorkoutIsPredefined_ShouldReturnForNonAdmin()
        {
            // Arrange
            var workoutId = 1;
            var workout = new PredefinedWorkout { Id = workoutId, Name = "Beginner Routine" };
            var expectedDto = new WorkoutDTO { Id = workoutId, Name = "Beginner Routine" };

            var workouts = MockDb.CreateAsyncQueryable(new List<WorkoutEntity> { workout });
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(workouts);

            _mockMapper.Setup(m => m.Map<WorkoutDTO>(workout)).Returns(expectedDto);

            // Act
            var result = await _workoutService.GetByIdAsync(workoutId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(workoutId);
            result.Name.Should().Be("Beginner Routine");
        }


        [TestMethod]
        public async Task UpdateAsync_WhenDtoIsValid_ShouldReturnUpdatedDTO()
        {
            // Arrange
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

            // Act
            var result = await _workoutService.UpdateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated");
        }

        [TestMethod]
        public async Task CreateAsync_WhenDtoIsValid_ShouldReturnMappedDTO()
        {
            // Arrange
            var createDto = new CreateWorkoutDTO { Name = "New Workout" };
            var createdWorkout = new CustomWorkout { Id = 1, Name = "New Workout", UserId = "test-user-id" };
            var expectedDto = new WorkoutDTO { Id = 1, Name = "New Workout" };

            _mockMapper.Setup(m => m.Map<CustomWorkout>(createDto)).Returns(createdWorkout);
            _mockMapper.Setup(m => m.Map<WorkoutDTO>(createdWorkout)).Returns(expectedDto);

            // Act
            var result = await _workoutService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Workout");
        }

        #endregion


        #region Query and Filter Tests

        [TestMethod]
        public async Task GetAllAsync_WithDescriptionFilter_ShouldReturnFilteredResults()
        {
            // Arrange
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

            // Act
            var result = await _workoutService.GetAllAsync(dto);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(d => d.Description.Contains("cardio"));
            result.Should().NotContain(d => d.Description == "Strength training");
        }


        [TestMethod]
        public async Task GetAllAsync_WhenNoWorkoutsMatch_ShouldReturnEmptyList()
        {
            // Arrange
            var dto = new GetWorkoutDTO { Name = "NonExistentWorkout" };
            var workouts = new List<WorkoutEntity>(); // Empty dataset

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(queryable);

            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns(Enumerable.Empty<WorkoutDTO>());

            // Act
            var result = await _workoutService.GetAllAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllAsync_WithUserIdFilterForNonAdmin_ShouldIgnoreFilter()
        {
            // Arrange
            var dto = new GetWorkoutDTO { UserId = "other-user" }; // Should be ignored
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

            // Act
            var result = await _workoutService.GetAllAsync(dto);

            // Assert
            result.Should().HaveCount(2); // Only "Mine" and "Shared"
            result.Should().Contain(d => d.Name == "Mine");
            result.Should().Contain(d => d.Name == "Shared");
            result.Should().NotContain(d => d.Name == "Theirs");
        }

        [TestMethod]
        public async Task GetAllAsync_WithNameFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var dto = new GetWorkoutDTO { Name = "Cardio" };
            var workouts = new List<WorkoutEntity>
                {   new CustomWorkout { Id = 1, Name = "Morning Cardio", UserId = "test-user-id" },
                    new CustomWorkout { Id = 2, Name = "Strength", UserId = "test-user-id" } };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            // THE FIX: The mapper must be set up to translate the filtered entities into DTOs.
            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                       .Returns((IEnumerable<WorkoutEntity> entities) =>
                           entities.Select(e => new WorkoutDTO { Id = e.Id, Name = e.Name })
                       );

            // Act
            var result = await _workoutService.GetAllAsync(dto);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Morning Cardio");
        }

        [TestMethod]
        public async Task GetAllAsync_WithPredefinedFilter_ShouldReturnOnlyPredefined()
        {
            // Arrange
            var dto = new GetWorkoutDTO { Discriminator = WorkoutEnum.Predefined };
            var workouts = new List<WorkoutEntity>
                {   new PredefinedWorkout { Id = 1, Name = "Beginner" },
                    new CustomWorkout { Id = 2, Name = "Custom", UserId = "test-user-id" } };

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            // THE FIX: The mapper must be set up to translate the filtered entities into DTOs.
            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                       .Returns((IEnumerable<WorkoutEntity> entities) =>
                           entities.Select(e => new WorkoutDTO { Id = e.Id, Name = e.Name })
                       );

            // Act
            var result = await _workoutService.GetAllAsync(dto);

            // Assert
            result.Should().HaveCount(1);
            result.First().Should().BeOfType<WorkoutDTO>();
        }

        [TestMethod]
        public async Task GetAllAsync_WithAdminUser_ShouldReturnAllWorkouts()
        {
            // Arrange
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

            // ### THE FIX IS HERE ###
            // Create an async-compatible queryable using the same helper from other tests.
            var asyncQueryableWorkouts = MockDb.CreateAsyncQueryable(workouts);

            // The service calls GetAll() and then applies Includes. We just need to return the
            // async-compatible queryable from the GetAll() mock.
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                    It.IsAny<bool>(),
                    It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(asyncQueryableWorkouts);

            // Setup mapper to return DTOs
            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns((IEnumerable<WorkoutEntity> entities) =>
                    entities.Select(e => new WorkoutDTO { Id = e.Id }));

            // Act
            var result = await _workoutService.GetAllAsync(new GetWorkoutDTO());

            // Assert
            result.Should().HaveCount(3);
        }

        #endregion


        #region Security and Authorization Tests

        [TestMethod]
        public async Task CreateAsync_WhenUserNotAuthenticated_ShouldThrow()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            var createDto = new CreateWorkoutDTO { Name = "Should Fail" };

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _workoutService.CreateAsync(createDto)
            );
        }

        [TestMethod]
        public async Task GetByIdAsync_UnauthorizedAccessAttempt_ShouldThrowNotFoundException()
        {
            // ARRANGE
            var workoutIdToAccess = 5;
            var ownerUserId = "another-user-id";
            var workoutOfOtherUser = new CustomWorkout { Id = workoutIdToAccess, Name = "Secret Workout", UserId = ownerUserId };

            // The repository will find this workout initially
            var workoutsInDb = MockDb.CreateAsyncQueryable(new List<WorkoutEntity> { workoutOfOtherUser });
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()
            )).Returns(workoutsInDb);

            // ACT & ASSERT
            // The service's security filter should remove the workout from the results
            // before attempting to find it by ID, resulting in a "not found" exception.
            var ex = await Assert.ThrowsExceptionAsync<Exception>(
                () => _workoutService.GetByIdAsync(workoutIdToAccess)
            );
            ex.Message.Should().Be("Workout not found");
        }

        [TestMethod]
        public async Task UpdateAsync_WithPredefinedWorkoutAsNonAdmin_ShouldThrow()
        {
            // Arrange
            var dto = new UpdateWorkoutDTO { Id = 1 };
            var workout = new PredefinedWorkout { Id = 1 };

            // THE FIX: Explicitly provide arguments for all parameters, even optional ones.
            _mockWorkoutRepository.Setup(repo =>
                    repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _workoutService.UpdateAsync(dto));
        }

        [TestMethod]
        public async Task DeleteAsync_WithPredefinedWorkoutAsAdmin_ShouldSucceed()
        {
            // Arrange
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            { new Claim(ClaimTypes.Role, "Admin")
                    }, "mock")); // Added authentication type to constructor
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var workout = new PredefinedWorkout { Id = 1 };

            // THE FIX: Explicitly provide arguments for all parameters, even optional ones.
            _mockWorkoutRepository.Setup(repo =>
                    repo.FindAsync(It.IsAny<Expression<Func<WorkoutEntity, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity> { workout });

            // Act
            await _workoutService.DeleteAsync(1);

            // Assert
            _mockWorkoutRepository.Verify(repo =>
                repo.RemoveAsync(It.Is<WorkoutEntity>(w => w.Id == 1), It.IsAny<bool>()), // Made verification more specific
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_ForNonAdmin_ShouldReturnOnlyOwnedAndPredefinedWorkouts()
        {
            // Arrange
            // Create all mocks locally to ensure complete test isolation.
            var mockWorkoutRepository = new Mock<IAppRepository<WorkoutEntity>>();
            var mockMapper = new Mock<IMapper>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Define a non-admin user specifically for this test.
            var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        new Claim(ClaimTypes.Role, "Trainee"),
            }, "mock"));

            // Setup the local HttpContextAccessor mock with the non-admin user.
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = nonAdminUser });

            // Instantiate the service with the local, correctly-configured mocks.
            var workoutService = new WorkoutService(
                mockWorkoutRepository.Object,
                _mockWorkoutExerciseRepository.Object, // This mock can be reused from TestSetup as it's not involved in the failure
                _mockExerciseRepository.Object,      // This mock can be reused from TestSetup as it's not involved in the failure
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

            // Act
            var result = await workoutService.GetAllAsync(new GetWorkoutDTO());

            // Assert
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
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _workoutService.CreateAsync(null));
        }

        [TestMethod]
        public async Task DeleteBulkAsync_WithEmptyList_ShouldNotCallRepository()
        {
            // Act
            await _workoutService.DeleteBulkAsync(new List<int>());

            // Assert
            _mockWorkoutRepository.Verify(repo =>
                repo.BulkRemoveAsync(It.IsAny<IEnumerable<int>>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithInvalidId_ShouldThrowProperException()
        {
            // Arrange
            _mockWorkoutRepository.Setup(repo => repo.GetAll(
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutEntity>().AsQueryable()));

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _workoutService.GetByIdAsync(999));
            ex.Message.Should().Be("Workout not found");
        }

        [TestMethod]
        public async Task AddToWorkout_WithNonExistentWorkout_ShouldThrow()
        {
            // Arrange
            var dto = new AddtoWorkoutDTO { workoutId = 99 };

            // THE FIX: Provide arguments for all parameters in FindAsync, including the optional ones.
            _mockWorkoutRepository.Setup(repo =>
                    repo.FindAsync(
                        It.IsAny<Expression<Func<WorkoutEntity, bool>>>(),
                        It.IsAny<bool>(),
                        It.IsAny<Expression<Func<WorkoutEntity, object>>[]>()))
                .ReturnsAsync(new List<WorkoutEntity>());

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _workoutService.AddToWorkout(dto));
            ex.Message.Should().Be("Workout wasn't found"); // Making the assertion more specific is good practice
        }

        [TestMethod]
        public async Task UpdateExerciseInWorkout_WithValidData_ShouldUpdateProperties()
        {
            // Arrange
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

            // THE FIX: The .Include() method call must be removed from the mock setup expression.
            // First, set up the GetAll() call to return an async-compatible queryable.
            _mockWorkoutExerciseRepository.Setup(repo =>
                    repo.GetAll(
                        It.IsAny<bool>(),
                        It.IsAny<Expression<Func<WorkoutExercise, object>>[]>()))
                .Returns(MockDb.CreateAsyncQueryable(new List<WorkoutExercise> { workoutExercise }));

            // The service will then call .Include() and .FirstOrDefaultAsync() on the returned queryable.

            // Act
            await _workoutService.UpdateExerciseInWorkout(dto);

            // Assert
            workoutExercise.Sets.Should().Be(4);
            workoutExercise.Reps.Should().Be(12);
            workoutExercise.WeightKg.Should().Be(50);
            _mockWorkoutExerciseRepository.Verify(repo =>
                repo.UpdateAsync(It.Is<WorkoutExercise>(we => we.Id == 1), It.IsAny<bool>()), // Made verification more specific
                Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_WithFullFilter_ShouldApplyAllFilters()
        {
            // Arrange
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

            // Setup an admin user to enable the UserId filter
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = adminUser });

            var queryable = MockDb.CreateAsyncQueryable(workouts.AsQueryable());
            _mockWorkoutRepository.Setup(repo => repo.GetAll(It.IsAny<bool>(), It.IsAny<Expression<Func<WorkoutEntity, object>>[]>())).Returns(queryable);

            // THE FIX: The mapper must be configured to transform the filtered entity into a DTO.
            _mockMapper.Setup(m => m.Map<IEnumerable<WorkoutDTO>>(It.IsAny<IEnumerable<WorkoutEntity>>()))
                .Returns((IEnumerable<WorkoutEntity> entities) =>
                    // This simulates the mapping from the entity to the DTO
                    entities.Select(e => new WorkoutDTO { Name = e.Name })
                );

            // Act
            var result = await _workoutService.GetAllAsync(dto);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Morning Routine");
        }

        #endregion



    }
}