using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Newsler.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private Mock<IWebDriver> mockWebDriver;

        [SetUp]
        public void Setup()
        {
            // Create a mock instance of IWebDriver
            mockWebDriver = new Mock<IWebDriver>();
        }

        [Test]
        public void Main_ValidWebsite_NavigatesToWebsite()
        {
            // Arrange
            var program = new Program();
            mockWebDriver.Setup(driver => driver.Navigate().GoToUrl(It.IsAny<string>()));

            // Act
            program.RunProgram();

            // Assert
            mockWebDriver.Verify(driver => driver.Navigate().GoToUrl("https://www.businessnews.com.tn/"), Times.Once);
        }

        [Test]
        public void Main_ValidArticles_StoresArticlesInDatabase()
        {
            // Arrange
            var program = new Program();
            var mockDriverOptions = new Mock<ChromeOptions>();
            mockDriverOptions.Setup(options => options.AddArgument("--ignore-certificate-errors"));
            mockWebDriver.Setup(driver => driver.FindElements(By.XPath(It.IsAny<string>())))
                .Returns(CreateMockArticleElements());
            mockWebDriver.Setup(driver => driver.Quit());
            mockWebDriver.Setup(driver => driver.Dispose());

            // Mock the NewsContext with an in-memory database
            var options = new DbContextOptionsBuilder<NewsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new NewsContext(options))
            {
                // Act
                program.RunProgram();

                // Assert
                Assert.AreEqual(2, dbContext.Articles.Count());
                Assert.AreEqual("Article 1", dbContext.Articles.First().Title);
                Assert.AreEqual("https://www.businessnews.com.tn/article1", dbContext.Articles.First().Link);
            }
        }

        private List<IWebElement> CreateMockArticleElements()
        {
            var article1 = new Mock<IWebElement>();
            article1.Setup(element => element.Text).Returns("Article 1");
            article1.Setup(element => element.GetAttribute("href")).Returns("https://www.businessnews.com.tn/article1");

            var article2 = new Mock<IWebElement>();
            article2.Setup(element => element.Text).Returns("Article 2");
            article2.Setup(element => element.GetAttribute("href")).Returns("https://www.businessnews.com.tn/article2");

            return new List<IWebElement> { article1.Object, article2.Object };
        }
    }
}
