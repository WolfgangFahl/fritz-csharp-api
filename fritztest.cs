using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using fritz;

[TestFixture]
public class FritzTest
{
    [Test]
    public void TestMD5() 
    {
        Fritz fritz=new Fritz();
        string md5_1=fritz.GetMD5Hash("d41d8cd98f00b204e9800998ecf8427e");
        Assert.AreEqual("",md5_1);
    }
}