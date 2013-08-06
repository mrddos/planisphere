using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Scada.DataCenterAgent;

namespace Scada.Data.Client.Unit
{
    [TestClass]
    public class PacketUnitTest
    {


        [TestMethod]
        public void TestAuthPacket()
        {
            DataPacket dp = new DataPacket(SentCommand.Auth);
            dp.Settings = this.GetSettingsMock();
            dp.St = 38;
            dp.Build();
            string str = dp.ToString();

            Assert.IsTrue(str.Contains("QN=20130101082233000;"));
            Assert.IsTrue(str.Contains("MN=1203A010000000"));
            Assert.IsTrue(str.EndsWith("\r\n"));
        }

        [TestMethod]
        public void TestReplyPacket()
        {
            DataPacket dp = new DataPacket(SentCommand.Reply);
            dp.Settings = this.GetSettingsMock();
            dp.BuildReply("20130101082233000", 1);
            string str = dp.ToString();

            Assert.IsTrue(str.Contains("MN=1203A010000000"));
        }

        [TestMethod]
        public void TestResultPacket()
        {
            DataPacket dp = new DataPacket(SentCommand.Result);
            dp.Settings = this.GetSettingsMock();
            dp.BuildResult("20130101082233000", 0);
            string str = dp.ToString();

            Assert.IsTrue(str.Contains("MN=1203A010000000"));
            Assert.IsTrue(str.Contains("ExeRtn=0"));
        }

        [TestMethod]
        public void TestSplittedPackets()
        {
            
        }

        // Shared methods.
        private ISettings GetSettingsMock()
        {
            var s = new Mock<ISettings>();
            s.Setup<string>(m => m.Mn).Returns("1203A010000000");
            s.Setup<string>(m => m.Password).Returns("123456");
            s.Setup<DateTime>(m => m.CurrentTime).Returns(DateTime.Parse("2013-01-01 08:22:33"));
            return s.Object;
        }
    }
}
