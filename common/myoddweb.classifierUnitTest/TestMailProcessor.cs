using System;
using NUnit.Framework;
using myoddweb.classifier.core;
using Moq;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestMailProcessor
  {
    [Test]
    public void TestConstructorEngineCannotBeNull()
    {
      Mock<Outlook._NameSpace> ns = new Mock<Outlook._NameSpace>();
      Assert.Throws<ArgumentNullException>( () =>  new MailProcessor(null, ns.Object));
    }

    [Test]
    public void TestConstructorSessionCannotBeNull()
    {
      Mock<IEngine> en = new Mock<IEngine>();
      Assert.Throws<ArgumentNullException>(() => new MailProcessor(en.Object, null));
    }
  }
}
