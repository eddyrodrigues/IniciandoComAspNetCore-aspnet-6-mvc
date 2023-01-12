using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppMVCBasica.Data;
using AppMVCBasica.Models;
using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using AppMVCBasica.Extensions;
using AppMVCBasica.Notificacoes;

namespace AppMVCBasica.Controllers;

[Authorize]
public class ProdutosController : OwnController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ProdutosController(ApplicationDbContext context, IMapper mapper, INotificador notificador) : base(notificador)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: Produtos
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _mapper.Map<List<ProdutoViewModel>>(await _context.Produtos.Include(p => p.Fornecedor).ToListAsync());
        return View(applicationDbContext);
    }

    // GET: Produtos/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        if (id == null || _context.Produtos == null)
        {
            return NotFound();
        }




        var produto = await ObterProdutoFornecedor(id);

        if (produto == null)
        {
            return NotFound();
        }

        return View(produto);
    }

    // GET: Produtos/Create
    [ClaimsAuthorize("Produto", "Adicionar")]
    public async Task<IActionResult> Create()
    {
        //ViewData["FornecedorId"] = new SelectList(_context.Fornecedores.Where(f => f.Ativo), "Id", "Nome");

        var produtoViewModel = await PopularProdutoViewModel(new ProdutoViewModel());
        return View(produtoViewModel);
    }

    // POST: Produtos/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ClaimsAuthorize("Produto", "Adicionar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProdutoViewModel produto)
    {
        produto = await PopularProdutoViewModel(produto);
        if (ModelState.IsValid)
        {
            var imgPrefixo = Guid.NewGuid() + "_";
            if (!await UploadArquivo(produto.ImagemUpload, imgPrefixo))
            {
                ModelState.AddModelError(nameof(ProdutoViewModel.Imagem), "Imagem não válida");
                return View(produto);
            }
            produto.Imagem = imgPrefixo + produto.ImagemUpload.FileName;
            var produto_novo = _mapper.Map<Produto>(produto);
            _context.Add(produto_novo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        AddError(ModelState);
        return View(produto);
    }


    // GET: Produtos/Edit/5
    [ClaimsAuthorize("Produto", "Editar")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return NotFound();
        }

        var produtoViewModel = _mapper.Map<ProdutoViewModel>(produto);
        produtoViewModel = await PopularProdutoViewModel(produtoViewModel);
        return View(produtoViewModel);
    }

    // POST: Produtos/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimsAuthorize("Produto", "Editar")]
    public async Task<IActionResult> Edit(Guid id,  ProdutoViewModel produto)
    {
        if (id != produto.Id)
        {
            return NotFound();
        }

        produto = await PopularProdutoViewModel(produto);
        if (!ModelState.IsValid) return View(produto);

        try
        {
            var imgPrefixo = Guid.NewGuid().ToString();
            if (!await UploadArquivo(produto.ImagemUpload, imgPrefixo))
            {
                ModelState.AddModelError(nameof(ProdutoViewModel.Imagem), "Selecione um imagem válida");
                return View(produto);
            }
            produto.Imagem = imgPrefixo + Path.GetExtension(produto.ImagemUpload.FileName);
            var produto_novo = _mapper.Map<Produto>(produto);

            _context.Update(produto_novo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProdutoExists(produto.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    // GET: Produtos/Delete/5
    [ClaimsAuthorize("Produto", "Excluir")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var produto = await ObterProdutoFornecedor(id);
        if (produto == null)
        {
            return NotFound();
        }

        return View(_mapper.Map<ProdutoViewModel>(produto));
    }

    // POST: Produtos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ClaimsAuthorize("Produto", "Excluir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Produtos == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Produtos'  is null.");
        }
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProdutoExists(Guid id)
    {
      return _context.Produtos.Any(e => e.Id == id);
    }

    async Task<List<ProdutoViewModel>> ObterProdutosFornecedor()
    {
        var produtos = _mapper.Map<List<ProdutoViewModel>>(await _context.Produtos.Include(a => a.Fornecedor).ToListAsync());
        return produtos;
    }

    async Task<ProdutoViewModel> PopularProdutoViewModel(ProdutoViewModel produto)
    {
        produto.Fornecedores = _mapper.Map<List<FornecedorViewModel>>(await _context.Fornecedores.ToListAsync());
        return produto;
    }

    async Task<ProdutoViewModel> ObterProdutoFornecedor(Guid id)
    {
        var produto = _mapper.Map<ProdutoViewModel>(await _context.Produtos.Include(t => t.Fornecedor).Where(t => t.Id == id).FirstOrDefaultAsync());
        return produto;
    }
    /// <summary>
    /// Retorna o nome da imagem
    /// </summary>
    /// <param name="produtoViewModel"></param>
    /// <returns></returns>
    private async Task<bool> UploadArquivo(IFormFile arquivo, string imgPrefixo)
    {

        if (arquivo == null) return false;
        if (arquivo.Length <= 0) return false;

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + Path.GetExtension(arquivo.FileName));

        if (System.IO.File.Exists(path))
        {
            ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
            return false;
        }

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        return true;
    }

    public void AddError(ModelStateDictionary model)
    {
        foreach(var item in model.Keys)
        {
            foreach(var item2 in model[item].Errors)
            {
                model.AddModelError("", item2.ErrorMessage);
            }
        }
    }
}
