using AppMVCBasica.Models;
using AutoMapper;

namespace AppMVCBasica.AutoMapper;

public class AutoMapperConfig : Profile
{
	public AutoMapperConfig()
	{
		CreateMap<Produto, ProdutoViewModel>().ReverseMap();
		CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
		CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
    }
}
