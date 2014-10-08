﻿using System;
using System.Linq;
using NUnit.Framework;

namespace NinjaNye.SearchExtensions.Tests.Integration.Fluent
{
    [TestFixture]
    public class FluentSearchTests : IDisposable
    {
        readonly TestContext context = new TestContext();

        [Test]
        public void Search_SearchWithoutActionHasNoAffectOnTheResults_ResultsAreUnchanged()
        {
            //Arrange
            
            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).ToList();

            //Assert
            CollectionAssert.AreEqual(this.context.TestModels, result);
        }

        [Test]
        public void Search_Contains_OnlyResultsContainingTermAreReturned()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).Containing("abc").ToList();

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains("abc")));
        }

        [Test]
        public void Search_ContainsThenStartsWith_OnlyResultsThatContainTextAndStartWithTextAreReturned()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).Containing("abc").StartsWith("a").ToList();

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains("abc") && x.StringOne.StartsWith("a")));
        }

        [Test]
        public void Search_Equals_DefinedPropertyEqualsSearchResult()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).IsEqual("abcd").ToList();

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne == "abcd"));
        }

        [Test]
        public void SearchMultiple_ResultContainsAcrossTwoProperties_ResultContainsTermInEitherProperty()
        {
            //Arrange
            const string searchTerm = "cd";

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne, x => x.StringTwo).Containing(searchTerm).ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains(searchTerm) || x.StringTwo.Contains(searchTerm)));
        }

        [Test]
        public void SearchMultiple_ResultStartsWithAcrossTwoProperties_ResultStartsWithTermInEitherProperty()
        {
            //Arrange
            const string searchTerm = "ef";

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne, x => x.StringTwo).StartsWith(searchTerm).ToList();
            
            //Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.StartsWith(searchTerm) || x.StringTwo.StartsWith(searchTerm)));
        }

        [Test]
        public void SearchAll_NoPropertiesDefined_AllPropertiesAreSearched()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search().Containing("cd").ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains("cd") || x.StringTwo.Contains("cd")));
        }

        [Test]
        public void Search_ContainingMultipleTerms_SearchAgainstMultipleTerms()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).Containing("ab", "jk").ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains("ab") || x.StringOne.Contains("jk")));
        }

        [Test]
        public void Search_StartsWithMultipleTerms_SearchAgainstMultipleTerms()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).StartsWith("ab", "ef").ToList();

            //Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.StartsWith("ab") || x.StringOne.StartsWith("ef")));

        }

        [Test]
        public void Search_SearchManyPropertiesContainingManyTerms_AllResultsHaveASearchTermWithin()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne, x => x.StringTwo).Containing("cd", "jk").ToList();

            //Assert
            Assert.AreEqual(5, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains("cd") || x.StringOne.Contains("jk")
                                       || x.StringTwo.Contains("cd") || x.StringTwo.Contains("jk")));
        }

        [Test]
        public void Search_SearchManyPropertiesStartingWithManyTerms_AllResultsHaveAPropertyStartingWithASpecifiedTerm()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne, x => x.StringTwo).StartsWith("cd", "ef").ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.StartsWith("cd") || x.StringOne.StartsWith("ef")
                                       || x.StringTwo.StartsWith("cd") || x.StringTwo.StartsWith("ef")));
        }

        [Test]
        public void Search_SearchContainsIgnoreCase_CaseIsIgnored()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).Containing("AB", "jk").ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne.Contains("jk") || x.StringOne.Contains("ab")));
        }

        [Test]
        public void Search_SearchStartsWithIgnoreCase_CaseIsIgnored()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringTwo).StartsWith("C").ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.All(x => x.StringTwo.StartsWith("c", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void Search_SearchIsEqual_CaseIsIgnored()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringTwo).IsEqual("CASE").ToList();

            //Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(x => x.StringTwo.Equals("case", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void Search_SearchManyTermsAreEqual_ResultsMatchAnyTerm()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).IsEqual("abcd", "efgh").ToList();

            //Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne == "abcd" || x.StringOne == "efgh"));
        }

        [Test]
        public void Search_SearchManyTermsAreEqual_ResultsMatchAnyTermInAnyCase()
        {
            //Arrange

            //Act
            var result = this.context.TestModels.Search(x => x.StringOne).IsEqual("ABCD", "EFGH");

            //Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(x => x.StringOne == "abcd" || x.StringOne == "efgh"));
        }

        //[Test]
        //public void Search_PerformNullCheck_NullCheckIsPerformed()
        //{
        //    //Arrange
            
        //    //Act
        //    var result = this.context.TestModels.Search(x => x.StringOne)
        //                                        .CheckForNull();

        //    //Assert
        //    Assert.AreEqual(2, result.Count());
        //}

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
