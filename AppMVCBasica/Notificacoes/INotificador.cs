namespace AppMVCBasica.Notificacoes;

public interface INotificador
{
    void Handle(string mensagem);
    bool TemNotificacoes();
}
