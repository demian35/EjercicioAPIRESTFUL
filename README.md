# EjercicioAPIRESTFUL

Usando ASP.NET Core, implemente una API RESTFUL para recuperar detalles de las mejores n historias de la API de Hacker News, según lo determinado por el puntaje, donde n es especificado por el llamador a la API

La API de Hacker News está documentada aquí https://github.com/HackerNews/API

Los IDS para las historias se pueden recuperar de este URI https://hacker-news.firebaseio.com/v0/item/21233041.json (En este caso para la historia con id 21233041)

La API debe devolver una matriz de las mejores n historias según lo devuelto por la API de Hacker News en orden decreciente de puntaje, en el formato
[
{"title": "Se rechazó una actualización de ublock Origin de la tienda web de Chrome", "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745","postedby": "ismaildonnez", "time": "2019-10-12T13:43:01+00:00","score":1716,"commentCount": 572},
{...},
{...},
{...}
]
Además de lo anterior, su API debe poder atender de manera eficiente una gran cantidad de solicitudes sin correr el riesgo de sobrecargar la API de Hacker News.

Debe compartir un repositorio público con nosotros. Este debe incluir un archivo README.md que describa cómo ejecutar la aplicación, cualquier suposición que haya realizado y cualquier mejora o cambio que realice, dado el tiempo