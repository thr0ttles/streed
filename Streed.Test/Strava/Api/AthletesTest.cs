using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Streed.Test.Strava.Api
{
    [TestClass]
    public class AthletesTest
    {
        [TestMethod]
        public async Task GetAthlete()
        {
            var result = await Streed.Strava.Api.Athletes.GetAthlete(Common.AthleteId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetKoms()
        {
            var result = await Streed.Strava.Api.Athletes.GetKoms(Common.AthleteId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetFriends()
        {
            var result = await Streed.Strava.Api.Athletes.GetFriends(Common.AthleteId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetFollowers()
        {
            var result = await Streed.Strava.Api.Athletes.GetFollowers(Common.AthleteId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetBothFollowing()
        {
            var result = await Streed.Strava.Api.Athletes.GetBothFollowing(Common.AthleteId, Common.AccessToken);
            Assert.IsNotNull(result);
        }
    }
}
