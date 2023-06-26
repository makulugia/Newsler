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
                    // Check if articles already exist in the database
                    var existingArticles = dbContext.Articles.ToList();

                    foreach (var article in articles)
                    {
                        // Check if the article already exists in the database
                        var existingArticle = existingArticles.FirstOrDefault(a => a.Title == article.Title && a.Link == article.Link);

                        if (existingArticle == null)
                        {
                            // Article is new, add it to the database
                            dbContext.Articles.Add(article);
                        }
                    }

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
