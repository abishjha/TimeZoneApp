using System;
using System.Collections.Generic;
using System.Linq;
using TimeZoneWebApi.Entities;
using TimeZoneWebApi.Helpers;

namespace TimeZoneWebApi.Services
{
    public interface ITimeZoneService
    {
        IEnumerable<Entities.TimeZone> GetAll();
        Entities.TimeZone GetById(int id);
        Entities.TimeZone Create(Entities.TimeZone timeZone);
        Entities.TimeZone Update(Entities.TimeZone timeZone);
        Entities.TimeZone Delete(int id);
    }

    public class TimeZoneService : ITimeZoneService
    {
        private DataContext _context;

        public TimeZoneService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Entities.TimeZone> GetAll()
        {
            return _context.TimeZones;
        }

        public Entities.TimeZone GetById(int id)
        {
            return _context.TimeZones.Find(id);
        }

        public Entities.TimeZone Create(Entities.TimeZone timeZone)
        {
            if (_context.TimeZones.Any(x => x.Id == timeZone.Id))
                throw new AppException("TimeZone \"" + timeZone.Id + "\" is already taken");

            if (string.IsNullOrWhiteSpace(timeZone.Name) || string.IsNullOrWhiteSpace(timeZone.City))
                throw new AppException("Name and/or City cannot be null");

            if (timeZone.DifferenceToGMT % 0.25 != 0 && timeZone.DifferenceToGMT < -12 && timeZone.DifferenceToGMT > 14)
                throw new AppException("DifferenceToGMT \"" + timeZone.DifferenceToGMT + "\" should be a multiple of 0.25");

            _context.TimeZones.Add(timeZone);
            _context.SaveChanges();

            return timeZone;
        }

        public Entities.TimeZone Update(Entities.TimeZone timeZoneParam)
        {
            var timeZone = _context.TimeZones.Find(timeZoneParam.Id);

            if (timeZone == null)
                throw new AppException("Time zone not found");

            if (string.IsNullOrWhiteSpace(timeZone.Name) || string.IsNullOrWhiteSpace(timeZone.City))
                throw new AppException("Name and/or City cannot be null");

            if (timeZoneParam.DifferenceToGMT % 0.25 == 0 && timeZoneParam.DifferenceToGMT > -13 && timeZoneParam.DifferenceToGMT < 15)
                timeZone.DifferenceToGMT = timeZoneParam.DifferenceToGMT;

            timeZone.Name = timeZoneParam.Name;
            timeZone.City = timeZoneParam.City;
            timeZone.DifferenceToGMT = timeZoneParam.DifferenceToGMT;

            _context.TimeZones.Update(timeZone);
            _context.SaveChanges();

            return timeZone;
        }

        public Entities.TimeZone Delete(int id)
        {
            var timeZone = _context.TimeZones.Find(id);
            if (timeZone != null)
            {
                _context.TimeZones.Remove(timeZone);
                _context.SaveChanges();
            }
            return timeZone;
        }
    }
}