# MuseAndMasterpiece

*Muse & Masterpiece* is a platform designed for artists to showcase their artworks and categorize them under specific art types, such as Pencil Sketch or Painting. 
## Features

- *User Authentication* (To be executed in future)
  - Secure login and role-based access for Artists.
- *Artists*
  - Create, update, and delete artists.
- *Artworks Management*
  - Create, update, and delete Artworks.
- *Category*
  - Create, update, and delete Category.
  
- *Entity Relationships*
  - One-to-Many:  
      • Artists -> Artworks  
      • Category -> Artworks  
    
- *Database Integration*
  - Built with ASP.NET Core, C#, and Entity Framework.
    
- *CRUD Functionality*
  - Full Create, Read, Update, and Delete (CRUD) operations for artist, Artworks and category.
  - Fetch a list of Artists by artists ID.
  - Fetch a list of Artworks by artworks ID.
  - Fetch a list of Category by Category ID.
  
## Technologies Used

- *Backend*: ASP.NET Core, C#
- *Frontend: Razor Pages / MVC *(To be executed in future)
- *Database*: Microsoft SQL Server, Entity Framework
- *Authentication: Identity Framework *(To be executed in future)

## Setup Instructions

1. Clone the repository:
   sh
   github.com/Isha003-hub/MuseAndMasterpiece
   cd MuseAndMasterpiece
   
2. Install dependencies:
   sh
   dotnet restore
   
3. Configure the database:
   - Update appsettings.json with your database connection string.
   - Run migrations:
     sh
     dotnet ef database update
     
4. Run the application:
   sh
   dotnet run
   

## Future Enhancements 

•	Artist Profile Picture (Add/Remove).  
•	Comments/ratings for artworks.  
•	Search functionality (e.g., by artist, category, or artwork title).  
•	Sort/Filter option  

## Contact

For any queries or suggestions, reach out to [Isha Shah]github.com/Isha003-hub
