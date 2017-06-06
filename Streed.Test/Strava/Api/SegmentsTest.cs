using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Streed.Test.Strava.Api
{
    [TestClass]
    public class SegmentsTest
    {
        [TestMethod]
        public async Task GetSegment()
        {
            var result = await Streed.Strava.Api.Segments.GetSegment(Common.SegmentId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetSegmentEffort()
        {
            var result = await Streed.Strava.Api.Segments.GetSegmentEffort(Common.SegmentEffortId, Common.AccessToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetSegmentLeaderboardOverall()
        {
            var result = await Streed.Strava.Api.Segments.GetSegmentLeaderboard(Common.SegmentId, Common.AccessToken, null, true);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetSegmentLeaderboardThisYear()
        {
            var result = await Streed.Strava.Api.Segments.GetSegmentLeaderboard(Common.SegmentId, Common.AccessToken, null, true);
            Assert.IsNotNull(result);
        }
    }
}
