using API.Controllers.Common;
using Application.Common;
using Application.DTOs.Action;
using Application.DTOs.Common;
using Application.DTOs.Exercise;
using Application.DTOs.Workout;
using Application.IAppServices.Authentication;
using Application.IAppServices.Workout;
using Application.Serializer;
using Domain.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class WorkoutController : BaseAuthenticatedController
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController(IAuthenticationService authenticationService,
            IJsonFieldsSerializer jsonFieldsSerializer, IWorkoutService workoutService) : base(authenticationService,jsonFieldsSerializer)
        {
            _workoutService = workoutService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToWorkout(AddtoWorkoutDTO dto)
        {
            await _workoutService.AddToWorkout(dto);

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Exercise added successfully", StatusCodes.Status200OK),
                    string.Empty));
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateExerciseInWorkout(UpdateWorkoutExerciseDTO dto)
        {
            try
            {
                await _workoutService.UpdateExerciseInWorkout(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, ex.Message, StatusCodes.Status404NotFound),
                        string.Empty));
            }
            catch (Exception ex)
            {
                return new RawJsonActionResult(
                   _jsonFieldsSerializer.Serialize(
                       new ApiResponse(false, ex.Message, StatusCodes.Status400BadRequest),
                       string.Empty));
            }
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFromWorkout (WorkoutExerciseDTO dto)
        {
            try
            {
                await _workoutService.DeleteFromWorkout(dto);
                return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Exercise deleted successfully", StatusCodes.Status204NoContent),
                    string.Empty));
            }
            catch (KeyNotFoundException ex)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, ex.Message, StatusCodes.Status404NotFound),
                        string.Empty));
            }
            catch (Exception ex)
            {
                return new RawJsonActionResult(
                  _jsonFieldsSerializer.Serialize(
                      new ApiResponse(false, ex.Message, StatusCodes.Status400BadRequest),
                      string.Empty));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<WorkoutDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetWorkoutDTO dto)
        {
            var categories = await _workoutService.GetAllAsync(dto);
            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "", StatusCodes.Status200OK, categories),
                    string.Empty));
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<WorkoutDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery] BaseDTO<int> dto)
        {
            var category = await _workoutService.GetByIdAsync(dto.Id);
            if (category == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "Workout not found", StatusCodes.Status404NotFound),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "", StatusCodes.Status200OK, category),
                    string.Empty));
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert(CreateWorkoutDTO createExerciseDto)
        {
            var result = await _workoutService.CreateAsync(createExerciseDto);
            if (result == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "failed", StatusCodes.Status400BadRequest),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Workout created successfully", StatusCodes.Status201Created),
                    string.Empty));
        }


        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(UpdateWorkoutDTO updateExerciseDto)
        {
            var result = await _workoutService.UpdateAsync(updateExerciseDto);
            if (result == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "failed", StatusCodes.Status400BadRequest),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Workout updated successfully", StatusCodes.Status200OK),
                    string.Empty));
        }


        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(BaseDTO<int> dto)
        {
            await _workoutService.DeleteAsync(dto.Id);

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Workout deleted successfully", StatusCodes.Status200OK),
                    string.Empty));
        }
    }
}
