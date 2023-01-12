using AppMVCBasica.Notificacoes;
using Microsoft.AspNetCore.Mvc;

namespace AppMVCBasica.Controllers;

public class OwnController : Controller
{
	private readonly INotificador notificador;

	public OwnController(INotificador notificador)
	{
		this.notificador = notificador;
	}

	protected bool OperacaoEhValida()
	{
		return !notificador.TemNotificacoes();
	}

}
