using System;
using NUnit.Framework;
using myoddweb.classifier.core;
using Moq;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestMailProcessor : TestCommon
  {
    [Test]
    public void TestConstructorEngineCannotBeNull()
    {
      Mock<Outlook._NameSpace> ns = new Mock<Outlook._NameSpace>();
      Outlook._NameSpace it = ns.Object;
      Assert.Throws<ArgumentNullException>( () =>  new MailProcessor(null, it ));
    }

    [Test]
    public void TestConstructorSessionCannotBeNull()
    {
      Assert.Throws<ArgumentNullException>(() => new MailProcessor(TheEngine, null));
    }
  }
}
