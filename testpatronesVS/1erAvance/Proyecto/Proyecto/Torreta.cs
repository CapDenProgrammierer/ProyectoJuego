public class Torreta
{
    public bool Arma { get; set; }
    public bool BaritaMagica { get; set; }
    public bool ArmaMixta { get; set; }


    public void MostrarEquipamiento()
    {
        Console.WriteLine($"Torre con: " +
                          $"{(Arma ? "Arma" : "")}"+
                          $"{(BaritaMagica ? "BaritaMagica" : "")}"+
                          $"{(ArmaMixta ? "ArmaMixta" : "")}");
    }
}

public class TorreBuilder
{
    private Torreta _torreta = new Torreta();

    public TorreBuilder AgregarArma()
    {
        _torreta.Arma = true;
        return this;
    }
    public TorreBuilder AgregarBaritaMagica()
    {
        _torreta.BaritaMagica = true;
        return this;
    }
    public TorreBuilder AgregarArmaMixta()
    {
        _torreta.ArmaMixta = true;
        return this;
    }

    public Torreta Build() => _torreta;
}