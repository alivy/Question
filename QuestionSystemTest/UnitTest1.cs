using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuestionSystem.Weixin;

namespace QuestionSystemTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string url = "1234";
            var config = WeiXinAPI.GetWeiXinConfig(url);
        }
    }
}
