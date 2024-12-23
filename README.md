# BulkMailSender

## Overview
BulkMailSender is an open-source Blazor project built using **Clean Architecture** principles and .NET 9. It is designed to make bulk email sending easy and efficient with advanced features like importing email addresses from Excel files, attaching files, embedding inline images, and editing email bodies using the Quill editor.

## Solution Structure
This project follows the Clean Architecture pattern and consists of the following layers:
- **Domain**: Core business logic and abstractions.
- **Application**: Use cases for email workflows.
- **Infrastructure**: External system integrations (SQL Server, email sending, etc.).
- **UI (Blazor)**: Front-end user interface.

## Features
- Bulk email sending.
- Import email addresses from Excel files.
- Attach files and inline images to emails.
- Rich text email editing using Quill editor.
- SQL Server support for storing email history.

## Prerequisites
- **.NET 9 SDK**: [Download .NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server**: Installed and configured.
- **Excel Support**: For importing email addresses.
- **Visual Studio 2022+**

## Getting Started
### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/BulkMailSender.git
   cd BulkMailSender
