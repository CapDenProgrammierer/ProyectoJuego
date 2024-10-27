public class Enemigo2
{
    public bool Pistola { get; set; }
    public bool ArmaMelee { get; set; }
    public bool Arco {  get; set; }

    public void MostrarEquipamientoEnemigo()
    {
        Console.WriteLine($"Enemigo con: " +
                          $"{(Pistola ? "Pistola" : "")}" +
                          $"{(ArmaMelee ? "ArmaMelee" : "")}" +
                          $"{(Arco ? "Arco" : "")}");
    }

    public class EnemigoBuilder
    {
        private Enemigo2 _enemigo = new Enemigo2();

        public EnemigoBuilder AgregarPistola()
        {
            _enemigo.Pistola = true;
            return this;
        }
        public EnemigoBuilder AgregarArmaMelee()
        {
            _enemigo.ArmaMelee = true;
            return this;
        }
        public EnemigoBuilder AgregarArco()
        {
            _enemigo.Arco = true;
            return this;
        }
        public Enemigo2 Build() => _enemigo;
    }
}

