using AZReport.Repositories.Interface;
using AZReport.Services.IServices;
using AZReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZReport.Services
{
    public class ScheduleService : EntityService<Schedule>, IScheduleService
    {
        IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository countryRepository)
            : base(countryRepository)
        {
            _scheduleRepository = countryRepository;
        }

        public Schedule CheckAndCreate(Schedule schedule)
        {
            var sche = _scheduleRepository.FindBy(x => x.Code == schedule.Code && x.Date == schedule.Date).FirstOrDefault();
            if (sche == null)
            {
                _scheduleRepository.Add(schedule);
            }
            return schedule;
        }

        public List<Schedule> GetByDate(DateTime start, DateTime end)
        {
            return _scheduleRepository.FindBy(x => x.Date <= end && x.Date >= start).ToList();
        }
    }
}
