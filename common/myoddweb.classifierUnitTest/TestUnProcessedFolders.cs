using System;
using myoddweb.classifier.core;
using Moq;
using NUnit.Framework;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  class TestUnProcessedFolders
  {
    [Test]
    public void TestConstructorEngineCannotBeNull()
    {
      var ns = new Mock<Microsoft.Office.Interop.Outlook._NameSpace>();
      var en = new Mock<IEngine>();
      var mp = new MailProcessor( en.Object, ns.Object );

      Assert.Throws<ArgumentNullException>(() => new UnProcessedFolders( "Title", mp, null));
    }

    [Test]
    public void TestConstructorMailProcessorCannotBeNull()
    {
      var lo = new Mock<ILogger>();

      Assert.Throws<ArgumentNullException>(() => new UnProcessedFolders( "Title", null, lo.Object ));
    }
  }
}
