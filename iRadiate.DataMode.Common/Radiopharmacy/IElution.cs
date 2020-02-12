namespace iRadiate.DataModel.Radiopharmacy
{
    public interface IElution
    {
        double Breakthrough { get; set; }
        Generator Generator { get; set; }
    }
}