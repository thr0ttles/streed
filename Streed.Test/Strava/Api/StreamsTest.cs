using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Streed.Test.Strava.Api
{
    [TestClass]
    public class StreamsTest
    {
        [TestMethod]
        public async Task GetElevationStreamAndDownsample()
        {
            var result = await Streed.Strava.Api.Streams.GetAltitude(Common.ActivityId, Common.AccessToken);
            Assert.IsNotNull(result);
            
            var distanceStream = result.Where(w => w.Type == "distance").Single();
            var altitudeStream = result.Where(w => w.Type == "altitude").Single();

            Assert.IsTrue(distanceStream.Data.Length == altitudeStream.Data.Length);

            var rawdata = new List<Tuple<double, double>>();
            var datalen = distanceStream.Data.Length;
            var distdata = distanceStream.Data;
            var altdata = altitudeStream.Data;
            for (int i = 0; i < datalen; i++)
            { 
                var dist = Convert.ToDouble(distdata[i]);
                var alt = Convert.ToDouble(altdata[i]);
                rawdata.Add(new Tuple<double,double>(dist, alt));                
            }

            var threshold = datalen / 10;
            var downsampleddata = Streed.LTTB.LargestTriangleThreeBuckets(rawdata, threshold);
            
            var sb = new StringBuilder();
            rawdata.ForEach(d => sb.AppendFormat("[{0}, {1}],", d.Item1, d.Item2));
            System.Diagnostics.Debugger.Log(0, string.Empty, sb.ToString() + Environment.NewLine);

            sb.Clear();
            downsampleddata.ToList().ForEach(d => sb.AppendFormat("[{0}, {1}],", d.Item1, d.Item2));
            System.Diagnostics.Debugger.Log(0, string.Empty, sb.ToString() + Environment.NewLine);
        }
    }
}
