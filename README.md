### Restaurant_API  

## Version  
0.3.0

## Project Specs  
ASP.net Core C#  

## Project Details  
Used for local development to build out a mock restaurant application  
Going to be utlitized for a react mobile project, and a desktop Angular project  

## Stories  
Once a story is complete, delete it here, and add a new one  
-- Implement authentication via the creation of a JWT token, return token with Login  
-- Add JWT token authorization within headers for JWT token  
-- Implement validation of JWT token, and ability to reissue new JWT Token  

# Paths  
GET					: /api/users  
GET, DELETE			: /api/user/:uuid  
POST, PUT			: /api/user  
POST				: /api/login  
GET					: /api/store/:storeId  
GET					: /api/stores  
GET					: /api/stores/dispositions/:storeId  
GET					: /api/stores/mealTypes/:storeId  
