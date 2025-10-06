using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.Schedule;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;

namespace studeehub.Application.Services
{
	public class ScheduleService : IScheduleService
	{
		private readonly IScheduleRepository _scheduleRepository;
		private readonly IMapper _mapper;
		private readonly IValidator<CreateScheduleRequest> _createValidator;
		private readonly IValidator<UpdateScheduleRequest> _updateValidator;

		public ScheduleService(IScheduleRepository scheduleRepository, IMapper mapper, IValidator<CreateScheduleRequest> createValidator, IValidator<UpdateScheduleRequest> updateValidator)
		{
			_scheduleRepository = scheduleRepository;
			_mapper = mapper;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<BaseResponse<string>> CheckIn(Guid id)
		{
			var schedule = await _scheduleRepository.GetByIdAsync(s => s.Id == id);
			if (schedule == null)
			{
				return BaseResponse<string>.Fail("Schedule not found", ErrorType.NotFound);
			}

			if (DateTime.UtcNow.AddMinutes(schedule.ReminderMinutesBefore) < schedule.StartTime)
			{
				return BaseResponse<string>.Fail("Cannot check in before the schedule starts", ErrorType.Validation);
			}
			schedule.IsCheckin = true;
			schedule.CheckInTime = DateTime.UtcNow;

			_scheduleRepository.Update(schedule);
			var result = await _scheduleRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Checkin successfully")
				: BaseResponse<string>.Fail("Failed to checkin", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> CreateScheduleAsync(CreateScheduleRequest request)
		{
			var validationResult = _createValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var schedule = _mapper.Map<Schedule>(request);
			await _scheduleRepository.AddAsync(schedule);
			var result = await _scheduleRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Schedule created successfully")
				: BaseResponse<string>.Fail("Failed to create schedule", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> UpdateScheduleAsync(Guid id, UpdateScheduleRequest request)
		{
			var validationResult = _updateValidator.Validate(request);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
				return BaseResponse<string>.Fail("Validation failed", ErrorType.Validation, errors);
			}

			var existingSchedule = await _scheduleRepository.GetByIdAsync(s => s.Id == id);

			if (existingSchedule == null)
				return BaseResponse<string>.Fail("Schedule not found", ErrorType.NotFound);

			_mapper.Map(request, existingSchedule);
			_scheduleRepository.Update(existingSchedule);
			var result = await _scheduleRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Schedule updated successfully")
				: BaseResponse<string>.Fail("Failed to update schedule", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteScheduleAsync(Guid id)
		{
			var existingSchedule = await _scheduleRepository.GetByIdAsync(s => s.Id == id);
			if (existingSchedule == null)
				return BaseResponse<string>.Fail("Schedule not found", ErrorType.NotFound);

			_scheduleRepository.Remove(existingSchedule);
			var result = await _scheduleRepository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Schedule deleted successfully")
				: BaseResponse<string>.Fail("Failed to delete schedule", ErrorType.ServerError);
		}

		public Task<IEnumerable<Schedule>> GetCheckinSchedulesToRemindAsync(DateTime now)
			=> _scheduleRepository.GetCheckinSchedulesToRemindAsync(now);

		public Task<IEnumerable<Schedule>> GetUpcomingSchedulesToRemindAsync(DateTime now)
			=> _scheduleRepository.GetUpcomingSchedulesToRemindAsync(now);

		public async Task UpdateAsync(Schedule schedule)
		{
			_scheduleRepository.Update(schedule);
			await _scheduleRepository.SaveChangesAsync();
		}
	}
}
