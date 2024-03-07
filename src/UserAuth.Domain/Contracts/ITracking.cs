namespace UserAuth.Domain.Contracts;

public interface ITracking
{
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}