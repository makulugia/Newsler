using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Newsler
{
    class Program
    {
        static void Main(string[] args)
        {
            RunProgram();
        }

        public static void RunProgram()
        {
            // Set the path to the ChromeDriver executable
            string chromeDriverPath = "path/to/chromedriver";

            // Create an instance of the ChromeDriver
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);
            IWebDriver driver = new ChromeDriver(chromeDriverService);

            // Navigate to the desired website
            driver.Navigate().GoToUrl("https://www.businessnews.com.tn/");

            // Find all the article title elements using Selenium
            var articleElements = driver.FindElements(By.XPath("//*[@id=\"mm-0\"]/main/div[2]/div[4]/div[2]/div[*]/div/a"));

            if (articleElements.Count > 0)
            {
                List<Article> articles = new List<Article>();

                foreach (var articleElement in articleElements)
                {
                    string articleTitle = articleElement.Text.Trim();
                    string articleLink = articleElement.GetAttribute("href");

                    articles.Add(new Article { Title = articleTitle, Link = articleLink });
                }

                // Store the extracted article titles and links or perform further actions
                foreach (var article in articles)
                {
                    Console.WriteLine($"Title: {article.Title}");
                    Console.WriteLine($"Link: {article.Link}");
                }

                using (var dbContext = new NewsContext())
                {
                    // Get existing article titles and links from the database
                    var existingArticles = dbContext.Articles.Select(a => new { a.Title, a.Link }).ToList();

                    // Filter out existing articles based on title and link
                    var newArticles = articles.Where(a => !existingArticles.Any(e => e.Title == a.Title && e.Link == a.Link));

                    // Add new articles to the database
                    dbContext.Articles.AddRange(newArticles);

                    // Save changes to the database
                    dbContext.SaveChanges();
                }

                Console.WriteLine("Articles saved to the database.");
            }
            else
            {
                Console.WriteLine("No articles found.");
            }

            // Close the browser window
            driver.Quit();

            // Dispose of the driver object to release resources
            driver.Dispose();
        }
    }
}
