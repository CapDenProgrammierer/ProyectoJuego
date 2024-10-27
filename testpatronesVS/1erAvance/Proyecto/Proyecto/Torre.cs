using static System.Net.Mime.MediaTypeNames;
//Interfaz para definir la estrategia de ataque de la torre hacia los enemigos
public interface IEstrategiaAtaque
{
    void Atacar(Enemigo enemigo);
}
//Tipos de ataque
//01. Ataque Magico
public class AtaqueMagico : IEstrategiaAtaque
{
    public void Atacar(Enemigo enemigo)
    {
        Console.WriteLine("La torre ataca con magia");
    }
}
//02. Ataque HP
public class AtaqueHP : IEstrategiaAtaque
{
    public void Atacar(Enemigo enemigo)
    {
        Console.WriteLine("La torre ataca los HP del enemigo");
    }
}
//03. Ataque Mixto
public class AtaqueMixto : IEstrategiaAtaque
{
    public void Atacar(Enemigo enemigo)
    {
        Console.WriteLine("La torre usa ataques magicos y fisicos");
    }
}

//Interfaz para los estados de la torre
public interface IEstadoTorre
{
    void Funcionar(Torre torre);
    void Atacar(Torre torre);
}

//Estado Activo de la torre
public class Activa : IEstadoTorre
{
    public void Funcionar(Torre torre)
    {
        Console.WriteLine("El estado de la torre es activo");
    }
    public void Atacar(Torre torre)
    {
        Console.WriteLine("La torre esta atacando al enemigo");
    }
}
//Estado StandBy de la Torre
public class Descanso : IEstadoTorre
{
    public void Funcionar(Torre torre)
    {
        Console.WriteLine("La torre esta en descanso");
    }

    public void Atacar(Torre torre)
    {
        Console.WriteLine("La torre no puede atacar porque se encuentra en descanso");
    }
}
//Clase Torre
public class Torre
{
    private string nombre;
    private IEstadoTorre estadoActual;

    //Estado torre Activa
    static void Funcionar(Torre torre)
    {

    }
    //Estado ataque de la torre
    public void Atacar(Torre torre)
    {

    }
    //Metodo para cambiar el estado de la torre
    public void CambiarEstado(IEstadoTorre nuevoEstado)
    {
        estadoActual = nuevoEstado;
    }
    public Torre(string pNombre)
    {
        this.nombre = pNombre;
        estadoActual = new Activa();
    }
}
