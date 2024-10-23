# Document Editor

A real-time document editing application built with ASP.NET Core and React.

## Project Structure

- `src/DocumentEditor.Web`: Web API and React frontend
- `src/DocumentEditor.Core`: Core business logic and interfaces
- `src/DocumentEditor.Infrastructure`: Data access and service implementations

## Setup

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore`
4. Navigate to `src/DocumentEditor.Web/ClientApp`
5. Run `npm install`
6. Return to root directory
7. Run `dotnet run --project src/DocumentEditor.Web`

## Development

- Backend runs on `https://localhost:5001`
- Frontend dev server runs on `https://localhost:44455`
