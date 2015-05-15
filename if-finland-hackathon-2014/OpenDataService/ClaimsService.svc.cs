using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using OpenDataService.DataManagement;
using OpenDataService.Helpers;

namespace OpenDataService {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ClaimsService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ClaimsService.svc or ClaimsService.svc.cs at the Solution Explorer and start debugging.
    public class ClaimsService : IClaimsService {
        

        public Claim GetClaimById(int id)
        {
            using (var db = new OpenDataDatabaseEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var claim = db.Claim.Include("Car").Include("Location").FirstOrDefault(cl=>cl.Id == id);
                return claim;
            }
        }

        public void WriteLocations(string address, Location uniLocation)
        {
            using (var db = new OpenDataDatabaseEntities())
            {
                var rand = new Random();
                var claims = db.Claim.Where(cl => cl.MunicipalityClaim.Contains(address));
                foreach (var claim in claims)
                {
                    claim.Location.Latitude = uniLocation.Latitude +
                                              (rand.Next(-1, 1)*rand.NextDouble() + rand.Next(-1, 1)*rand.NextDouble())/10;
                    claim.Location.Longitude = uniLocation.Longitude +
                                               (rand.Next(-1, 1)*rand.NextDouble() + rand.Next(-1, 1)*rand.NextDouble())/
                                               10;

                }
                db.SaveChanges();
            }
        }

        public IList<Location> GetLocationPacksByData(int skipmultiplier)
        {
            using (var db = new OpenDataDatabaseEntities()) {
                db.Configuration.ProxyCreationEnabled = false;
                var locations = db.Location.OrderBy(loc => loc.Id).Skip(200 * skipmultiplier).Take(200).ToList();
                return locations;
            }
        }

        public IList<string> GetAddresses()
        {
            using (var db = new OpenDataDatabaseEntities()) {
                var indexes = db.Claim.GroupBy(cl => cl.MunicipalityClaim);
                var indexList = indexes.Select(e => e.Key).ToList();

                return indexList;
            }
        }

        public IList<int> GetHourlyStatistics(string claimType) {
            using (var db = new OpenDataDatabaseEntities())
            {
                var list = new int[24];
                list[0] = db.Claim.Where(cl=> cl.ClaimType.Contains(claimType)).Count(cl => cl.Date.Hour == 24 || cl.Date.Hour == 0);
                for (var i = 1; i < 24; i++)
                {
                    list[i] = db.Claim.Where(cl=> cl.ClaimType.Contains(claimType)).Count(cl => cl.Date.Hour == i);
                }
                return list;
            }
        }

        public IList<int> GetDailyStatistics(string claimType) {
            using (var db = new OpenDataDatabaseEntities()) {
                var list = new int[7];
                var claims = db.Claim.Where(cl => string.IsNullOrEmpty(claimType) || cl.ClaimType.Contains(claimType)).Select(cl => cl.Date).ToArray();
                for (var i = 0; i < 7 ; i++) {
                    list[i] = claims.Count(cl => cl.Date.DayOfWeek == (DayOfWeek)i);
                }
                return list;
            }
        }

        public Dictionary<string, int> GetAccidentTypesByCoordinates(Location coords, int range)
        {
            using (var db = new OpenDataDatabaseEntities())
            {
                var locations = db.Location.ToArray().Where(loc => FormulaHelper.Measure(loc, coords) <= range).Select(e => e.Id).ToArray();
                var claims = db.Claim.Where(cl => locations.Contains(cl.LocationId)).GroupBy(claim => claim.ClaimType);
                return claims.OrderByDescending(claim => claim.Count()).ToDictionary(claim => claim.Key, claim => claim.Count());
            }
        }

        public IList<Claim> GetClaimsByCoordinates(Location coords, int range) {
            using (var db = new OpenDataDatabaseEntities()) {
                db.Configuration.ProxyCreationEnabled = false;
                var locations = db.Location.ToArray().Where(loc => FormulaHelper.Measure(loc, coords) <= range).Select(e => e.Id).ToArray();
                var claims = db.Claim.Where(cl => locations.Contains(cl.LocationId)).ToList();
                return claims;
            }
        }
    }
}
