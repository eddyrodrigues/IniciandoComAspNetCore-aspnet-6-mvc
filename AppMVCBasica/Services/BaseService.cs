using AppMVCBasica.Notificacoes;

namespace AppMVCBasica.Services;

public abstract class BaseService : IServiceSys
{
	private INotificador _notificador;

	public BaseService(INotificador notificador)
	{
		_notificador = notificador;
	}
}
