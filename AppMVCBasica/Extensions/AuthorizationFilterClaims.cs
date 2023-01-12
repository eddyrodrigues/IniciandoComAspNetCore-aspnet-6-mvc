using Microsoft.AspNetCore.Authorization;

namespace AppMVCBasica.Extensions;

public class ProdutoPermissaoNecessaria : IAuthorizationRequirement
{
    public string Permissao { get; set; }
	public ProdutoPermissaoNecessaria(string permissao)
	{
		Permissao = permissao;
	}
}

public class PermissaoNecessariaHandler : AuthorizationHandler<ProdutoPermissaoNecessaria>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProdutoPermissaoNecessaria requirement)
	{
		if (context.User.HasClaim(c => c.Type == "Produtos" && c.Value.Contains(requirement.Permissao)))
		{
			context.Succeed(requirement);
		}
		return Task.CompletedTask;
	}
}
