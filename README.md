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
19. Add view response class and reuse functionality from the HTML response for setting the content
20. Add functionality to find specific views by path and by convention
21. Make sure the project copies the view files into the output directory
22. Add functionality in the base controller class to get the view and controller names by convention
23. Add functionality to parse the request form when the specific content type is present
24. Add functionality for extracting model data via reflection and replacing it in the HTML
25. Add functionality for storing and retrieving HTTP cookies
26. Add functionality for storing HTTP session
27. Add global exception handling and log all requests and responses in the console
28. Use session to store the currently authenticated user ID and write helper methods for authentication
29. Add static files option by choosing a public folder and adding all files in it as GET requests in the route table
30. Make sure the HTTP server handles byte array response bodies
31. Add automatic controller discovery by using reflection and mapping all public methods into the route table by convention
32. Add HttpGet and HttpPost attributes to automatically register the HTTP method of the action
33. Add Authorize attribute and short-circuit the request if there is no authenticated user
34. Implement a layout page logic and insert the view content in it
35. Use reflection to analyze the action parameters and populate them automatically from the request
36. Remove the required constructor on the base Controller class and populate the request property automatically
37. Introduce proper collections for headers, cookies, form and query
38. Implement service collection with configuration and create controllers by using a dependendency resolver with reflection and recursion
39. Add advanced view engine features - conditionals statements and loops
40. Introduce user information during view rendering

Potential Tasks:
- Introduce model state and automatic validation
- Cache
- Include the view files into the assemblies
- Allow headers with the same name
- Make the setter of the controller request private
- Introduce server features to separate the abstraction for routing and services