using Microsoft.Data.Entity;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningTime.Models
{
    public class LearningRepository : ILearningRepository
    {
        private LearningContext _context;
        private ILogger<LearningRepository> _logger;

        public LearningRepository(LearningContext context, ILogger<LearningRepository> logger)
        {
            _logger = logger;
            _context = context;
        }

        public void AddStop(string tripName, Stop newStop)
        {
            var trip = GetTripByName(tripName);
            newStop.Order = trip.Stops.Max(s => s.Order) + 1;
            trip.Stops.Add(newStop);
        }

        public void AddTrip(Trip newTrip)
        {
            _context.Add(newTrip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return _context.Trips.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get trips from database", ex);
                return null;
            }

        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            try
            {
                return _context.Trips
                    .Include(t => t.Stops)
                    .OrderBy(t => t.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get trips with stops from database", ex);
                return null;
            }
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                    .Include(t => t.Stops)
                    .Where(t => t.Name == tripName)
                    .FirstOrDefault();
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
