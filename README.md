# Newsler

Newsler is a .NET Core console application that extracts article titles and links from a specific website using Selenium and saves them to a database. It demonstrates web scraping and database interaction using Entity Framework Core.

## Prerequisites

To run this application, make sure you have the following prerequisites installed on your machine:

- .NET Core SDK
- ChromeDriver (compatible with your Chrome browser version)

## Installation

1. Clone the repository or download the project files.
2. Open the project in your favorite IDE or code editor.

## Configuration

1. Open the `appsettings.json` file and update the connection string in the `NewsDbConnection` field to point to your SQL Server database.

## Usage

1. Set the path to the ChromeDriver executable by modifying the `chromeDriverPath` variable in the `Main` method of the `Program` class.
2. Run the application.
3. The application will open a Chrome browser, navigate to the specified website, and extract the article titles and links.
4. The extracted articles will be saved to the specified database table (make sure the table is created in the database).
5. The application will display the extracted article titles and links in the console.
6. If any new articles are found (not already existing in the database), they will be added to the database.
7. After completion, the application will display a message indicating whether articles were saved to the database or no articles were found.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.
