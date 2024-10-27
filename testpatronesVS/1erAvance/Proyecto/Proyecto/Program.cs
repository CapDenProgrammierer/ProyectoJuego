using Proyecto;
using System; 

class Program
{
    static void Main(string[] args)
    {
        //si es null, devolvera una nueva instancia de Game
        Game _instance = Game.GetInstance();
        if (_instance != null)
        {
            _instance.IniciarJuego();
        }
        else
        {
            Console.WriteLine("Primero cree una partida");
        }

        Torre torre = new Torre("Torre 1");
        Enemigo enemigo = new Enemigo("Mago", 50);
        _instance.FinalizarJuego();
    }
}