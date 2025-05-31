---
applyTo: "**/*.cs"
---

- Follow DDD and Clean Architecture principles.
- Use the latest C# language features.
- Avoid try/catch blocks unless necessary. Remember a global exception handler (middleware) is already in place.
- Avoid logging unless it makes sense for the business logic. Remember that request and response logging are already in place.
- Prefer solutions that are tailored to a minimal API approach instead of MVC.
- Try to keep the Program.cs file as clean as possible.
- Clean up unused usings.
- Format the code using the .NET formatter.