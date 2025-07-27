using API.Controllers.Common;
using Application.Common;
using Application.DTOs.Action;
using Application.DTOs.Category;
using Application.DTOs.Common;
using Application.DTOs.Exercise;
using Application.IAppServices.Authentication;
using Application.IAppServices.Exercise;
using Application.Serializer;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ExerciseController : BaseAuthenticatedController
    {
        private readonly IExerciseService _exerciseService;

        public ExerciseController(IAuthenticationService authenticationService,
            IJsonFieldsSerializer jsonFieldsSerializer, IExerciseService exerciseService) : base(authenticationService, jsonFieldsSerializer)
        {
            _exerciseService = exerciseService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ExerciseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetExerciseDTO dto)
        {
            var categories = await _exerciseService.GetAllAsync(dto);
            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "", StatusCodes.Status200OK, categories),
                    string.Empty));
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ExerciseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery] BaseDTO<int> dto)
        {
            var category = await _exerciseService.GetByIdAsync(dto.Id);
            if (category == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "Exercise not found", StatusCodes.Status404NotFound),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "", StatusCodes.Status200OK, category),
                    string.Empty));
        }


        [Authorize(Roles = ApiConsts.AdminRoleName)]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert([FromForm]CreateExerciseDTO createExerciseDto)
        {
            var result = await _exerciseService.CreateAsync(createExerciseDto);
            if (result == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "failed", StatusCodes.Status400BadRequest),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Exercise created successfully", StatusCodes.Status201Created),
                    string.Empty));
        }


        [Authorize(Roles = ApiConsts.AdminRoleName)]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(UpdateExerciseDTO updateExerciseDto)
        {
            var result = await _exerciseService.UpdateAsync(updateExerciseDto);
            if (result == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "failed", StatusCodes.Status400BadRequest),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Exercise updated successfully", StatusCodes.Status200OK),
                    string.Empty));
        }


        [Authorize(Roles = ApiConsts.AdminRoleName)]
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(BaseDTO<int> dto)
        {
            await _exerciseService.DeleteAsync(dto.Id);

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Exercise deleted successfully", StatusCodes.Status200OK),
                    string.Empty));
        }

    }
}
