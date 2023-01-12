namespace AppMVCBasica.Notificacoes;

/// <summary>
/// Notificações
/// </summary>
public class Notificador : INotificador
{
    private List<string> _notificacoes { get; }
    public Notificador()
    {
        _notificacoes = new List<string>();
    }
    public void Handle(string mensagem)
    {
        _notificacoes.Add(mensagem);
    }
    public bool TemNotificacoes()
    {
        return _notificacoes.Any();
    }
}