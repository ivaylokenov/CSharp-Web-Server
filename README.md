# C# Web Server

A simple C# web server created for educational purposes.

Steps to create a similar web server from scratch:

1. Choose the localhost IP address (127.0.0.1) and a free local port
2. Create a TcpListener and accept incomming client request asynchronously 
3. Write a valid HTTP response and convert it to a byte array
4. Add Content-Type and Content-Length headers (be careful with UTF8 characters)
5. Read the request in chunks (1024 bytes each) and store it in a StringBuilder
6. Extract separate server and HTTP classes
7. Parse the HTTP request
8. Create routing table which should allow various HTTP methods
9. Make sure the HTTP server can populate the routing table
10. Create specific HTTP response classes - TextResponse, for example
11. Implement the ToString method for the HTTP response class
12. Implement the routing table for storing and retrieving request mapping
13. Use the routing table in the HTTP server for actual request-response matching and execution
14. Separate the URL and parse the query string if it exists
15. Introduce the option to use the request by storing request-response functions in the routing table
16. Introduce base controllers and extract common functionalities
17. Shorten the route syntax and add support for controllers
18. Add redirect HTTP response and use the Location header

Tasks:
- Views and HTML
- Forms and user input
- Cookies and state 
- Reflection-based controllers 
- Conroller attributes
- Static files
- Error handling
- Dependency inversion concepts
- Model binding
- Views with models
- Session and cache
- Basic authentication