# C# Web Server

A simple C# web server created for educational purposes.

Steps to create a similar web server from scratch:

1. Choose the localhost IP address (127.0.0.1) and a free local port
2. Create a TcpListener and accept incomming client request asynchronously 
3. Write a valid HTTP response and convert it to a byte array
4. Add Content-Type and Content-Length headers (be careful with UTF8 characters)
5. Read the request in chunks (1024 bytes each) and store it in a StringBuilder
6. Extract separate Server and HTTP classes
7. Parse the HTTP request