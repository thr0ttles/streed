using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Streed.Test.Strava
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void PolylineToGeoCoordinates()
        {
            var polyline = "cmprFhd}hMD}AnEh@nFsEcV}i@rJcJxH}SzQgArX{h@hAwObI{Apd@k|D~l@glAlSii@ll@rBjLoD|f@q]~uHenEp[sTmsAccBuvAmc@}B@aJhH}\\hEwPpUq_@zEa[j|@zYfB`HxIDhDkMx[}V`AqOlEgDbNm`@bcAon@rfA_z@hxBmk@bhAqe@b`EqHlA}ApPkIvPob@vq@}@jGyEtAuDhEtOv_@sD|EcFIeAv@l@bA";
            var result = Streed.Strava.Utilities.PolylineToGeoCoordinates(polyline);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() != 0);
        }
    }
}
