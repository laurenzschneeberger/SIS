# Student Information System

## Overview

The Student Information System (SIS) is a comprehensive web application designed to manage educational data. It includes features for handling student information, course management, enrollments, and more. The system also integrates a chatbot powered by OpenAI's GPT-3.5-turbo model to assist users with their queries. It is live on the web under [this](https://sis-h6ffgdawhab9g3g5.westeurope-01.azurewebsites.net/) URL.  

## Features

- **User Management**: Manage student and course information.
- **Enrollment Tracking**: Keep track of student enrollments in various courses.
- **Chatbot Integration**: An intelligent chatbot that answers questions related to the system.
- **Error Handling**: Robust error handling for API interactions and user inputs.

## Technologies Used

- **C#**: The primary programming language for the backend.
- **ASP.NET Core**: Framework for building the web API.
- **Entity Framework**: For database interactions (if applicable).
- **OpenAI API**: Used for generating responses in the chatbot.
- **Microsoft.Extensions.Logging**: For logging application events and errors.

## Installation

To set up the project locally, follow these steps:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/student-information-system.git
   cd student-information-system
   ```

2. **Install dependencies**:
   Make sure you have the .NET SDK installed. You can download it from [here](https://dotnet.microsoft.com/download).

3. **Set up configuration**:
   Create a `.env` file or set environment variables for your OpenAI API key:
   ```plaintext
   OPENAI_API_KEY=your_openai_api_key
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```


