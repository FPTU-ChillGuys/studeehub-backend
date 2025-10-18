using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PomodoroSetting;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;

namespace studeehub.Application.Services
{
	public class PomodoroSettingService : IPomodoroSettingService
	{
		private readonly IGenericRepository<PomodoroSetting> _repository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly IValidator<UpdateSettingRequest> _validator;

		public PomodoroSettingService(IGenericRepository<PomodoroSetting> repository, IMapper mapper, IValidator<UpdateSettingRequest> validator, IUserRepository userRepository)
		{
			_repository = repository;
			_mapper = mapper;
			_validator = validator;
			_userRepository = userRepository;
		}

		public async Task<PomodoroSetting> GetForUserAsync(Guid userId)
		{
			var setting = await _repository.GetByConditionAsync(s => s.UserId == userId) ?? new PomodoroSetting { UserId = userId };
			return setting; // default if not yet set
		}

		public async Task<BaseResponse<GetSettingResponse>> GetSettingByUserIdAsync(Guid userId)
		{
			var setting = await _repository.GetByConditionAsync(s => s.UserId == userId) ?? new PomodoroSetting { UserId = userId };
			var mappedResponse = _mapper.Map<GetSettingResponse>(setting);
			return BaseResponse<GetSettingResponse>.Ok(mappedResponse, "Pomodoro setting retrieved successfully.");
		}

		public async Task<BaseResponse<string>> UpdateAsync(Guid userId, UpdateSettingRequest request)
		{
			var validationResult = await _validator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed.", ErrorType.Validation, errors);
			}

			var setting = await _repository.GetByConditionAsync(s => s.UserId == userId);
			if (setting == null)
			{
				setting = new PomodoroSetting { Id = Guid.NewGuid(), UserId = userId };
				var existedUser = await _userRepository.GetByConditionAsync(u => u.Id == userId);
				if (existedUser == null)
				{
					return BaseResponse<string>.Fail("User not found.", ErrorType.NotFound);
				}
				existedUser.PomodoroSettingId = setting.Id;
				_userRepository.Update(existedUser);
				var userUpdateResponse = await _userRepository.SaveChangesAsync();
				await _repository.AddAsync(setting);
				_mapper.Map(request, setting);
				var addResponse = await _repository.SaveChangesAsync();
				return addResponse && userUpdateResponse
					? BaseResponse<string>.Ok("Pomodoro settings created successfully.")
					: BaseResponse<string>.Fail("Failed to create Pomodoro settings.", ErrorType.ServerError);
			}

			_mapper.Map(request, setting);
			_repository.Update(setting);
			var response = await _repository.SaveChangesAsync();
			return response
				? BaseResponse<string>.Ok("Pomodoro settings updated successfully.")
				: BaseResponse<string>.Fail("Failed to update Pomodoro settings.", ErrorType.ServerError);
		}
	}
}
