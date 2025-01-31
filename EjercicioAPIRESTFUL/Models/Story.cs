//clase que modelara las historias que manejara el API desde La API de Hacker News
//model class that model a note of the Hacker News Api in the api
public class Story{
    public string Title{get; set;} //titulo de la historia. Title of the Story
    public string URI{get; set;}//URI de la Historia. Uri of the Story
    public string PostedBy{get; set;}// Autor. Author
    public DateTime Time{get; set;}//fecha de publicacion. Date of the posted
    public int Score{get; set;}//puntuacion de la historia. Score of Story
    public int CommentCount{get; set;}//cantidad de comentarios. Number of comments
}
