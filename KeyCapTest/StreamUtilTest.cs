using KeyCap.Format;
using KeyCap.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace KeyCapTest
{
    [TestClass]
    public class StreamUtilTest
    {
        [TestMethod]
        public void TestReadIntFromStream()
        {
            var zStream = new MemoryStream();
            const int EXPECTED_VALUE = int.MaxValue - 10;
            StreamUtil.WriteIntToStream(zStream, EXPECTED_VALUE);
            zStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(EXPECTED_VALUE, StreamUtil.ReadIntFromStream(zStream));
        }

        [TestMethod]
        public void TestReadIntFromStreamFlag()
        {
            var zStream = new MemoryStream();
            var EXPECTED_VALUE = BitUtil.UpdateFlag(0, OutputConfig.OutputFlag.Control, true);
            StreamUtil.WriteIntToStream(zStream, EXPECTED_VALUE);
            zStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(EXPECTED_VALUE, StreamUtil.ReadIntFromStream(zStream));
        }
    }
}
