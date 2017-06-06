using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Streed.Test.Strava.Api
{
    [TestClass]
    public class ActivitiesTest
    {
        [TestMethod]
        public async Task GetActivity()
        {
            var result = await Streed.Strava.Api.Activities.GetActivity(Common.ActivityId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UpdateActivity()
        {
            var result = await Streed.Strava.Api.Activities.UpdateActivity(Common.ActivityId, Common.AccessToken, "Put your life on hold for 1/2 second instead of hitting me", string.Empty, false);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GiveKudos()
        {
            await Streed.Strava.Api.Activities.GiveKudos(Common.ActivityId, Common.AccessToken);
        }

        [TestMethod]
        public async Task GetRelatedActivities()
        {
            var result = await Streed.Strava.Api.Activities.GetRelatedActivities(Common.ActivityId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetZones()
        {
            var result = await Streed.Strava.Api.Activities.GetZones(Common.ActivityId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetFriendActivities()
        {
            var result = await Streed.Strava.Api.Activities.GetFriendActivities(Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAthleteActivities()
        {
            var result = await Streed.Strava.Api.Activities.GetAthleteActivities(Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetComments()
        {
            var result = await Streed.Strava.Api.Activities.GetComments(Common.ActivityId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task InsertComment()
        {
            var result = await Streed.Strava.Api.Activities.InsertComment(Common.ActivityId, Common.AccessToken, string.Empty);
            Assert.IsNotNull(result);
        }
    }
}
