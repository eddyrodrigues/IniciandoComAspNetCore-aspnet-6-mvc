using AppMVCBasica.Notificacoes;

namespace AppMVCBasica.Services;

public class FornecedorService : BaseService, IFornecedorService
{
    public FornecedorService(INotificador notificador) : base(notificador)
    {
        //notificador.Handle("Notificacao teste");
    }
}
