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
			var schedule = await _scheduleRepository.GetByConditionAsync(s => s.Id == id);
			if (schedule == null)
			{
				return BaseResponse<string>.Fail("Schedule not found", ErrorType.NotFound);
			}

			if (schedule.IsCheckin)
				return BaseResponse<string>.Fail("You already checked in.", ErrorType.Validation);

			var earliestCheckIn = schedule.StartTime.AddMinutes(-schedule.ReminderMinutesBefore);
			if (DateTime.UtcNow < earliestCheckIn)
			{
				return BaseResponse<string>.Fail(
					$"You can only check in within {schedule.ReminderMinutesBefore} minutes before the schedule starts.",
					ErrorType.Validation
				);
			}

			if (DateTime.UtcNow > schedule.EndTime)
			{
				return BaseResponse<string>.Fail("Cannot check in after the schedule has ended.", ErrorType.Validation);
			}

			schedule.IsCheckin = true;
			schedule.CheckInTime = DateTime.UtcNow;
			schedule.UpdatedAt = DateTime.UtcNow;

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

			var hasConflict = await _scheduleRepository.AnyAsync(s =>
																	s.UserId == request.UserId &&
																	s.EndTime > request.StartTime &&
																	s.StartTime < request.EndTime
																);

			if (hasConflict)
				return BaseResponse<string>.Fail("Schedule time overlaps with another schedule.", ErrorType.Conflict);

			var schedule = _mapper.Map<Schedule>(request);
			// Ensure UpdatedAt is set on creation
			schedule.UpdatedAt = DateTime.UtcNow;

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

			var existingSchedule = await _scheduleRepository.GetByConditionAsync(s => s.Id == id);

			if (existingSchedule == null)
				return BaseResponse<string>.Fail("Schedule not found", ErrorType.NotFound);

			if (existingSchedule.IsCheckin)
				return BaseResponse<string>.Fail("Cannot update a checked-in schedule.", ErrorType.Validation);

			if (existingSchedule.EndTime < DateTime.UtcNow)
				return BaseResponse<string>.Fail("Cannot update a past schedule.", ErrorType.Validation);

			_mapper.Map(request, existingSchedule);
			existingSchedule.UpdatedAt = DateTime.UtcNow;

			_scheduleRepository.Update(existingSchedule);
			var result = await _scheduleRepository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Schedule updated successfully")
				: BaseResponse<string>.Fail("Failed to update schedule", ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteScheduleAsync(Guid id)
		{
			var existingSchedule = await _scheduleRepository.GetByConditionAsync(s => s.Id == id && !s.IsCheckin);
			if (existingSchedule == null)
				return BaseResponse<string>.Fail("Schedule not found or already check-in", ErrorType.NotFound);

			if (existingSchedule.EndTime < DateTime.UtcNow)
				return BaseResponse<string>.Fail("Cannot delete past schedules.", ErrorType.Validation);

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
			schedule.UpdatedAt = DateTime.UtcNow;
			_scheduleRepository.Update(schedule);
			await _scheduleRepository.SaveChangesAsync();
		}
	}
}
