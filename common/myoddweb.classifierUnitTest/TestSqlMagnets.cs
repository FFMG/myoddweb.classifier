using System;
using System.Linq;
using NUnit.Framework;
using myoddweb.classifier.core;

namespace myoddweb.classifierUnitTest
{
  /// <summary>
  /// Summary description for TestSqlMgnets
  /// </summary>
  [TestFixture]
  public class TestSqlMagnets : TestCommon
  {
    public TestSqlMagnets()
    {
      ReleaseEngine(false);
    }

    [TearDown]
    public void CleanupTest()
    {
      ReleaseEngine(false);
    }

    [OneTimeTearDown]
    public void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [Test]
    public void TestSimpleCreate()
    {
      var randomName = RandomString(10);

      var categoryId = TheEngine.GetCategory(RandomString(5));
      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName, 1, categoryId));
    }

    [Test]
    public void TestCreateWithInvalidCategoryId()
    {
      var randomName = RandomString(10);

      var random = new Random(Guid.NewGuid().GetHashCode());
      var categoryId = random.Next(100, 1000);
      Assert.AreEqual(-1, TheEngine.Magnets.CreateMagnet(randomName, 1, categoryId));
    }

    [Test]
    public void TestCreateWithEmptyName()
    {
      var categoryId = TheEngine.GetCategory(RandomString(5));
      Assert.AreEqual(-1, TheEngine.Magnets.CreateMagnet("", 1, categoryId));
    }

    [Test]
    public void TestCreateWithEmptyNameWithSpaces()
    {
      var categoryId = TheEngine.GetCategory(RandomString(5));
      Assert.AreEqual(-1, TheEngine.Magnets.CreateMagnet("            ", 1, categoryId));
    }

    [Test]
    public void TestDeleteMagnetThatDoesNotExist()
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
      var magnetId = random.Next(100, 1000);
      Assert.IsFalse(TheEngine.Magnets.DeleteMagnet(magnetId));
    }

    [Test]
    public void TestCreateThenDelete()
    {
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));
      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, 1, categoryId);
      Assert.IsTrue(TheEngine.Magnets.DeleteMagnet(magnetId));
    }

    [Test]
    public void TestCreateMultipleWithSameCategoryThenGetMagnets()
    {
      var randomName1 = RandomString(10);
      var randomName2 = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName1, 1, categoryId));
      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName2, 1, categoryId));

      var magnets = TheEngine.Magnets.GetMagnets();
      Assert.IsNotNull(magnets);
      Assert.AreEqual(2, magnets.Count);

      var magnet1 = magnets.FirstOrDefault(o => o.Name == randomName1);
      Assert.IsNotNull(magnet1);
      Assert.AreEqual(magnet1.Category, categoryId);
      Assert.AreEqual(magnet1.Rule, 1);

      var magnet2 = magnets.FirstOrDefault(o => o.Name == randomName2);
      Assert.IsNotNull(magnet2);
      Assert.AreEqual(magnet2.Category, categoryId);
      Assert.AreEqual(magnet2.Rule, 1);
    }

    [Test]
    public void TestCreateMultipleWithDifferentCategoryAndRuleThenGetMagnets()
    {
      var randomName1 = RandomString(10);
      var randomName2 = RandomString(10);
      var categoryId1 = TheEngine.GetCategory(RandomString(5));
      var categoryId2 = TheEngine.GetCategory(RandomString(5));

      var random = new Random(Guid.NewGuid().GetHashCode());
      var ruleId1 = random.Next(0, 100);
      var ruleId2 = random.Next(0, 100);

      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName1, ruleId1, categoryId1));
      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName2, ruleId2, categoryId2));

      var magnets = TheEngine.Magnets.GetMagnets();
      Assert.IsNotNull(magnets);
      Assert.AreEqual(2, magnets.Count);

      var magnet1 = magnets.FirstOrDefault(o => o.Name == randomName1);
      Assert.IsNotNull(magnet1);
      Assert.AreEqual(magnet1.Category, categoryId1);
      Assert.AreEqual(magnet1.Rule, ruleId1);

      var magnet2 = magnets.FirstOrDefault(o => o.Name == randomName2);
      Assert.IsNotNull(magnet2);
      Assert.AreEqual(magnet2.Category, categoryId2);
      Assert.AreEqual(magnet2.Rule, ruleId2);
    }

    [Test]
    public void TestUpdateTheMagnetNameUsingValues()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var random = new Random(Guid.NewGuid().GetHashCode());
      var ruleId = random.Next((int)RuleTypes.RuleTypesFirst, (int)RuleTypes.RuleTypesLast - 1);

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // update it.
      var newRandomName = RandomString(15);
      Assert.IsTrue(TheEngine.Magnets.UpdateMagnet(magnetId, newRandomName, ruleId, categoryId));

      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnet);
      Assert.AreEqual(newRandomName, updatedMagnet.Name);
    }

    [Test]
    public void TestUpdateTheMagnetNameUsingClass()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var random = new Random(Guid.NewGuid().GetHashCode());
      var ruleId = random.Next((int)RuleTypes.RuleTypesFirst, (int)RuleTypes.RuleTypesLast - 1);

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      var currentMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(currentMagnet);

      // update it.
      var newRandomName = RandomString(15);
      Assert.IsTrue(TheEngine.Magnets.UpdateMagnet(currentMagnet, newRandomName, ruleId, categoryId));

      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnet);
      Assert.AreEqual(newRandomName, updatedMagnet.Name);
    }

    [Test]
    public void TestUpdateTheMagnetWithEmptyName()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var random = new Random(Guid.NewGuid().GetHashCode());
      var ruleId = random.Next((int)RuleTypes.RuleTypesFirst, (int)RuleTypes.RuleTypesLast - 1);

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // update it.
      Assert.IsFalse(TheEngine.Magnets.UpdateMagnet(magnetId, "", ruleId, categoryId));

      // everything is still the same 
      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnet);
      Assert.AreEqual(randomName, updatedMagnet.Name);
    }

    [Test]
    public void TestCreateMultipleWithDifferentCategoryAndRuleThenGetMagnetsThenAddAnother()
    {
      var randomName1 = RandomString(10);
      var randomName2 = RandomString(10);
      var categoryId1 = TheEngine.GetCategory(RandomString(5));
      var categoryId2 = TheEngine.GetCategory(RandomString(5));

      var random = new Random(Guid.NewGuid().GetHashCode());
      var ruleId1 = random.Next(0, 100);
      var ruleId2 = random.Next(0, 100);

      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName1, ruleId1, categoryId1));
      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName2, ruleId2, categoryId2));

      var magnets = TheEngine.Magnets.GetMagnets();
      Assert.IsNotNull(magnets);
      Assert.AreEqual(2, magnets.Count);

      var magnet1 = magnets.FirstOrDefault(o => o.Name == randomName1);
      Assert.IsNotNull(magnet1);
      Assert.AreEqual(magnet1.Category, categoryId1);
      Assert.AreEqual(magnet1.Rule, ruleId1);

      var magnet2 = magnets.FirstOrDefault(o => o.Name == randomName2);
      Assert.IsNotNull(magnet2);
      Assert.AreEqual(magnet2.Category, categoryId2);
      Assert.AreEqual(magnet2.Rule, ruleId2);

      var randomName3 = RandomString(10);
      var categoryId3 = TheEngine.GetCategory(RandomString(5));
      var ruleId3 = random.Next(0, 100);

      Assert.AreNotEqual(-1, TheEngine.Magnets.CreateMagnet(randomName3, ruleId3, categoryId3));

      magnets = TheEngine.Magnets.GetMagnets();
      Assert.IsNotNull(magnets);
      Assert.AreEqual(3, magnets.Count);

      var magnet3 = magnets.FirstOrDefault(o => o.Name == randomName3);
      Assert.IsNotNull(magnet3);
      Assert.AreEqual(magnet3.Category, categoryId3);
      Assert.AreEqual(magnet3.Rule, ruleId3);

      //  check another one.
      magnet2 = magnets.FirstOrDefault(o => o.Name == randomName2);
      Assert.IsNotNull(magnet2);
      Assert.AreEqual(magnet2.Category, categoryId2);
      Assert.AreEqual(magnet2.Rule, ruleId2);
    }

    [Test]
    public void TestUpdateUpdateMagnetButNothingChanges()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var random = new Random(Guid.NewGuid().GetHashCode());
      var ruleId = random.Next((int)RuleTypes.RuleTypesFirst, (int)RuleTypes.RuleTypesLast - 1);

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // nothing has changed
      Assert.IsTrue(TheEngine.Magnets.UpdateMagnet(magnetId, randomName, ruleId, categoryId));

      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnet);
      Assert.AreEqual(randomName, updatedMagnet.Name);
    }

    [Test]
    public void TestUpdateUpdateMagnetUsingClassButNothingChanges()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var ruleId = RandomRuleType();

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // nothing has changed
      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsTrue(TheEngine.Magnets.UpdateMagnet(updatedMagnet, randomName, ruleId, categoryId));

      var updatedMagnetCheck = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnetCheck);
      Assert.AreEqual(randomName, updatedMagnetCheck.Name);
    }

    [Test]
    public void TestUpdateUpdateMagnetNullClass()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));
      var ruleId = RandomRuleType();

      Assert.IsFalse(TheEngine.Magnets.UpdateMagnet(null, randomName, ruleId, categoryId));
    }

    [Test]
    public void TestUpdateUpdateMagnetUsingClassButNothingChangesNameHasSpaces()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var ruleId = RandomRuleType();

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // nothing has changed
      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      var randomNameWithSpaces = $"      {randomName}        ";
      Assert.IsTrue(TheEngine.Magnets.UpdateMagnet(updatedMagnet, randomNameWithSpaces, ruleId, categoryId));

      var updatedMagnetCheck = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnetCheck);
      Assert.AreEqual(randomName, updatedMagnetCheck.Name);

      Assert.AreEqual(updatedMagnetCheck.Id, updatedMagnetCheck.Id);
    }

    [Test]
    public void TestUpdateMagnetEmpty()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var ruleId = RandomRuleType();

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // nothing has changed
      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsFalse(TheEngine.Magnets.UpdateMagnet(updatedMagnet, "", ruleId, categoryId));

      // the old one still exists.
      var updatedMagnetCheck = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnetCheck);
      Assert.AreEqual(randomName, updatedMagnetCheck.Name);
      Assert.AreEqual(updatedMagnetCheck.Id, updatedMagnetCheck.Id);
    }

    [Test]
    public void TestUpdateMagnetWithSpace()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var ruleId = RandomRuleType();

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      // nothing has changed
      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsFalse(TheEngine.Magnets.UpdateMagnet(updatedMagnet, "   ", ruleId, categoryId));

      // the old one still exists.
      var updatedMagnetCheck = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnetCheck);
      Assert.AreEqual(randomName, updatedMagnetCheck.Name);
      Assert.AreEqual(updatedMagnetCheck.Id, updatedMagnetCheck.Id);
    }

    [Test]
    public void TestUpdateMagnetInvalidCategoryId()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));

      var ruleId = RandomRuleType();

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      var random = new Random(Guid.NewGuid().GetHashCode());
      var newCategoryId = random.Next(categoryId + 1, categoryId + 500);

      var updatedMagnet = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsFalse(TheEngine.Magnets.UpdateMagnet(updatedMagnet, randomName, ruleId, newCategoryId));

      // the old one still exists.
      var updatedMagnetCheck = TheEngine.Magnets.GetMagnets().FirstOrDefault(m => m.Id == magnetId);
      Assert.IsNotNull(updatedMagnetCheck);
      Assert.AreEqual(randomName, updatedMagnetCheck.Name);
      Assert.AreEqual(updatedMagnetCheck.Id, updatedMagnetCheck.Id);
    }
    
    [Test]
    public void TestCreateMagnetWithEmtyString()
    {
      var categoryId = TheEngine.GetCategory(RandomString(5));
      var ruleId = RandomRuleType();
      var magnetId = TheEngine.Magnets.CreateMagnet("", ruleId, categoryId);
      Assert.AreEqual(-1, magnetId);
    }

    [Test]
    public void TestCreateMagnetWithEmtyStringWithSpaces()
    {
      var categoryId = TheEngine.GetCategory(RandomString(5));
      var ruleId = RandomRuleType();
      var magnetId = TheEngine.Magnets.CreateMagnet( "            ", ruleId, categoryId);
      Assert.AreEqual(-1, magnetId);
    }

    static private int RandomRuleType()
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
      return random.Next((int)RuleTypes.RuleTypesFirst, (int)RuleTypes.RuleTypesLast - 1);
    }

    [Test]
    public void TestReCreateTheSameMagnet()
    {
      // create a magnet
      var randomName = RandomString(10);
      var categoryId = TheEngine.GetCategory(RandomString(5));
      var ruleId = RandomRuleType();

      var magnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId);

      var newMagnetId = TheEngine.Magnets.CreateMagnet(randomName, ruleId, categoryId);

      Assert.AreEqual( magnetId, newMagnetId);
    }

    [Test]
    public void TestUpdateAnExistingMagnetWithTheValuesOfAnother()
    {
      var categoryId = TheEngine.GetCategory(RandomString(5));
      var ruleId = RandomRuleType();

      // create magnets
      var randomName1 = RandomString(10);
      var randomName2 = RandomString(10);

      var magnetId1 = TheEngine.Magnets.CreateMagnet(randomName1, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId1);

      var magnetId2 = TheEngine.Magnets.CreateMagnet(randomName2, ruleId, categoryId);
      Assert.AreNotEqual(-1, magnetId2);

      // try and give #2 the values of #1
      Assert.IsFalse( TheEngine.Magnets.UpdateMagnet( magnetId2, randomName1, ruleId, categoryId) );
    }
  }
}
