using API.Controllers.Common;
using Application.Common;
using Application.DTOs.Action;
using Application.DTOs.Category;
using Application.DTOs.Common;
using Application.IAppServices.Authentication;
using Application.IAppServices.Category;
using Application.Serializer;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = ApiConsts.AdminRoleName)]
    public class CategoryController : BaseAuthenticatedController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ICategoryService categoryService,
            IAuthenticationService authenticationService,
            IJsonFieldsSerializer jsonFieldsSerializer) : base(authenticationService, jsonFieldsSerializer) 
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetCategoryDTO dto)
        {
            var categories = await _categoryService.GetAllAsync(dto);
            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "", StatusCodes.Status200OK, categories),
                    string.Empty));
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery] BaseDTO<int> dto)
        {
            var category = await _categoryService.GetByIdAsync(dto.Id);
            if (category == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "Category not found", StatusCodes.Status404NotFound),
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
        public async Task<IActionResult> Insert(CreateCategoryDTO createCategoryDto)
        {
            var result = await _categoryService.CreateAsync(createCategoryDto);
            if (result == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "failed", StatusCodes.Status400BadRequest),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Category created successfully", StatusCodes.Status201Created),
                    string.Empty));
        }


        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(UpdateCategoryDTO updateCategoryDto)
        {
            var result = await _categoryService.UpdateAsync(updateCategoryDto);
            if (result == null)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, "failed", StatusCodes.Status400BadRequest),
                        string.Empty));
            }

            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Category updated successfully", StatusCodes.Status200OK),
                    string.Empty));
        }


        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(BaseDTO<int> dto)
        {
            try
            {
                await _categoryService.DeleteAsync(dto.Id);
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(true, "Category deleted successfully", StatusCodes.Status200OK),
                        string.Empty));
            }
            catch (KeyNotFoundException ex)
            {
                return new RawJsonActionResult(
                    _jsonFieldsSerializer.Serialize(
                        new ApiResponse(false, ex.Message, StatusCodes.Status404NotFound),
                        string.Empty));
            }
        }
    }
}
