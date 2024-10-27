using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto
{
    //Singleton nos permitira garantizar que solo haya una instancia de Game
    public class Game
    {
        private static Game _instance;
        public int[] slot = new int[4];
        public int score {  get; private set; }
        private Game() 
        {
            slot = new int[4];
            this.score = score;

        }

        public static Game GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Game();
            }
            return _instance;
        }


        public void IniciarJuego()
        {
            Console.WriteLine("Selecciona un slot para empezar."); 
        }

        public void FinalizarJuego()
        { 
            Console.WriteLine("La partida ha finalizado.");
        }
    }
}