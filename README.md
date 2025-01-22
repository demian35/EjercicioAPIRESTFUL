# EjercicioAPIRESTFUL


Using ASP.NET Core, implement a RESTFUL API to retrieve details of the top n stories from the Hacker News API, as determined by score, where n is specified by the API caller

The Hacker News API is documented here https://github.com/HackerNews/API

IDs for stories can be retrieved from this URI https://hacker-news.firebaseio.com/v0/item/21233041.json (In this case for the story with id 21233041)

The API should return an array of the top n stories as returned by the Hacker News API in decreasing order of score, in the format
[
{"title": "An ublock Origin update from the Chrome web store was rejected", "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745","postedby": "ismaildonnez", "time": "2019-10-12T13:43:01+00:00","score":1716,"commentCount": 572},
{...},
{...},
{...}
]
In addition to the above, your API must be able to efficiently serve a large number of requests without risking overloading the Hacker News API.

You must share a public repository with us. This should include a README.md file describing how to run the application, any assumptions you have made, and any improvements or changes you will make, given the time




##Como ejecutar 
primero Clonar el repositorio en su PC 

Luego entrar al proyecto descargado "EjercicioAPIRESTFUL"

Ya en el fichero posicionarse en el subdirectorio con el mismo nombre "EjercicioAPIRESTFUL"

Ya ahi ejecuta el comando "dotnet restore" para restaurar dependencias

Despues ejecuta el comando "dotnet build" para compilar 

Por ultimo ejecuta el comando "dotnet run" para correr el proyecto

Para hacer peticiones GET mediante postman solo hay que pasarle la uri http://localhost:(tu direccion localhost)/api/stories?n=numero de historias que quieras mostrar

O puedes ingresar la uri en el navegador de tu preferencia http://localhost:(Tu direccion localhost)/api/stories?n=numero de historias que quieras mostras

##Eng

##Requirements to run the API
You need at least SDK .NET 7.0 or higher


##How to run
First Clone the repository on your PC

Then enter the downloaded project "EjercicioAPIRESTFUL"

Once in the file position yourself in the subdirectory with the same name "EjercicioAPIRESTFUL"

Now run the command "dotnet restore" to restore dependencies

Then run the command "dotnet build" to compile

Finally run the command "dotnet run" to run the project

To make GET requests through postman you only have to pass the uri http://localhost:(your localhost address)/api/stories?n=number of stories you want to show

Or you can enter the uri in the browser of your preference http://localhost:(Your localhost address)/api/stories?n=number of stories you want to show you show



