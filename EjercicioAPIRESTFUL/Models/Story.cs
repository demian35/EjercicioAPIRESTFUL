//clase que modelara las historias que manejara el API desde La API de Hacker News
public class Story{
    public string Title{get; set;} //titulo de la historia
    public string URI{get; set;}//URI de la Historia
    public string PostedBy{get; set;}// Autor 
    public DateTime Time{get; set;}//fecha de publicacion
    public int Score{get; set;}//puntuacion de la historia
    public int CommentCount{get; set;}//cantidad de comentarios
}
